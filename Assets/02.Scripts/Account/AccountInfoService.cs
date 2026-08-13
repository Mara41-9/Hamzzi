using System;
using UnityEngine;
using MySqlConnector;
using Cysharp.Threading.Tasks;

public class AccountInfoData
{
    public string UserId = "";
    public string UserName = "";
}

public class AccountInfoService
{
    public async UniTask<AccountInfoData> GetAccountInfoAsync(string userId)
    {
        AccountInfoData resultData = null;

        if (userId != "")
        {
            using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = $"SELECT userId, userName FROM {DBConfig.GameUserTable} WHERE userId = @userId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                resultData = new AccountInfoData();
                                resultData.UserId = reader.GetString(0);
                                resultData.UserName = reader.GetString(1);
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

        return resultData;
    }
}