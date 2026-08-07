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
                    int currentPath = CalculatePath(path, room);

                    if (currentPath < minPathLength)
                    {
                        minPathLength = currentPath;
                        bestPath = path;
                    }
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

        Dictionary<Vector2Int, int> dist = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        List<Vector2Int> open = new List<Vector2Int>();

        dist[start] = 0;
        open.Add(start);

        bool isFind = false;

        while (open.Count > 0)
        {
            int minIdx = 0;

            for (int i = 1; i < open.Count; i++)
            {
                if (dist[open[i]] < dist[open[minIdx]])
                {
                    minIdx = i;
                }
            }

            Vector2Int current = open[minIdx];
            open.RemoveAt(minIdx);

            if (current == end)
            {
                isFind = true;
                break;
            }

            foreach (Vector2Int dir in _directions)
            {
                Vector2Int next = current + dir;

                if (IsRoom(next, room))
                {
                    continue;
                }

                if (room.TryGetValue(next, out RoomViewModel vm) && vm.BuildType == BuildType.Room)
                {
                    continue;
                }

                int move = 100;

                if (room.TryGetValue(next, out RoomViewModel roomVM) && roomVM.BuildType == BuildType.Aisle)
                {
                    move = 1;
                }

                int newDis = dist[current] + move;

                if (!dist.ContainsKey(next) || newDis < dist[next])
                {
                    dist[next] = newDis;
                    parent[next] = current;

                    if (!open.Contains(next))
                    {
                        open.Add(next);
                    }
                }
            }
        }

        if (!isFind)
        {
            return null;
        }

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int curr = end;

        int count = 0;
        int maxCount = 1000;

        while (curr != start && count < maxCount)
        {
            path.Add(curr);

            if (!parent.TryGetValue(curr, out curr))
            {
                return null;
            }

            count++;
        }

        if (count >= maxCount)
        {
            return null;
        }

        path.Add(start);
        path.Reverse();

        return path;
    }

    private int CalculatePath(List<Vector2Int> path, Dictionary<Vector2Int, RoomViewModel> room)
    {
        int total = 0;

        foreach (Vector2Int pos in path)
        {
            if (room.TryGetValue(pos, out RoomViewModel vm) && vm.BuildType == BuildType.Aisle)
            {
                total += 1;
            }
            else
            {
                total += 100;
            }
        }

        return total;
    }

    private bool IsRoom(Vector2Int pos, Dictionary<Vector2Int, RoomViewModel> roomMap)
    {
        return roomMap.TryGetValue(pos, out RoomViewModel vm) && vm.BuildType == BuildType.Room;
    }
}