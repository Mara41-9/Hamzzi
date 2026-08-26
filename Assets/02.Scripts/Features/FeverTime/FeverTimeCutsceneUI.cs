// 피버타임 컷신(영상 재생 자리)과 연타 버튼을 담당하는 UI
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FeverTimeCutsceneUI : UIBase
{
    [SerializeField] private RawImage RawImage_Cutscene;
    [SerializeField] private TMP_Text Text_TapGuide;
    [SerializeField] private UIButton Button_Tap;
    [SerializeField] private Image Image_Fill;

    private VideoPlayer _cutsceneVideoPlayer;

    private void Awake()
    {
        _cutsceneVideoPlayer = RawImage_Cutscene.GetComponent<VideoPlayer>();
    }

    private void OnEnable()
    {
        Button_Tap.BindOnClickButtonEvent(OnClickTap);
        _cutsceneVideoPlayer.Play();
    }

    private void OnDisable()
    {
        _cutsceneVideoPlayer.Stop();
    }

    private void Update()
    {
        var currentFeverTimeData = FeverTimeManager.Instance.GetCurrentFeverTimeData();
        if(currentFeverTimeData == null)
        {
            return;
        }

        var elapsedTime = FeverTimeManager.Instance.GetTapInputElapsedTime();
        var duration = currentFeverTimeData.TapDurationSec;

        Image_Fill.fillAmount = (elapsedTime / duration);
    }

    private void OnClickTap()
    {
        FeverTimeManager.Instance.RegisterTap();
    }
}