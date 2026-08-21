using Cysharp.Threading.Tasks;
using MySqlConnector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCollectionService
{
    private CollectionViewModel _collectionViewModel;

    public CollectionViewModel GetCollectionViewModel()
    {
        if(_collectionViewModel == null)
        {
            var collectionViewModel = new CollectionViewModel();
            SetCollectionViewModel(collectionViewModel);
            _collectionViewModel = collectionViewModel;
        }

        return _collectionViewModel;
    }

    private void SetCollectionViewModel(CollectionViewModel vm)
    {
        GameDataManager.Instance.LoadData<HamsterData>();
        GameDataManager.Instance.LoadData<FaceData>();
        LoadHamsterId(vm);
    }

    private void LoadHamsterId(CollectionViewModel vm)
    {
        var allHamsterIds = GameDataManager.Instance.GetAllDataId<HamsterData>();
        vm.AllHamsterIdList = allHamsterIds;

        var allFaceIds = GameDataManager.Instance.GetAllDataId<FaceData>();
        vm.AllFaceIdList = allFaceIds;
    }

    public async UniTask<List<HamsterSave>> LoadHamsterCollectionData(long userUID)
    {
        List<HamsterSave> hamsterList = new List<HamsterSave>();

        using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
        {
            try
            {
                await conn.OpenAsync();

                string query = $"SELECT * FROM {DBConfig.HamsterTable} WHERE Owner_User_UID = @userUID";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userUID", userUID);

                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            HamsterSave hamster = new HamsterSave
                            {
                                HamsterUID = reader.GetInt64("Hamster_UID"),
                                UserUID = reader.GetInt64("Owner_User_UID"),
                                HamsterId = reader.GetString("Hamster_Data_ID"),
                                FaceId = reader.GetString("Face_Data_ID")
                            };

                            // 2. 리스트에 추가
                            hamsterList.Add(hamster);

                            Debug.Log($"Hamster Data Load : {hamster.HamsterId}, {hamster.FaceId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        return hamsterList;
    }

    public async UniTask TrySaveHamsterData(HamsterSave hamsterSave)
    {
        using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
        {
            try
            {
                await conn.OpenAsync();

                string query = $"INSERT INTO {DBConfig.HamsterTable} (Hamster_UID, Owner_User_UID, Hamster_Data_ID, Face_Data_ID)" +
                               $"VALUES (@hamsterUID, @userUID, @hamsterId, @faceId)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@hamsterUID", hamsterSave.HamsterUID);
                    cmd.Parameters.AddWithValue("@userUID", hamsterSave.UserUID);
                    cmd.Parameters.AddWithValue("@hamsterId", hamsterSave.HamsterId);
                    cmd.Parameters.AddWithValue("@faceId", hamsterSave.FaceId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if(rowsAffected < 0)
                    {
                        Debug.Log("햄스터 데이터 저장 성공");
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }
}
