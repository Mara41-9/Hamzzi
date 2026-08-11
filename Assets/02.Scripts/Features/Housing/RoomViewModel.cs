using System;
using System.Collections.Generic;
using UnityEngine;

public enum BuildType
{
    None,
    Room,
    Aisle
}

public struct DoorInfo
{
    public Vector2Int InsidePos;
    public Vector2Int OutsidePos;
    public int DirectionIndex;
}

public struct DoorData
{
    public Vector2Int Offset;
    public int DirectionIndex;
}

public class RoomViewModel : ViewModelBase
{
    private int _roomWidth = 6;
    private int _roomHeight = 4;

    public bool IsReady { get; set; } = false;
    public List<DoorData> DoorDataList { get; private set; } = new List<DoorData>();

    private Dictionary<Vector2Int, FurnitureViewModel> _furnitureGrid = new Dictionary<Vector2Int, FurnitureViewModel>();
    
    public List<FurnitureViewModel> FurnitureList { get; private set; } = new List<FurnitureViewModel>();
    public Vector2Int SubGridSize => new Vector2Int(Size.x * _gridFactor, Size.y * _gridFactor);

    private int _gridFactor = 2;
    public int GridFactor
    {
        get => _gridFactor;
    }

    private string _instanceID;
    public string InstanceID
    {
        get => _instanceID;
        set
        {
            if (_instanceID != value)
            {
                _instanceID = value;
                OnPropertyChanged(nameof(InstanceID));
            }
        }
    }

    private BuildType _buildType;
    public BuildType BuildType
    {
        get => _buildType;
        set
        {
            if (_buildType != value)
            {
                _buildType = value;
                OnPropertyChanged(nameof(BuildType));
            }
        }
    }

    private Vector2Int _originPos;
    public Vector2Int OriginPos
    {
        get => _originPos;
        set
        {
            if (_originPos != value)
            {
                _originPos = value;
                OnPropertyChanged(nameof(OriginPos));
            }
        }
    }

