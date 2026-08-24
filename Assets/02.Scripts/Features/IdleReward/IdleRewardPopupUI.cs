// 방치 보상 수령 팝업 UI. 오프라인 경과 시간과 보상 수량을 표시한다
using TMPro;
using UnityEngine;

public class IdleRewardPopupUI : UIBase
{
    private const int SecondsPerHour = 3600;
    private const int SecondsPerMinute = 60;

    [SerializeField] private TMP_Text Text_OfflineTime;
    [SerializeField] private TMP_Text Text_RewardCount;
    [SerializeField] private UIButton Button_GetReward;

    private void OnEnable()
    {
        Button_GetReward.BindOnClickButtonEvent(OnClickGetReward);
    }

    public void SetRewardInfo(int rewardAmount, float elapsedSeconds, float capSeconds)
    {
        Text_RewardCount.text = rewardAmount.ToString();

        int capHours = Mathf.FloorToInt(capSeconds / SecondsPerHour);
        Text_OfflineTime.text = $"{BuildElapsedTimeText(elapsedSeconds)} (최대 {capHours}시간)";
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