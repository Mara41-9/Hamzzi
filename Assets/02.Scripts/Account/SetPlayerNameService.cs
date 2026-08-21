using System;
using UnityEngine;
using MySqlConnector;
using Cysharp.Threading.Tasks;

public class SetPlayerNameService
{
    public async UniTask<bool> TrySetPlayerNameAsync(string userId, string newName)
    {
        bool isSuccess = false;

        if (userId != "" && newName != "")
        {
            using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = $"UPDATE {DBConfig.UserGameTable} SET userName = @userName WHERE userId = @userId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userName", newName);
                        cmd.Parameters.AddWithValue("@userId", userId);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            isSuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        return isSuccess;
    }
}