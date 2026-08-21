using System;
using UnityEngine;
using MySqlConnector;
using Cysharp.Threading.Tasks;

public class LoginService
{
    private LoginViewModel _viewModel;

    public LoginService()
    {
        _viewModel = new LoginViewModel();
        _viewModel.SetLoginService(this);
    }

    public LoginViewModel GetViewModel()
    {
        return _viewModel;
    }

    public async UniTask<long> TryLoginAsync(string userId, string password)
    {
        long resultUid = 0;

        if (userId != "" && password != "")
        {
            using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "SELECT User_UID FROM User_Account WHERE User_Id = @userId AND User_Password = @password;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@password", password);

                        object result = await cmd.ExecuteScalarAsync();

                        if (result != null)
                        {
                            resultUid = Convert.ToInt64(result);
                        }
                    }

                    if (resultUid != 0)
                    {
                        string updateQuery = "UPDATE User_Account SET Last_Login = @lastLogin WHERE User_UID = @uid;";

                        using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@lastLogin", DateTime.UtcNow);
                            updateCmd.Parameters.AddWithValue("@uid", resultUid);

                            await updateCmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        return resultUid;
    }

    public async UniTask<long> CreateAccountAsync(string userId, string password)
    {
        long newUserUid = 0;

        if (userId != "" && password != "")
        {
            using (MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string checkQuery = "SELECT COUNT(*) FROM User_Account WHERE User_Id = @userId;";
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
                        long generatedUid = GameUtil.GenerateUID();

                        string insertAccountQuery = "INSERT INTO User_Account (User_UID, User_Id, User_Password, Last_Login) VALUES (@uid, @userId, @password, @lastLogin);";

                        using (MySqlCommand insertAccountCmd = new MySqlCommand(insertAccountQuery, conn))
                        {
                            insertAccountCmd.Parameters.AddWithValue("@uid", generatedUid);
                            insertAccountCmd.Parameters.AddWithValue("@userId", userId);
                            insertAccountCmd.Parameters.AddWithValue("@password", password);
                            insertAccountCmd.Parameters.AddWithValue("@lastLogin", DateTime.UtcNow);

                            int accountRows = await insertAccountCmd.ExecuteNonQueryAsync();

                            if (accountRows > 0)
                            {
                                string insertGameDataQuery = "INSERT INTO User_Game_Data (User_UID, User_Name, User_Icon_Data_ID, Gold_Count) VALUES (@uid, @userName, @iconId, @gold);";

                                using (MySqlCommand insertGameDataCmd = new MySqlCommand(insertGameDataQuery, conn))
                                {
                                    insertGameDataCmd.Parameters.AddWithValue("@uid", generatedUid);
                                    insertGameDataCmd.Parameters.AddWithValue("@userName", "기본이름");
                                    insertGameDataCmd.Parameters.AddWithValue("@iconId", "default_icon");
                                    insertGameDataCmd.Parameters.AddWithValue("@gold", 0);

                                    int gameDataRows = await insertGameDataCmd.ExecuteNonQueryAsync();

                                    if (gameDataRows > 0)
                                    {
                                        newUserUid = generatedUid;
                                    }
                                }
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

        return newUserUid;
    }
}