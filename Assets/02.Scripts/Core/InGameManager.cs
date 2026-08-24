// 게임 내 데이터 중앙 관리 시스템 - 방치 보상 등 게임 내 데이터를 관리하는 매니저
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEditor.Overlays;
using UnityEngine;

public class InGameManager : SingletonBase<InGameManager>
{
    private const float IdleRewardCapSeconds = 12f * 60f * 60f;
    private const float IdleRewardRateMultiplier = 0.7f;

    private int _pendingIdleReward;

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
        long lastLoginTicks = loginVm.LastLoginTime.Ticks;
        float elapsedSeconds = GameUtil.CalculateElapsedSeconds(lastLoginTicks, IdleRewardCapSeconds);
        int idleReward = GameUtil.CalculateIdleReward(lastLoginTicks, productionPerSec, IdleRewardCapSeconds);

        loginVm.RequestUpdateLastLogin();
        AutoSaveSeedCount(this.GetCancellationTokenOnDestroy()).Forget();

        if (idleReward <= 0)
        {
            return;
        }

        _pendingIdleReward = idleReward;
        UIManager.Instance.OpenIdleRewardPopupUI(idleReward, elapsedSeconds, IdleRewardCapSeconds);
    }

    public void ClaimIdleReward()
    {
        if (_pendingIdleReward <= 0)
        {
            return;
        }

        UserViewModel userVm = ServiceManager.Instance.UserService.GetUserViewModel();

#if UNITY_EDITOR
        Debug.Log($"[방치 보상 수령] +{_pendingIdleReward}");
#endif

        userVm.AddSeed(_pendingIdleReward);
        _pendingIdleReward = 0;

        UIManager.Instance.CloseIdleRewardPopupUI();
        loginVm.RequestUpdateLastLogin();
        AutoSaveGameData(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask AutoSaveGameData(CancellationToken token)
    {
        while(true)
        {
            await UniTask.Delay(TimeSpan.FromMinutes(5), cancellationToken: token);
            await SaveSeedCount();
            await SaveInventory();
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

    private async UniTask SaveInventory()
    {
        var loginVm = ServiceManager.Instance.LoginService.GetViewModel();

        await ServiceManager.Instance.HousingService.SaveAllInventoryData(loginVm.UserUID);
    }

    private void OnApplicationQuit()
    {
        SaveSeedCount().Forget();
        SaveInventory().Forget();
    }
}