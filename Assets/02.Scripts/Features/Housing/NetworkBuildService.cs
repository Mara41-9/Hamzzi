using Cysharp.Threading.Tasks;
using MySqlConnector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBuildService
{
    private BuildViewModel _buildVM;

    public BuildViewModel GetBuildViewModel()
    {
        if (_buildVM == null)
        {
            _buildVM = ServiceManager.Instance.BuildService.GetBuildViewModel();
        }

        return _buildVM;
    }

    public async UniTask LoadBuildAndFurnitureData(long userUID)
    {
        var buildVM = GetBuildViewModel();
        buildVM.IsLoading = true;

        using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
        {
            try
            {
                await conn.OpenAsync();

                string roomQuery = $"SELECT Room_UID, Room_Index, Position_X, Position_Y FROM {DBConfig.RoomTable} WHERE Owner_User_UID = @userUID GROUP BY Room_UID";
                using (MySqlCommand cmd = new MySqlCommand(roomQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@userUID", userUID);
                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long roomUID = reader.GetInt64("Room_UID");
                            int roomIndex = reader.GetInt32("Room_Index");
                            int posX = reader.GetInt32("Position_X");
                            int posY = reader.GetInt32("Position_Y");

                            Vector2Int pos = new Vector2Int(posX, posY);
                            RoomViewModel roomVM = new RoomViewModel(BuildType.Room, pos)
                            {
                                InstanceID = roomUID.ToString(),
                                IsReady = true,
                                IsDefault = (roomIndex == 0)
                            };

                            for (int x = 0; x < roomVM.Size.x; x++)
                            {
                                for (int y = 0; y < roomVM.Size.y; y++)
                                {
                                    buildVM.Builds[pos + new Vector2Int(x, y)] = roomVM;
                                }
                            }

                            SpawnLoadPrefab(roomVM).Forget();
                        }
                    }
                }

                string furnitureQuery = $@"SELECT furniture.*, room.Room_UID FROM {DBConfig.FurnitureTable} furniture JOIN {DBConfig.RoomTable}
                                        room ON furniture.Furniture_UID = room.Furniture_UID WHERE room.Owner_User_UID = @userUID AND furniture.Furniture_UID != 0";

                using (MySqlCommand cmd = new MySqlCommand(furnitureQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@userUID", userUID);

                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long roomUID = reader.GetInt64("Room_UID");
                            long furnitureUID = reader.GetInt64("Furniture_UID");
                            string furnitureDataId = reader.GetString("Furniture_Data_ID");
                            int posX = reader.GetInt32("Position_X");
                            int posY = reader.GetInt32("Position_Y");
                            int rotateState = reader.GetInt32("Rotate_State");
                            long? hamsterUID = reader.IsDBNull(reader.GetOrdinal("Useing_Hamster_UID")) ? (long?)null : reader.GetInt64("Useing_Hamster_UID");

                            var itemData = GameDataManager.Instance.GetData<ItemData>(furnitureDataId);

                            if (itemData == null)
                            {
                                continue;
                            }

                            FurnitureViewModel furnitureVM = new FurnitureViewModel(itemData.Id, itemData.PrefabPath, new Vector2Int(posX, posY), Vector2Int.one)
                            {
                                RotationAngle = rotateState,
                                AssignHamsterID = hamsterUID?.ToString()
                            };

                            foreach (var room in buildVM.Builds.Values)
                            {
                                if (room.InstanceID == roomUID.ToString())
                                {
                                    room.AddFurniture(furnitureVM);
                                    SpawnLoadFurniture(room, furnitureVM).Forget();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"건설 및 가구 데이터 로드 오류 : {ex.Message}");
            }
        }

        buildVM.IsLoading = false;
        Debug.Log("건설 및 가구 데이터 로드 완료");
    }

    public async UniTask SaveAllBuildAndFurnitureData(long userUID)
    {
        if (userUID == 0)
        {
            return;
        }

        var buildVM = GetBuildViewModel();

        using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
        {
            try
            {
                await conn.OpenAsync();
                using (MySqlTransaction transaction = await conn.BeginTransactionAsync())
                {
                    string deleteFurniture = $@"DELETE furniture FROM {DBConfig.FurnitureTable} furniture JOIN {DBConfig.RoomTable} room ON furniture.Furniture_UID = room.Furniture_UID WHERE room.Owner_User_UID = @userUID";
                    
                    using (MySqlCommand cmd = new MySqlCommand(deleteFurniture, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@userUID", userUID);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    string deleteRoom = $"DELETE FROM {DBConfig.RoomTable} WHERE Owner_User_UID = @userUID";
                    
                    using (MySqlCommand cmd = new MySqlCommand(deleteRoom, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@userUID", userUID);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    HashSet<RoomViewModel> uniqueRooms = new HashSet<RoomViewModel>(buildVM.Builds.Values);

                    foreach (var room in uniqueRooms)
                    {
                        long roomUID = 0;
                        long.TryParse(room.InstanceID, out roomUID);

                        if (roomUID == 0)
                        {
                            roomUID = GameUtil.GenerateUID();
                        }

                        if (room.FurnitureList == null || room.FurnitureList.Count == 0)
                        {
                            string insertRoomOnly = $@"INSERT INTO {DBConfig.RoomTable} (Room_UID, Owner_User_UID, Room_Index, Furniture_UID, Position_X, Position_Y) VALUES (@roomUID, @userUID, @roomIndex, 0, @roomPosX, @roomPosY)";

                            using (MySqlCommand cmd = new MySqlCommand(insertRoomOnly, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@roomUID", roomUID);
                                cmd.Parameters.AddWithValue("@userUID", userUID);
                                cmd.Parameters.AddWithValue("@roomIndex", room.IsDefault ? 0 : 1);
                                cmd.Parameters.AddWithValue("@roomPosX", room.OriginPos.x);
                                cmd.Parameters.AddWithValue("@roomPosY", room.OriginPos.y);

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        else
                        {
                            foreach (var furnitureVM in room.FurnitureList)
                            {
                                long furnitureUID = GameUtil.GenerateUID();

                                string insertF = $@"INSERT INTO {DBConfig.FurnitureTable} (Furniture_UID, Furniture_Data_ID, Furniture_Level, Position_X, Position_Y, Rotate_State, Useing_Hamster_UID)
                                                     VALUES (@furnitureUID, @furnitureDataId, @level, @posX, @posY, @rotate, @hamsterUID)";

                                using (MySqlCommand fCmd = new MySqlCommand(insertF, conn, transaction))
                                {
                                    fCmd.Parameters.AddWithValue("@furnitureUID", furnitureUID);
                                    fCmd.Parameters.AddWithValue("@furnitureDataId", furnitureVM.FurnitureID);
                                    fCmd.Parameters.AddWithValue("@level", 1);
                                    fCmd.Parameters.AddWithValue("@posX", furnitureVM.LocalPos.x);
                                    fCmd.Parameters.AddWithValue("@posY", furnitureVM.LocalPos.y);
                                    fCmd.Parameters.AddWithValue("@rotate", furnitureVM.RotationAngle);

                                    object hamsterVal = string.IsNullOrEmpty(furnitureVM.AssignHamsterID) ? DBNull.Value : (object)long.Parse(furnitureVM.AssignHamsterID);
                                    fCmd.Parameters.AddWithValue("@hamsterUID", hamsterVal);

                                    await fCmd.ExecuteNonQueryAsync();
                                }

                                string insertR = $@"INSERT INTO {DBConfig.RoomTable} (Room_UID, Owner_User_UID, Room_Index, Furniture_UID, Position_X, Position_Y) VALUES (@roomUID, @userUID, @roomIndex, @furnitureUID, @roomPosX, @roomPosY)";

                                using (MySqlCommand rCmd = new MySqlCommand(insertR, conn, transaction))
                                {
                                    rCmd.Parameters.AddWithValue("@roomUID", roomUID);
                                    rCmd.Parameters.AddWithValue("@userUID", userUID);
                                    rCmd.Parameters.AddWithValue("@roomIndex", room.IsDefault ? 0 : 1);
                                    rCmd.Parameters.AddWithValue("@furnitureUID", furnitureUID);
                                    rCmd.Parameters.AddWithValue("@roomPosX", room.OriginPos.x);
                                    rCmd.Parameters.AddWithValue("@roomPosY", room.OriginPos.y);

                                    await rCmd.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }

                    await transaction.CommitAsync();

                    Debug.Log("건설 및 가구 저장 성공");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"저장 오류 : {ex.Message}");
            }
        }
    }

    private async UniTaskVoid SpawnLoadPrefab(RoomViewModel roomVM)
    {
        float worldX = roomVM.OriginPos.x + (roomVM.Size.x * 0.5f);
        float worldY = roomVM.OriginPos.y + (roomVM.BuildType == BuildType.Room ? 2f : 0f);
        Vector3 worldPos = new Vector3(worldX, worldY, 9f);
        string path = roomVM.BuildType == BuildType.Room ? "Prefabs/Room" : "Prefabs/Aisle";
        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync(roomVM.InstanceID, path, worldPos);

        if (prefab.TryGetComponent(out Room room))
        {
            room.Bind(roomVM);
        }
        else if (prefab.TryGetComponent(out Aisle aisle))
        {
            aisle.Bind(roomVM);
        }
    }

    private async UniTaskVoid SpawnLoadFurniture(RoomViewModel roomVM, FurnitureViewModel furnitureVM)
    {
        float subCellSize = 1.0f / roomVM.GridFactor;
        float localX = (furnitureVM.LocalPos.x + furnitureVM.Size.x * 0.5f) * subCellSize;
        float localZ = (furnitureVM.LocalPos.y + furnitureVM.Size.y * 0.5f) * subCellSize;

        float worldX = (roomVM.OriginPos.x * 1.0f) + localX;
        float worldY = (roomVM.OriginPos.y + 2.0f) * 1.0f + 0.2f;
        float worldZ = 9f - localZ - 0.5f;

        Vector3 spawnPos = new Vector3(worldX, worldY, worldZ);
        Quaternion spawnRot = Quaternion.Euler(0f, furnitureVM.RotationAngle, 0f);

        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync(furnitureVM.InstanceID, furnitureVM.PrefabPath, spawnPos);

        if (prefab != null)
        {
            prefab.transform.rotation = spawnRot;

            if (prefab.TryGetComponent(out FurnitureView furnitureView))
            {
                furnitureView.ResetMaterial();
                furnitureView.Bind(furnitureVM);
            }
        }
    }

    public async UniTask<bool> HasUserRoomData(long userUID)
    {
        if (userUID == 0)
        {
            return false;
        }

        using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
        {
            try
            {
                await conn.OpenAsync();

                string query = $"SELECT COUNT(*) FROM {DBConfig.RoomTable} WHERE Owner_User_UID = @userUID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userUID", userUID);
                    long count = (long)(await cmd.ExecuteScalarAsync());

                    return count > 0;

                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"저장 데이터 확인 오류 : {ex.Message}");
                return false;
            }
        }
    }
}