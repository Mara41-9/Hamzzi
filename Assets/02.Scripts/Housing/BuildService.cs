using System.Collections.Generic;
using UnityEngine;

public struct AisleConnection
{
    public bool Up;
    public bool Down;
    public bool Left;
    public bool Right;
}

public class BuildService
{
    private static readonly Vector2Int[] _directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    private BuildViewModel _buildVM;

    public BuildViewModel GetBuildViewModel()
    {
        if (_buildVM == null)
        {
            CreateBuildViewModel();
        }

        return _buildVM;
    }

    public BuildViewModel CreateBuildViewModel()
    {
        _buildVM = new BuildViewModel();

        return _buildVM;
    }

    public List<Vector2Int> SearchBestPath(RoomViewModel startRoom, RoomViewModel endRoom, Dictionary<Vector2Int, RoomViewModel> room)
    {
        var startDoors = startRoom.DoorDataList;
        var endDoors = endRoom.DoorDataList;

        List<Vector2Int> bestPath = null;
        int minPathLength = int.MaxValue;

        foreach (DoorData startData in startDoors)
        {
            DoorInfo startInfo = startRoom.GetDoorInfo(startData.Offset);

            if (IsRoom(startInfo.OutsidePos, room))
            {
                continue;
            }

            foreach (DoorData endData in endDoors)
            {
                DoorInfo endInfo = endRoom.GetDoorInfo(endData.Offset);

                if (IsRoom(endInfo.OutsidePos, room))
                {
                    continue;
                }

                Vector2Int startDir = _directions[startInfo.DirectionIndex];
                List<Vector2Int> path = GetAislePath(startInfo.OutsidePos, endInfo.OutsidePos, room);

                if (path != null && path.Count > 0 && path.Count < minPathLength)
                {
                    minPathLength = path.Count;
                    bestPath = path;
                }
            }
        }

        return bestPath;
    }

    public List<Vector2Int> GetAislePath(Vector2Int start, Vector2Int end, Dictionary<Vector2Int, RoomViewModel> room)
    {
        if (start == end || IsRoom(start, room) || IsRoom(end, room))
        {
            return null;
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        bool isFind = false;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == end)
            {
                isFind = true;
                break;
            }

            foreach (Vector2Int dir in _directions)
            {
                Vector2Int next = current + dir;

                if (visited.Contains(next) || IsRoom(next, room))
                {
                    continue;
                }

                visited.Add(next);
                parent[next] = current;
                queue.Enqueue(next);
            }
        }

        if (!isFind)
        {
            return null;
        }

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int curr = end;

        while (curr != start)
        {
            path.Add(curr);
            curr = parent[curr];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }

    private bool IsRoom(Vector2Int pos, Dictionary<Vector2Int, RoomViewModel> room)
    {
        if (room.TryGetValue(pos, out RoomViewModel vm))
        {
            return vm.BuildType == BuildType.Room;
        }

        return false;
    }
}