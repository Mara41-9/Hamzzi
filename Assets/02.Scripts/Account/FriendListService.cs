using System;
using System.Collections.Generic;
using UnityEngine;
using MySqlConnector;
using Cysharp.Threading.Tasks;

public class FriendInfoData
{
    public long FriendUid = 0;
    public string FriendName = "";
}

public class FriendListService
{
    private FriendListViewModel _viewModel;

    public FriendListService()
    {
        _viewModel = new FriendListViewModel();
        _viewModel.SetService(this);
    }

    public FriendListViewModel GetViewModel()
    {
        return _viewModel;
    }

    public async UniTask<List<FriendInfoData>> GetFriendListAsync(long myUserUid)
    {
        List<FriendInfoData> resultList = new List<FriendInfoData>();

        if (myUserUid != 0)
        {
            using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "SELECT f.Friend_User_UID, u.User_Name FROM Friend_Data f JOIN User_Game_Data u ON f.Friend_User_UID = u.User_UID WHERE f.Owner_User_UID = @uid AND f.Is_Accept = 1 ORDER BY u.User_Name ASC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", myUserUid);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                FriendInfoData data = new FriendInfoData();
                                data.FriendUid = reader.GetInt64(0);
                                data.FriendName = reader.GetString(1);

                                resultList.Add(data);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        return resultList;
    }
}