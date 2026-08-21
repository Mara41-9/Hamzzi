using Cysharp.Threading.Tasks;
using MySqlConnector;
using System;
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

    public async UniTask<UserData> GetUserAsync(long userUid)
    {
        UserData resultUserData = null;

        if(userUid != 0)
        {
            using(MySqlConnection conn = new MySqlConnection(DBConfig.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = $"SELECT User_Name, User_Icon_Data_ID, Gold_Count FROM User_Game_Data WHERE User_UID = @userUid";

                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userUid", userUid);

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

    public async UniTask InitUser(long userUid)
    {
        if (userUid == 0)
        {
            Debug.LogError("[InitUser] userId가 비어있음");
            return;
        }

        var userData = await GetUserAsync(userUid);
        if (userData == null)
        {
            Debug.LogError($"[InitUser] 유저 데이터 조회 실패 / User_Uid: {userUid}");
            return;
        }

        var userVm = GetUserViewModel();

        userVm.UserName = userData.UserName;
        userVm.UserIconId = userData.UserIconId;
        userVm.SeedCount = userData.GoldCount;
    }
}
