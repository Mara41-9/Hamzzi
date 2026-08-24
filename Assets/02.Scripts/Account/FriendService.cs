using System;
using UnityEngine;
using MySqlConnector;
using Cysharp.Threading.Tasks;

public class FriendService
{
    public async UniTask<bool> TryAddFriendAsync(long myUid, long targetUid)
    {
        bool isSuccess = false;

        if (myUid == 0 || targetUid == 0) return isSuccess;

        using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
        {
            try
            {
                await conn.OpenAsync();

                long dataUid = GameUtil.GenerateUID();

                string query = $"INSERT INTO {DBConfig.FriendTable} (Friend_Data_UID, Owner_User_UID, Friend_User_UID, Is_Send, Is_Accept) VALUES (@dataUid, @myUid, @targetUid, 1, 0);";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@dataUid", dataUid);
                    cmd.Parameters.AddWithValue("@myUid", myUid);
                    cmd.Parameters.AddWithValue("@targetUid", targetUid);

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

        return isSuccess;
    }
}