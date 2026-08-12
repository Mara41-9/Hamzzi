// 피버타임 컷신(영상 재생 자리)과 연타 버튼을 담당하는 UI
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeverTimeCutsceneUI : UIBase
{
    [SerializeField] private RawImage RawImage_Cutscene;
    [SerializeField] private TMP_Text Text_TapGuide;
    [SerializeField] private UIButton Button_Tap;

    private void OnEnable()
    {
        Button_Tap.BindOnClickButtonEvent(OnClickTap);
    }

    private void OnClickTap()
    {
        FeverTimeManager.Instance.RegisterTap();
    }
}