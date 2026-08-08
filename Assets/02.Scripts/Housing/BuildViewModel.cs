using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BuildViewModel : ViewModelBase
{
    private Dictionary<Vector2Int, RoomViewModel> _builds = new Dictionary<Vector2Int, RoomViewModel>();
    private BuildService _buildService = new BuildService();        // 임시

    private bool _hasStartPos = false;
    private RoomViewModel _startRoom;

    private BuildType _selectType = BuildType.None;
    public BuildType SelectType
    {
        get => _selectType;
        set
        {
            if (_selectType != value)
            {
                _selectType = value;
                OnPropertyChanged(nameof(SelectType));
            }
        }
    }

    private RoomViewModel _lastBuild;
    public RoomViewModel LastBuild
    {
        get => _lastBuild;
        set
        {
            if (_lastBuild != value)
            {
                _lastBuild = value;
                OnPropertyChanged(nameof(LastBuild));
            }
        }
    }

    public void EnterBuildMode()
    {
        ResetAisle();
        SelectType = BuildType.Room;
    }

    public void ExitBuildMode()
    {
        ResetAisle();
        SelectType = BuildType.None;
    }

    public void InitDefaultRoom(List<Vector2Int> defaultRoom, List<Vector2Int> defaultAisle)
    {
        foreach (Vector2Int pos in defaultRoom)
        {
            TryBuildRoom(pos);
        }

        foreach (Vector2Int pos in defaultAisle)
        {
            BuildDefaultAisle(pos);
        }

        Vector2Int exitAislePos = defaultAisle[0];

        if (_builds.TryGetValue(exitAislePos, out RoomViewModel aisleVM) && aisleVM.BuildType == BuildType.Aisle)
        {
            aisleVM.SetWallActive(0, true);
            aisleVM.Refresh();
        }
    }

    public bool TryBuildRoom(Vector2Int pos)
    {
        RoomViewModel newRoom = new RoomViewModel(BuildType.Room, pos);

        for (int x = 0; x < newRoom.Size.x; x++)
        {
            for (int y = 0; y < newRoom.Size.y; y++)
            {
                Vector2Int checkPos = pos + new Vector2Int(x, y);

                if (_builds.ContainsKey(checkPos))
                {
                    return false;
                }
            }
        }

        for (int x = 0; x < newRoom.Size.x; x++)
        {
            for (int y = 0; y < newRoom.Size.y; y++)
            {
                Vector2Int tilePos = pos + new Vector2Int(x, y);
                _builds[tilePos] = newRoom;
            }
        }

        newRoom.PropertyChanged += OnRoomPropertyChanged;
        UpdateRoomConnection(newRoom);

        LastBuild = newRoom;

        _startRoom = newRoom;
        _hasStartPos = true;
        SelectType = BuildType.Aisle;

        return true;
    }

    private void OnRoomPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RoomViewModel.DoorDataList) && sender is RoomViewModel room)
        {
            UpdateRoomConnection(room);
            room.PropertyChanged -= OnRoomPropertyChanged;
        }
    }

    public void BuildDefaultAisle(Vector2Int pos)
    {
        if (_builds.ContainsKey(pos))
        {
            return;
        }

        RoomViewModel newAisle = new RoomViewModel(BuildType.Aisle, pos);
        _builds[pos] = newAisle;

        UpdateConnection(pos);
        UpdateNearConnection(pos);

        LastBuild = newAisle;
    }

    public bool TryBuildAisle(Vector2Int pos)
    {
        if (!_hasStartPos)
        {
            if (!_builds.TryGetValue(pos, out RoomViewModel clickedVM) || clickedVM.BuildType != BuildType.Room || !clickedVM.IsReady)
            {
                return false;
            }

            _startRoom = clickedVM;
            _hasStartPos = true;

            return true;
        }

        if (!_builds.TryGetValue(pos, out RoomViewModel targetVM) || targetVM.BuildType != BuildType.Room || _startRoom == targetVM)
        {
            ResetAisle();
            return false;
        }

        List<Vector2Int> path = _buildService.SearchBestPath(_startRoom, targetVM, _builds);

        if (path == null || path.Count == 0)
        {
            ResetAisle();
            return false;
        }

        ResetAisle();
        RoomViewModel lastAisle = null;

        foreach (Vector2Int aislePos in path)
        {
            if (_builds.TryGetValue(aislePos, out RoomViewModel existing) && existing.BuildType == BuildType.Room)
            {
                continue;
            }

            if (!_builds.ContainsKey(aislePos))
            {
                RoomViewModel newAisle = new RoomViewModel(BuildType.Aisle, aislePos);
                _builds[aislePos] = newAisle;
                LastBuild = newAisle;
            }

            UpdateConnection(aislePos);
            UpdateNearConnection(aislePos);
        }

        if (lastAisle != null)
        {
            LastBuild = lastAisle;
        }

        SelectType = BuildType.None;

        return true;
    }

    private void ResetAisle()
    {
        _hasStartPos = false;
        _startRoom = null;
    }

    public void UpdateRoomConnection(RoomViewModel room)
    {
        if (room == null || room.BuildType != BuildType.Room)
        {
            return;
        }

        if (room.DoorDataList == null || room.DoorDataList.Count == 0)
        {
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            room.SetWallActive(i, false);
        }

        foreach (DoorData doorData in room.DoorDataList)
        {
            DoorInfo doorInfo = room.GetDoorInfo(doorData.Offset);

            if (_builds.TryGetValue(doorInfo.OutsidePos, out RoomViewModel targetVM))
            {
                if (targetVM == room)
                {
                    continue;
                }

                bool shouldConnect = false;

                if (targetVM.BuildType == BuildType.Aisle)
                {
                    shouldConnect = true;
                }
                else if (targetVM.BuildType == BuildType.Room)
                {
                    Vector2Int neighborDoorPos = targetVM.GetNearDoor(doorInfo.InsidePos);
                    shouldConnect = (doorInfo.OutsidePos == neighborDoorPos);
                }

                if (shouldConnect)
                {
                    room.SetWallActive(doorInfo.DirectionIndex, true);
                    targetVM.SetWallActive(GetOppositeDirection(doorInfo.DirectionIndex), true);
                    targetVM.Refresh();
                }
            }
        }

        room.Refresh();
    }

    public void UpdateConnection(Vector2Int current)
    {
        if (!_builds.TryGetValue(current, out RoomViewModel currentVM))
        {
            return;
        }

        if (currentVM.BuildType == BuildType.Room)
        {
            UpdateRoomConnection(currentVM);
            return;
        }

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int targetPos = current + directions[i];
            int oppDir = GetOppositeDirection(i);

            if (_builds.TryGetValue(targetPos, out RoomViewModel targetVM))
            {
                if (currentVM == targetVM)
                {
                    continue;
                }

                bool shouldConnect = false;

                if (targetVM.BuildType == BuildType.Aisle)
                {
                    shouldConnect = true;
                }
                else if (targetVM.BuildType == BuildType.Room)
                {
                    Vector2Int doorPos = targetVM.GetNearDoor(current);
                    shouldConnect = (targetPos == doorPos);
                }

                currentVM.SetWallActive(i, shouldConnect);

                if (shouldConnect)
                {
                    targetVM.SetWallActive(oppDir, true);
                    targetVM.Refresh();
                }
            }
            else
            {
                currentVM.SetWallActive(i, false);
            }
        }

        currentVM.Refresh();
    }

    public void UpdateNearConnection(Vector2Int pos)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int target = pos + dir;

            if (_builds.TryGetValue(target, out RoomViewModel vm))
            {
                if (vm.BuildType == BuildType.Room)
                {
                    UpdateRoomConnection(vm);
                }
                else
                {
                    UpdateConnection(target);
                }
            }
        }
    }

    private int GetOppositeDirection(int direction)
    {
        switch (direction)
        {
            case 0: return 1;
            case 1: return 0;
            case 2: return 3;
            case 3: return 2;
            default: return 0;
        }
    }
}