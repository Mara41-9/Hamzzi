// 방치 보상 수령 팝업 UI. 오프라인 경과 시간과 보상 수량을 표시한다
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class IdleRewardPopupUI : UIBase
{
    private const int SecondsPerHour = 3600;
    private const int SecondsPerMinute = 60;
    private const int PercentScale = 100;

    [SerializeField] private TMP_Text Text_OfflineTime;
    [SerializeField] private TMP_Text Text_RewardCount;
    [SerializeField] private UIButton Button_GetReward;
    [SerializeField] private TMP_Text Text_Effect;
    [SerializeField] private GameObject Object_Loading;
    [SerializeField] private float _buttonUnlockDelaySeconds = 8f;

    private void OnEnable()
    {
        Button_GetReward.BindOnClickButtonEvent(OnClickGetReward);
    }

    public void SetRewardInfo(int rewardAmount, float elapsedSeconds, float capSeconds, float buffRate)
    {
        Text_RewardCount.text = rewardAmount.ToString();

        int capHours = Mathf.FloorToInt(capSeconds / SecondsPerHour);
        Text_OfflineTime.text = $"{BuildElapsedTimeText(elapsedSeconds)} (최대 {capHours}시간)";

        SetBuffText(buffRate);

        PlayLoadingUntilReady(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask PlayLoadingUntilReady(CancellationToken token)
    {
        Object_Loading.SetActive(true);
        Button_GetReward.SetInteractable(false);

        await UniTask.Delay(TimeSpan.FromSeconds(_buttonUnlockDelaySeconds), cancellationToken: token);

        Object_Loading.SetActive(false);
        Button_GetReward.SetInteractable(true);
    }

    private void SetBuffText(float buffRate)
    {
        if (buffRate <= 0f)
        {
            Text_Effect.text = "없음";
            return;
        }

        int buffPercent = Mathf.RoundToInt(buffRate * PercentScale);
        Text_Effect.text = $"가구 버프 +{buffPercent}%";
    }

    private string BuildElapsedTimeText(float elapsedSeconds)
    {
        int totalSeconds = Mathf.FloorToInt(elapsedSeconds);
        int hours = totalSeconds / SecondsPerHour;
        int minutes = (totalSeconds % SecondsPerHour) / SecondsPerMinute;
        int seconds = totalSeconds % SecondsPerMinute;

        return $"{hours}시간 {minutes}분 {seconds}초";
    }

    private void OnClickGetReward()
    {
        InGameManager.Instance.ClaimIdleReward();
    }
}