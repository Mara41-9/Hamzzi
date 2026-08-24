// 게임 내 데이터 중앙 관리 시스템 - 방치 보상 등 게임 내 데이터를 관리하는 매니저
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class InGameManager : SingletonBase<InGameManager>
{
    private const float IdleRewardCapSeconds = 12f * 60f * 60f;
    private const float IdleRewardRateMultiplier = 0.7f;

    private void Start()
    {
        LoginViewModel loginVm = ServiceManager.Instance.LoginService.GetViewModel();
        loginVm.OnCompleteLogin += HandleCompleteLogin;
    }

    private void OnDestroy()
    {
        LoginViewModel loginVm = ServiceManager.Instance.LoginService.GetViewModel();
        if (loginVm != null)
        {
            loginVm.OnCompleteLogin -= HandleCompleteLogin;
        }
    }

    private void HandleCompleteLogin()
    {
        LoginViewModel loginVm = ServiceManager.Instance.LoginService.GetViewModel();

        float productionPerSec = HamsterManager.Instance.TotalCollectSpeedPerSec * IdleRewardRateMultiplier;
        int idleReward = GameUtil.CalculateIdleReward(loginVm.LastLoginTime.Ticks, productionPerSec, IdleRewardCapSeconds);

        if (idleReward > 0)
        {
            UserViewModel userVm = ServiceManager.Instance.UserService.GetUserViewModel();
            userVm.AddSeed(idleReward);

#if UNITY_EDITOR
            Debug.Log($"[방치 보상] +{idleReward}");
#endif
        }

        loginVm.RequestUpdateLastLogin();
        AutoSaveSeedCount(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask AutoSaveSeedCount(CancellationToken token)
    {
        while(true)
        {
            await UniTask.Delay(TimeSpan.FromMinutes(5), cancellationToken: token);
            await SaveSeedCount();
        }
    }

    private async UniTask SaveSeedCount()
    {
        var loginVm = ServiceManager.Instance.LoginService.GetViewModel();

        var userVm = ServiceManager.Instance.UserService.GetUserViewModel();

        UserSaveData saveData = new UserSaveData
        {
            GoldCount = userVm.SeedCount
        };

        await ServiceManager.Instance.UserService.SaveUserAsync(loginVm.UserUID, saveData);
    }

    private void OnApplicationQuit()
    {
        SaveSeedCount().Forget();
    }
}