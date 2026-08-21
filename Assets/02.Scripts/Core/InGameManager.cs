// 게임 내 데이터 중앙 관리 시스템 - 방치 보상 등 게임 내 데이터를 관리하는 매니저
using UnityEngine;

public class InGameManager : SingletonBase<InGameManager>
{
    private const float IdleRewardCapSeconds = 12f * 60f * 60f;
    private const float TempProductionPerSec = 1f; // TODO: HamsterManager 실제값으로 교체

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

        int idleReward = GameUtil.CalculateIdleReward(loginVm.LastLoginTime.Ticks, TempProductionPerSec, IdleRewardCapSeconds);

        if (idleReward > 0)
        {
            UserViewModel userVm = ServiceManager.Instance.UserService.GetUserViewModel();
            userVm.AddSeed(idleReward);

#if UNITY_EDITOR
            Debug.Log($"[방치 보상] +{idleReward}");
#endif
        }

        loginVm.RequestUpdateLastLogin();
    }
}