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
    private static readonly Vector2Int[] _diagonalDirs = { new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(1, 1) };

    private const int MAX_SEARCH = 500;

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
        List<DoorData> startDoors = startRoom.DoorDataList;
        List<DoorData> endDoors = endRoom.DoorDataList;

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

                List<Vector2Int> path = GetAislePath(startInfo.OutsidePos, endInfo.OutsidePos, room);

                if (path != null && path.Count > 0)
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
        if (IsRoom(start, room) || IsRoom(end, room))
        {
            return null;
        }

        if (start == end)
        {
            return new List<Vector2Int> { start };
        }

        Dictionary<Vector2Int, int> dist = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        List<Vector2Int> open = new List<Vector2Int>();

        dist[start] = 0;
        open.Add(start);

        bool isFind = false;
        int searchCount = 0;

        while (open.Count > 0 && searchCount < MAX_SEARCH)
        {
            searchCount++;

            int minIdx = 0;
            int minScore = dist[open[0]] + CalculateDistance(open[0], end);

            for (int i = 1; i < open.Count; i++)
            {
                int score = dist[open[i]] + CalculateDistance(open[i], end);

                if (score < minScore)
                {
                    minScore = score;
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

                if (IsRoom(next, room) || IsNearRoomCorner(next, room))
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
        if (roomMap.TryGetValue(pos, out RoomViewModel vm))
        {
            return vm.BuildType == BuildType.Room;
        }

        return false;
    }

    private bool IsNearRoomCorner(Vector2Int pos, Dictionary<Vector2Int, RoomViewModel> room)
    {
        foreach (Vector2Int dir in _diagonalDirs)
        {
            Vector2Int checkPos = pos + dir;

            if (IsRoom(checkPos, room))
            {
                bool nearRoomX = IsRoom(new Vector2Int(pos.x + dir.x, pos.y), room);
                bool nearRoomY = IsRoom(new Vector2Int(pos.x, pos.y + dir.y), room);

                if (!nearRoomX && !nearRoomY)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private int CalculateDistance(Vector2Int a, Vector2Int b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)) * 100;
    }

    // 저장 관련
    public void SaveBuildData()
    {
        // TODO : 방, 복도 배치 저장
        // _buildVM.Builds 순회 / OriginPos, BuildType, InstanceID 저장
    }

    public void LoadBuildData()
    {
        // TODO : 방, 복도 배치 로드
        // 저장된 데이터를 BuildViewModel.Builds에 추가 & SpawnBuildPrefab으로 맵 생성
        // 문 연결 계산 (UpdateRoomConnection & UpdateConnection)
    }
}