using System;
using UnityEngine;
using MySqlConnector;
using Cysharp.Threading.Tasks;

public class FriendService
{
    public async UniTask<bool> TryAddFriendAsync(string myUserId, string targetUserId)
    {
        bool isSuccess = false;

        if (myUserId != "" && targetUserId != "")
        {
            using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = $"INSERT INTO friend (userId, friendId) VALUES (@myUserId, @targetUserId);";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@myUserId", myUserId);
                        cmd.Parameters.AddWithValue("@targetUserId", targetUserId);

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