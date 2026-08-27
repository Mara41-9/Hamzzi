// 게임 내 데이터 중앙 관리 시스템 - 방치 보상 등 게임 내 데이터를 관리하는 매니저
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class InGameManager : SingletonBase<InGameManager>
{
    private const float IdleRewardCapSeconds = 12f * 60f * 60f;
    private const float IdleRewardRateMultiplier = 0.7f;
    private const float PopupCloseDelaySeconds = 0.3f;

    private int _pendingIdleReward;
    private long _lastLoginTicks;


    private void Start()
    {
        LoginViewModel loginVm = ServiceManager.Instance.LoginService.GetViewModel();
        loginVm.OnCompleteLogin += HandleCompleteLogin;

        ServiceManager.Instance.UserService.OnUserDataLoaded += HandleUserDataLoaded;
    }

    private void OnDestroy()
    {
        LoginViewModel loginVm = ServiceManager.Instance.LoginService.GetViewModel();
        if (loginVm != null)
        {
            loginVm.OnCompleteLogin -= HandleCompleteLogin;
        }

        UserService userService = ServiceManager.Instance.UserService;
        if (userService != null)
        {
            userService.OnUserDataLoaded -= HandleUserDataLoaded;
        }
    }

    private void HandleCompleteLogin()
    {
        LoginViewModel loginVm = ServiceManager.Instance.LoginService.GetViewModel();
        if (loginVm != null && loginVm.UserUID != 0)
        {
            GameManager.Instance.InitMap(loginVm.UserUID).Forget();
        }


        _lastLoginTicks = loginVm.LastLoginTime.Ticks;

        loginVm.RequestUpdateLastLogin();
        AutoSaveGameData(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private void HandleUserDataLoaded()
    {
        if (_lastLoginTicks == 0)
        {
            return;
        }

        UserViewModel userVm = ServiceManager.Instance.UserService.GetUserViewModel();

        float productionPerSec = userVm.GoldPerSec * IdleRewardRateMultiplier;
        float elapsedSeconds = GameUtil.CalculateElapsedSeconds(_lastLoginTicks, IdleRewardCapSeconds);
        int idleReward = GameUtil.CalculateIdleReward(_lastLoginTicks, productionPerSec, IdleRewardCapSeconds);

#if UNITY_EDITOR
        Debug.Log($"[방치 보상 계산] 저장된 초당 {userVm.GoldPerSec} / 적용 초당 {productionPerSec} / 경과 {elapsedSeconds}초 / 보상 {idleReward}");
#endif

        if (idleReward <= 0)
        {
            return;
        }

        _pendingIdleReward = idleReward;

        float buffRate = userVm.GetSeedBuffRate();

        UIManager.Instance.OpenIdleRewardPopupUI(idleReward, elapsedSeconds, IdleRewardCapSeconds, buffRate);
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

        userVm.AddSeedWithoutBuff(_pendingIdleReward);
        _pendingIdleReward = 0;

        CloseIdleRewardPopupDelayed(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask CloseIdleRewardPopupDelayed(CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(PopupCloseDelaySeconds), cancellationToken: token);

        UIManager.Instance.CloseIdleRewardPopupUI();
    }

    private async UniTask AutoSaveGameData(CancellationToken token)
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
            GoldCount = userVm.SeedCount,
            GoldPerSec = CalculateCurrentGoldPerSec()
        };

        await ServiceManager.Instance.UserService.SaveUserAsync(loginVm.UserUID, saveData);
    }

    private float CalculateCurrentGoldPerSec()
    {
        UserViewModel userVm = ServiceManager.Instance.UserService.GetUserViewModel();
        float buffRate = userVm.GetSeedBuffRate();

        return HamsterManager.Instance.TotalCollectSpeedPerSec * (1f + buffRate);
    }

    private void OnApplicationQuit()
    {
        SaveSeedCount().Forget();
    }
}