using Cysharp.Threading.Tasks;
using MySqlConnector;
using System;
using System.IO;
using System.Threading.Tasks;
using Unity.AppUI.Redux;
using UnityEngine;

public class UserService
{
    private UserViewModel _userViewModel;

    public UserViewModel GetUserViewModel()
    {
        if(_userViewModel == null)
        {
            CreateUserViewModel();
        }

        return _userViewModel;
    }

    private UserViewModel CreateUserViewModel()
    {
        var userVm = new UserViewModel();
        _userViewModel = userVm;

        return userVm;
    }

    public async UniTask<UserData> GetUserAsync(string userId)
    {
        UserData resultUserData = null;

        if(userId != "")
        {
            using(MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = $"SELECT User_Name, User_Icon_Data_ID, Gold_Count FROM User_Account AS account INNER JOIN User_Game_Data AS game ON account.User_UID = game.User_UID WHERE account.User_Id = @userId";

                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using(MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if(reader.Read())
                            {
                                resultUserData = new UserData();
                                resultUserData.UserName = reader.GetString(0);
                                resultUserData.UserIconId = reader.GetString(1);
                                resultUserData.GoldCount = reader.GetInt32(2);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        return resultUserData;
    }

    public async UniTask InitUser(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("[InitUser] userId가 비어있음");
            return;
        }

        var userData = await GetUserAsync(userId);
        if (userData == null)
        {
            Debug.LogError($"[InitUser] 유저 데이터 조회 실패 / User_Id: {userId}");
            return;
        }

        var userVm = GetUserViewModel();

        userVm.UserName = userData.UserName;
        userVm.UserIconId = userData.UserIconId;
        userVm.SeedCount = userData.GoldCount;
    }
}
