using System.Collections.Generic;
using UnityEngine;

public class BuildViewModel : ViewModelBase
{
    private static readonly Vector2Int[] _directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public Dictionary<Vector2Int, RoomViewModel> Builds { get; private set; } = new Dictionary<Vector2Int, RoomViewModel>();

    private bool _hasStartPos = false;
    private RoomViewModel _startRoom;

    private RoomViewModel _waitingRoom;
    private List<RoomViewModel> _waitingAisle = new List<RoomViewModel>();

    public bool IsLoading { get; set; } = false;

    public bool CanDestroy
    {
        get
        {
            return SelectRoom != null && SelectRoom.IsReady && SelectType == BuildType.Room && !SelectRoom.IsDefault;
        }
    }

    public bool CanConnectAisle
    {
        get
        {
            return SelectRoom != null && SelectRoom.IsReady && SelectType == BuildType.Room;
        }
    }

    private bool _canConfirm = false;
    public bool CanConfirm
    {
        get => _canConfirm;
        set
        {
            if (_canConfirm != value)
            {
                _canConfirm = value;
                OnPropertyChanged(nameof(CanConfirm));
            }
        }
    }

    private RoomViewModel _selectRoom;
    public RoomViewModel SelectRoom
    {
        get => _selectRoom;
        set
        {
            if (_selectRoom != value)
            {
                _selectRoom = value;
                OnPropertyChanged(nameof(SelectRoom));
                OnPropertyChanged(nameof(CanDestroy));
                OnPropertyChanged(nameof(CanConnectAisle));
            }
        }
    }


    private RoomViewModel _destroyBuild;
    public RoomViewModel DestroyBuild
    {
        get => _destroyBuild;
        set
        {
            if (_destroyBuild != value)
            {
                _destroyBuild = value;
                OnPropertyChanged(nameof(DestroyBuild));
            }
        }
    }

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
                OnPropertyChanged(nameof(CanDestroy));
                OnPropertyChanged(nameof(CanConnectAisle));
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

    public void ChooseRoom(RoomViewModel room)
    {
        if (room == null || room.BuildType != BuildType.Room)
        {
            return;
        }

        SelectRoom = room;
    }

    public void DeselectRoom()
    {
        SelectRoom = null;
    }

    public void StartConnectingAisle()
    {
        if (SelectRoom == null)
        {
            return;
        }

        _startRoom = SelectRoom;
        _hasStartPos = true;

        DeselectRoom();
        SelectType = BuildType.Aisle;
    }

    public void DestroyRoom()
    {
        if (SelectRoom == null)
        {
            return;
        }

        RoomViewModel target = SelectRoom;
        DeselectRoom();

        RemoveBuild(target);
        ClearAisle();
    }

    public void ClearAisle()
    {
        while (true)
        {
            HashSet<RoomViewModel> deadEndAisles = new HashSet<RoomViewModel>();

            foreach (var pair in Builds)
            {
                Vector2Int pos = pair.Key;
                RoomViewModel vm = pair.Value;

                if (vm.BuildType == BuildType.Aisle && !vm.IsDefault)
                {
                    if (CountAisle(pos, vm) <= 1)
                    {
                        deadEndAisles.Add(vm);
                    }
                }
            }

            if (deadEndAisles.Count == 0)
            {
                break;
            }

            foreach (var aisle in deadEndAisles)
            {
                RemoveBuild(aisle);
            }
        }
    }

