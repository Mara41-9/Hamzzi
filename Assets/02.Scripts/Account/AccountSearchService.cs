using System;
using UnityEngine;
using MySqlConnector;
using Cysharp.Threading.Tasks;

public class AccountSearchService
{
    public async UniTask<bool> TrySearchAccountAsync(string userId)
    {
        bool isExist = false;

        if (userId != "")
        {
            using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = $"SELECT COUNT(*) FROM {DBConfig.GameUserTable} WHERE userId = @userId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        object result = await cmd.ExecuteScalarAsync();
                        int count = Convert.ToInt32(result);

                        if (count > 0)
                        {
                            isExist = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        return isExist;
    }
}