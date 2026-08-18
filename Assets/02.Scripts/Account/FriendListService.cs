using System;
using System.Collections.Generic;
using UnityEngine;
using MySqlConnector;
using Cysharp.Threading.Tasks;

public class FriendInfoData
{
    public string FriendId = "";
    public string FriendName = "";
}

public class FriendListService
{
    public async UniTask<List<FriendInfoData>> GetFriendListAsync(string myUserId)
    {
        List<FriendInfoData> resultList = new List<FriendInfoData>();

        if (myUserId != "")
        {
            using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = $"SELECT f.friendId, u.userName FROM {DBConfig.FriendTable} f JOIN {DBConfig.GameUserTable} u ON f.friendId = u.userId WHERE f.userId = @userId ORDER BY u.userName ASC;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", myUserId);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                FriendInfoData data = new FriendInfoData();
                                data.FriendId = reader.GetString(0);
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