    private int CountAisle(Vector2Int pos, RoomViewModel aisleVM)
    {
        int count = 0;

        for (int i = 0; i < _directions.Length; i++)
        {
            Vector2Int targetPos = pos + _directions[i];

            if (Builds.TryGetValue(targetPos, out RoomViewModel targetVM))
            {
                if (targetVM == aisleVM)
                {
                    continue;
                }

                if (targetVM.BuildType == BuildType.Aisle)
                {
                    count++;
                }
                else if (targetVM.BuildType == BuildType.Room)
                {
                    Vector2Int doorPos = targetVM.GetNearDoor(pos);

                    if (targetPos == doorPos && !targetVM.IsDefault)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    public void EnterBuildMode()
    {
        CancelBuildMode();
        DeselectRoom();
        SelectType = BuildType.Room;
    }

    public void ConfirmBuild()
    {
        DeselectRoom();

        if (_waitingRoom != null)
        {
            _waitingRoom.IsReady = true;
            _waitingRoom = null;
        }

        foreach (RoomViewModel aisle in _waitingAisle)
        {
            aisle.IsReady = true;
        }

        _waitingAisle.Clear();
        CanConfirm = false;
        SelectType = BuildType.None;
    }

    public void CancelBuildMode()
    {
        DeselectRoom();

        if (_waitingRoom != null)
        {
            RemoveBuild(_waitingRoom);
            _waitingRoom = null;
        }

        ClearWaitingAisle();

        ResetAisle();
        CanConfirm = false;
        SelectType = BuildType.None;
    }

    public void InitDefaultRoom(List<Vector2Int> defaultRoom, List<Vector2Int> defaultAisle)
    {
        foreach (Vector2Int pos in defaultRoom)
        {
            BuildDefaultRoom(pos);
        }

        foreach (Vector2Int pos in defaultAisle)
        {
            BuildDefaultAisle(pos);
        }

        Vector2Int exitAislePos = defaultAisle[0];

        if (Builds.TryGetValue(exitAislePos, out RoomViewModel aisleVM) && aisleVM.BuildType == BuildType.Aisle)
        {
            aisleVM.SetWallActive(0, true);
            aisleVM.Refresh();
        }
    }

    public bool TryBuildRoom(Vector2Int pos)
    {
        RoomViewModel newRoom = new RoomViewModel(BuildType.Room, pos);

        if (!CanPlaceRoom(pos, newRoom.Size))
        {
            return false;
        }

        RegisterRoom(newRoom, pos);
        UpdateRoomConnection(newRoom);

        _waitingRoom = newRoom;
        LastBuild = newRoom;

        _startRoom = newRoom;
        _hasStartPos = true;
        SelectType = BuildType.Aisle;

        return true;
    }

    private void BuildDefaultRoom(Vector2Int pos)
    {
        RoomViewModel newRoom = new RoomViewModel(BuildType.Room, pos);
        newRoom.IsReady = true;
        newRoom.IsDefault = true;

        RegisterRoom(newRoom, pos);
        UpdateRoomConnection(newRoom);

        LastBuild = newRoom;
    }

    public void BuildDefaultAisle(Vector2Int pos)
    {
        if (Builds.ContainsKey(pos))
        {
            return;
        }

        RoomViewModel newAisle = new RoomViewModel(BuildType.Aisle, pos);
        newAisle.IsDefault = true;
        Builds[pos] = newAisle;

        UpdateConnection(pos);
        UpdateNearConnection(pos);

        LastBuild = newAisle;
    }

    public bool TryBuildAisle(Vector2Int pos)
    {
        if (!_hasStartPos || _startRoom == null)
        {
            return false;
        }

        if (!Builds.TryGetValue(pos, out RoomViewModel targetVM) || targetVM.BuildType != BuildType.Room || !targetVM.IsReady || _startRoom == targetVM)
        {
            return false;
        }

        List<Vector2Int> path = ServiceManager.Instance.BuildService.SearchBestPath(_startRoom, targetVM, Builds);

        if (path == null || path.Count == 0)
        {
            return false;
        }

        foreach (Vector2Int aislePos in path)
        {
            if (Builds.TryGetValue(aislePos, out RoomViewModel existing) && existing.BuildType == BuildType.Room)
            {
                continue;
            }

            if (!Builds.ContainsKey(aislePos))
            {
                RoomViewModel newAisle = new RoomViewModel(BuildType.Aisle, aislePos);
                Builds[aislePos] = newAisle;

                _waitingAisle.Add(newAisle);
                LastBuild = newAisle;
            }

            UpdateConnection(aislePos);
            UpdateNearConnection(aislePos);
        }

        SelectType = BuildType.None;

        CanConfirm = true;
        return true;
    }

    private void ResetAisle()
    {
        _hasStartPos = false;
        _startRoom = null;
    }

    private void ClearWaitingAisle()
    {
        foreach (RoomViewModel aisle in _waitingAisle)
        {
            RemoveBuild(aisle);
        }

        _waitingAisle.Clear();
        CanConfirm = false;
    }

    private void RemoveBuild(RoomViewModel target)
    {
        for (int x = 0; x < target.Size.x; x++)
        {
            for (int y = 0; y < target.Size.y; y++)
            {
                Vector2Int tilePos = target.OriginPos + new Vector2Int(x, y);

                if (Builds.TryGetValue(tilePos, out RoomViewModel existVM) && existVM == target)
                {
                    Builds.Remove(tilePos);
                }

                UpdateNearConnection(tilePos);
            }
        }

        DestroyBuild = target;
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

            if (Builds.TryGetValue(doorInfo.OutsidePos, out RoomViewModel targetVM))
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
        if (!Builds.TryGetValue(current, out RoomViewModel currentVM))
        {
            return;
        }

        if (currentVM.BuildType == BuildType.Room)
        {
            UpdateRoomConnection(currentVM);
            return;
        }

        for (int i = 0; i < _directions.Length; i++)
        {
            Vector2Int targetPos = current + _directions[i];
            int oppDir = GetOppositeDirection(i);

            if (Builds.TryGetValue(targetPos, out RoomViewModel targetVM))
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
        foreach (Vector2Int dir in _directions)
        {
            Vector2Int target = pos + dir;

            if (Builds.TryGetValue(target, out RoomViewModel vm))
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

    private bool CanPlaceRoom(Vector2Int pos, Vector2Int size)
    {
        if (pos.y + size.y > 11)
        {
            return false;
        }

        if (pos.y < -24 || pos.x < -39 || pos.x + size.x > 39)
        {
            return false;
        }

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (Builds.ContainsKey(pos + new Vector2Int(x, y)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void RegisterRoom(RoomViewModel room, Vector2Int pos)
    {
        for (int x = 0; x < room.Size.x; x++)
        {
            for (int y = 0; y < room.Size.y; y++)
            {
                Builds[pos + new Vector2Int(x, y)] = room;
            }
        }
    }
}