    private Vector2Int _size;
    public Vector2Int Size
    {
        get => _size;
        set
        {
            if (_size != value)
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
            }
        }
    }

    private AisleConnection _aisleConnection;
    public AisleConnection AisleConnection
    {
        get => _aisleConnection;
        set
        {
            if (_aisleConnection.Up != value.Up || _aisleConnection.Down != value.Down ||
                _aisleConnection.Left != value.Left || _aisleConnection.Right != value.Right)
            {
                _aisleConnection = value;
                OnPropertyChanged(nameof(AisleConnection));
            }
        }
    }

    public RoomViewModel(BuildType type, Vector2Int pos)
    {
        InstanceID = Guid.NewGuid().ToString();
        BuildType = type;
        OriginPos = pos;
        Size = (type == BuildType.Room) ? new Vector2Int(_roomWidth, _roomHeight) : Vector2Int.one;

        if (type == BuildType.Aisle)
        {
            IsReady = true;
            DoorDataList.Add(new DoorData { Offset = Vector2Int.zero, DirectionIndex = 0 });
        }
        else if (type == BuildType.Room)
        {
            InitDefaultDoor();
        }
    }

    // 건설 관련
    private void InitDefaultDoor()
    { 
        int centerY = 0;

        List<DoorData> defaultDoors = new List<DoorData>
        {
            new DoorData { Offset = new Vector2Int(0, centerY), DirectionIndex = 2 },
            new DoorData { Offset = new Vector2Int(Size.x - 1, centerY), DirectionIndex = 3 }
        };

        SetDoorData(defaultDoors);
    }

    public void SetDoorData(List<DoorData> doorDataList)
    {
        DoorDataList = doorDataList;
        OnPropertyChanged(nameof(DoorDataList));
    }

    public Vector2Int GetNearDoor(Vector2Int targetPos)
    {
        if (DoorDataList == null || DoorDataList.Count == 0)
        {
            InitDefaultDoor();
        }

        Vector2Int nearDoor = OriginPos + DoorDataList[0].Offset;
        int minDist = int.MaxValue;

        foreach (DoorData doorData in DoorDataList)
        {
            DoorInfo info = GetDoorInfo(doorData.Offset);

            int dist = Mathf.Abs(info.OutsidePos.x - targetPos.x) + Mathf.Abs(info.OutsidePos.y - targetPos.y);

            if (dist < minDist)
            {
                minDist = dist;
                nearDoor = OriginPos + doorData.Offset;
            }
        }

        return nearDoor;
    }

    public void SetWallActive(int dir, bool isConnected)
    {
        AisleConnection current = AisleConnection;

        switch (dir)
        {
            case 0:
                current.Up = isConnected;
                break;

            case 1:
                current.Down = isConnected;
                break;

            case 2:
                current.Left = isConnected;
                break;

            case 3:
                current.Right = isConnected;
                break;
        }

        AisleConnection = current;
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(AisleConnection));
    }

    public DoorInfo GetDoorInfo(Vector2Int doorOffset)
    {
        Vector2Int inside = OriginPos + doorOffset;
        int dirIndex = (doorOffset.x == 0) ? 2 : 3;

        if (DoorDataList != null)
        {
            foreach (DoorData data in DoorDataList)
            {
                if (data.Offset == doorOffset && data.DirectionIndex >= 0 && data.DirectionIndex <= 3)
                {
                    dirIndex = data.DirectionIndex;
                    break;
                }
            }
        }

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector2Int outside = inside + dirs[dirIndex];

        if (outside.x >= OriginPos.x && outside.x < OriginPos.x + Size.x &&
            outside.y >= OriginPos.y && outside.y < OriginPos.y + Size.y)
        {
            switch (dirIndex)
            {
                case 2:
                    outside = new Vector2Int(OriginPos.x - 1, inside.y);
                    break;

                case 3:
                    outside = new Vector2Int(OriginPos.x + Size.x, inside.y);
                    break;
            }
        }

        return new DoorInfo { InsidePos = inside, OutsidePos = outside, DirectionIndex = dirIndex };
    }

    // 하우징 관련
    public Vector2Int ChangeLocalGrid(Vector3 worldPos, float cellSize = 1.0f)
    {
        float subCellSize = cellSize / _gridFactor;

        float localX = worldPos.x - (OriginPos.x * cellSize);
        float localZ = 9.0f - worldPos.z;

        int subX = Mathf.Clamp(Mathf.FloorToInt(localX / subCellSize), 0, SubGridSize.x - 1);
        int subZ = Mathf.Clamp(Mathf.FloorToInt(localZ / subCellSize), 0, SubGridSize.y - 1);

        return new Vector2Int(subX, subZ);
    }

    public bool IsValidPlace(Vector2Int localPos, Vector2Int furnitureSize)
    {
        for (int x = 0; x < furnitureSize.x; x++)
        {
            for (int y = 0; y < furnitureSize.y; y++)
            {
                Vector2Int checkPos = localPos + new Vector2Int(x, y);

                if (checkPos.x < 0 || checkPos.x >= SubGridSize.x || checkPos.y < 0 || checkPos.y >= SubGridSize.y)
                {
                    return false;
                }

                if (_furnitureGrid.ContainsKey(checkPos))
                {
                    return false;
                }

                if (IsDoorPos(checkPos))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool IsDoorPos(Vector2Int checkPos)
    {
        foreach (DoorData door in DoorDataList)
        {
            int minX = door.Offset.x * _gridFactor;
            int maxX = minX + _gridFactor;
            int minY = door.Offset.y * _gridFactor;
            int maxY = minY + _gridFactor;

            if (checkPos.x >= minX && checkPos.x < maxX && checkPos.y >= minY && checkPos.y < maxY)
            {
                return true;
            }
        }

        return false;
    }

    public bool AddFuniture(FurnitureViewModel furnitureVM)
    {
        if (!IsValidPlace(furnitureVM.LocalPos, furnitureVM.Size))
        {
            return false;
        }

        for (int x = 0; x < furnitureVM.Size.x; x++)
        {
            for (int y = 0; y < furnitureVM.Size.y; y++)
            {
                _furnitureGrid[furnitureVM.LocalPos + new Vector2Int(x, y)] = furnitureVM;
            }
        }

        FurnitureList.Add(furnitureVM);
        OnPropertyChanged(nameof(FurnitureList));

        return true;
    }
}