using System;
using UnityEngine;
using MySqlConnector;
using Cysharp.Threading.Tasks;

public class LoginService
{
    public async UniTask<bool> TryLoginAsync(string userId, string password)
    {
        bool isSuccess = false;

        if (userId != "" && password != "")
        {
            using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = $"SELECT COUNT(*) FROM {DBConfig.GameUserTable} WHERE userId = @userId AND userPassword = @password;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@password", password);

                        object result = await cmd.ExecuteScalarAsync();
                        int count = Convert.ToInt32(result);

                        if (count > 0)
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

    public async UniTask<bool> CreateAccountAsync(string userId, string password)
    {
        bool isSuccess = false;

        if (userId != "" && password != "")
        {
            using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string checkQuery = $"SELECT COUNT(*) FROM {DBConfig.GameUserTable} WHERE userId = @userId;";
                    bool isExist = false;

                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@userId", userId);
                        object result = await checkCmd.ExecuteScalarAsync();
                        int count = Convert.ToInt32(result);

                        if (count > 0)
                        {
                            isExist = true;
                        }
                    }

                    if (isExist == false)
                    {
                        string insertQuery = $"INSERT INTO {DBConfig.GameUserTable} (userId, userPassword, userName, userTotalExp) VALUES (@userId, @password, @userName, 0);";

                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@userId", userId);
                            insertCmd.Parameters.AddWithValue("@password", password);
                            insertCmd.Parameters.AddWithValue("@userName", "기본이름");

                            int rowsAffected = await insertCmd.ExecuteNonQueryAsync();

                            if (rowsAffected > 0)
                            {
                                isSuccess = true;
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

        return isSuccess;
    }
}