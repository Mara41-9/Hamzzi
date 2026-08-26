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

                            BuildType buildType = (roomIndex == 2) ? BuildType.Aisle : BuildType.Room;
                            bool isDefault = (roomIndex == 0);

                            Vector2Int pos = new Vector2Int(posX, posY);
                            RoomViewModel roomVM = new RoomViewModel(buildType, pos)
                            {
                                InstanceID = roomUID.ToString(),
                                IsReady = true,
                                IsDefault = isDefault
                            };

                            int sizeX = (buildType == BuildType.Room) ? 10 : 2;
                            int sizeY = (buildType == BuildType.Room) ? 6 : 2;

                            for (int x = 0; x < sizeX; x++)
                            {
                                for (int y = 0; y < sizeY; y++)
                                {
                                    buildVM.Builds[pos + new Vector2Int(x, y)] = roomVM;
                                }
                            }

                            buildVM.LastBuild = roomVM;
                        }
                    }
                }

                string furnitureQuery = $@"SELECT furniture.* FROM {DBConfig.FurnitureTable} furniture JOIN {DBConfig.RoomTable} room ON furniture.Room_UID = room.Room_UID WHERE room.Owner_User_UID = @userUID";

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
                            Debug.Log($"가구 ID: {furnitureDataId} / 할당된 햄스터 UID: {hamsterUID}");

                            var itemData = GameDataManager.Instance.GetData<ItemData>(furnitureDataId);

                            if (itemData == null)
                            {
                                continue;
                            }

                            FurnitureViewModel furnitureVM = new FurnitureViewModel(furnitureUID.ToString(), itemData.Id, itemData.PrefabPath, new Vector2Int(posX, posY), Vector2Int.one)
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

                foreach (var pair in buildVM.Builds)
                {
                    Vector2Int pos = pair.Key;
                    RoomViewModel vm = pair.Value;

                    if (vm.BuildType == BuildType.Room)
                    {
                        buildVM.UpdateRoomConnection(vm);
                    }
                    else
                    {
                        buildVM.UpdateConnection(pos);
                    }
                }

                ServiceManager.Instance.BuildService.RefreshAisleNavMesh(buildVM.Builds);
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
            await conn.OpenAsync();
            using (MySqlTransaction transaction = await conn.BeginTransactionAsync())
            {
                try
                {
                    string deleteFurniture = $@"DELETE furniture FROM {DBConfig.FurnitureTable} furniture JOIN {DBConfig.RoomTable} room ON furniture.Room_UID = room.Room_UID WHERE room.Owner_User_UID = @userUID";
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

                    HashSet<RoomViewModel> uniqueBuilds = new HashSet<RoomViewModel>(buildVM.Builds.Values);

                    foreach (var build in uniqueBuilds)
                    {
                        long uid = 0;
                        long.TryParse(build.InstanceID, out uid);

                        if (uid == 0)
                        {
                            uid = GameUtil.GenerateUID();
                            build.InstanceID = uid.ToString();
                        }

                        int roomIndexValue = 1;

                        if (build.BuildType == BuildType.Aisle)
                        {
                            roomIndexValue = 2;
                        }
                        else if (build.IsDefault)
                        {
                            roomIndexValue = 0;
                        }

                        string insertQuery = $@"INSERT INTO {DBConfig.RoomTable} (Room_UID, Owner_User_UID, Room_Index, Position_X, Position_Y) 
                           VALUES (@roomUID, @userUID, @roomIndex, @roomPosX, @roomPosY)";

                        using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@roomUID", uid);
                            cmd.Parameters.AddWithValue("@userUID", userUID);
                            cmd.Parameters.AddWithValue("@roomIndex", roomIndexValue);
                            cmd.Parameters.AddWithValue("@roomPosX", build.OriginPos.x);
                            cmd.Parameters.AddWithValue("@roomPosY", build.OriginPos.y);

                            await cmd.ExecuteNonQueryAsync();
                        }

                        if (build.FurnitureList != null && build.FurnitureList.Count > 0)
                        {
                            foreach (var furnitureVM in build.FurnitureList)
                            {
                                long furnitureUID = 0;
                                long.TryParse(furnitureVM.InstanceID, out furnitureUID);

                                if (furnitureUID == 0)
                                {
                                    furnitureUID = GameUtil.GenerateUID();
                                    furnitureVM.InstanceID = furnitureUID.ToString();
                                }

                                string insertFurniture = $@"INSERT INTO {DBConfig.FurnitureTable} 
                                                            (Furniture_UID, Room_UID, Furniture_Data_ID, Position_X, Position_Y, Rotate_State, Useing_Hamster_UID)
                                                            VALUES (@furnitureUID, @roomUID, @furnitureDataId, @posX, @posY, @rotate, @hamsterUID)";

                                using (MySqlCommand fCmd = new MySqlCommand(insertFurniture, conn, transaction))
                                {
                                    fCmd.Parameters.AddWithValue("@furnitureUID", furnitureUID);
                                    fCmd.Parameters.AddWithValue("@roomUID", uid);
                                    fCmd.Parameters.AddWithValue("@furnitureDataId", furnitureVM.FurnitureID);
                                    fCmd.Parameters.AddWithValue("@posX", furnitureVM.LocalPos.x);
                                    fCmd.Parameters.AddWithValue("@posY", furnitureVM.LocalPos.y);
                                    fCmd.Parameters.AddWithValue("@rotate", furnitureVM.RotationAngle);

                                    long parsedHamsterUID = 0;
                                    object hamsterVal = DBNull.Value;

                                    if (!string.IsNullOrEmpty(furnitureVM.AssignHamsterID) && long.TryParse(furnitureVM.AssignHamsterID, out parsedHamsterUID))
                                    {
                                        hamsterVal = parsedHamsterUID;
                                    }

                                    fCmd.Parameters.AddWithValue("@hamsterUID", hamsterVal);

                                    await fCmd.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }

                    await transaction.CommitAsync();
                    Debug.Log("건설 및 가구 저장 성공");
                }
                catch (Exception ex)
                {
                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        Debug.LogError($"트랜잭션 롤백 실패 : {rollbackEx.Message}");
                    }

                    Debug.LogError($"저장 오류 : {ex.Message}");
                }
            }
        }
    }

    private async UniTaskVoid SpawnLoadFurniture(RoomViewModel roomVM, FurnitureViewModel furnitureVM)
    {
        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync(furnitureVM.InstanceID.ToString(), furnitureVM.PrefabPath, Vector3.zero);
        prefab.transform.rotation = Quaternion.identity;

        float subCellSize = 1.0f / roomVM.GridFactor;

        if (prefab.TryGetComponent(out FurnitureView furnitureView))
        {
            furnitureVM.Size = furnitureView.GetFurnitureSize(subCellSize);
        }

        bool isRotated = (furnitureVM.RotationAngle / 90) % 2 != 0;
        int sizeX = isRotated ? furnitureVM.Size.y : furnitureVM.Size.x;
        int sizeY = isRotated ? furnitureVM.Size.x : furnitureVM.Size.y;

        float localX = (furnitureVM.LocalPos.x + sizeX * 0.5f) * subCellSize;
        float localZ = (furnitureVM.LocalPos.y + sizeY * 0.5f) * subCellSize;

        Vector3 spawnPos = new Vector3((roomVM.OriginPos.x * 1.0f) + localX, (roomVM.OriginPos.y + 2.0f) * 1.0f + 0.2f, 9f - localZ - 0.5f);

        Quaternion spawnRot = Quaternion.Euler(0f, furnitureVM.RotationAngle, 0f);

        prefab.transform.SetPositionAndRotation(spawnPos, spawnRot);

        if (furnitureView != null)
        {
            furnitureView.ResetMaterial();
            furnitureView.Bind(furnitureVM);
        }

        ServiceManager.Instance.HousingService.RegisterSpawnFurniture(furnitureVM.InstanceID, prefab);
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

    public void RequestSaveHousingData()
    {
        long userUID = 0;

        var loginVm = ServiceManager.Instance.LoginService?.GetViewModel();
        if (loginVm != null)
        {
            userUID = loginVm.UserUID;
        }

        if (userUID != 0)
        {
            ServiceManager.Instance.NetworkBuildService.SaveAllBuildAndFurnitureData(userUID).Forget();

            Debug.Log("건설/가구/인벤토리 데이터 저장 요청 완료");
        }
    }
}