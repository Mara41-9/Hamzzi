// 방치 보상 획득 시 보상 수량을 보여주는 팝업 UI
using TMPro;
using UnityEngine;

public class IdleRewardPopupUI : UIBase
{
    [SerializeField] private TMP_Text Text_RewardAmount;
    [SerializeField] private UIButton Button_Confirm;

    private void OnEnable()
    {
        Button_Confirm.BindOnClickButtonEvent(OnClickConfirm);
    }

    public void SetRewardAmount(int rewardAmount)
    {
        Text_RewardAmount.text = $"+{rewardAmount}";
    }

    private void OnClickConfirm()
    {
        UIManager.Instance.CloseIdleRewardPopupUI();
    }
}