using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : UIBase
{
    [SerializeField] private RawImage RawImage_Loading;   
    [SerializeField] private Slider Slider_LoadingBar;
    [SerializeField] private TMP_Text Text_loading;
    [SerializeField] private TMP_Text Text_Tip;

    private CancellationTokenSource _cancelToken;   
    float[] _pausePoints = { 0.2f, 0.2f, 0.2f };    
    int _pauseIndex = 0;                            

    private void OnEnable()
    {
        LoadAndSetLoadingImg().Forget();
        StartLoadingResouce(2f).Forget();
        PlayLoadingText().Forget();
        LoadAndSetTipText().Forget();
    }

    // 로딩 이미지 선택 + 로딩바 시작을 담당하는 함수
    private async UniTask LoadAndSetLoadingImg()
    {
        Slider_LoadingBar.value = 0f;

        // 이전 이미지 안 보이게
        RawImage_Loading.texture = null;

        int randomIdx = UnityEngine.Random.Range(0, 2);

        string texturePath = string.Empty;

        switch (randomIdx)
        {
            case 0:
                texturePath = "Texture2D/Texture2D_Loading_01";
                break;
            case 1:
                texturePath = "Texture2D/Texture2D_Loading_02";
                break;
        }

        Texture2D texture = await ResourceManager.Instance.LoadAsset<Texture2D>(texturePath);
        RawImage_Loading.texture = texture;

        // 새 이미지 준비된 뒤 표시
        RawImage_Loading.enabled = true;

        await UniTask.Yield();
    }

    // 로딩바를 일정 시간 동안 채우는 비동기 함수
    public async UniTaskVoid StartLoadingResouce(float duration)
    {
        _cancelToken = new CancellationTokenSource();

        float elapsed = 0f;
        Slider_LoadingBar.value = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            Slider_LoadingBar.value = progress;

            if (_pauseIndex < _pausePoints.Length && progress >= _pausePoints[_pauseIndex])
            {
                float pausePointValue = _pausePoints[_pauseIndex];
                Slider_LoadingBar.value = pausePointValue;
                await UniTask.Delay(TimeSpan.FromSeconds(pausePointValue), cancellationToken: _cancelToken.Token);
                _pauseIndex++;
            }

            Slider_LoadingBar.value = progress;

            await UniTask.Yield(PlayerLoopTiming.Update, _cancelToken.Token);
        }

        Slider_LoadingBar.value = 1.0f;
        UIManager.Instance.CloseLoadingUI();
    }

    public async UniTask PlayLoadingText()
    {
        const string loadingText = "씨앗을 모으는 중이에요";

        int dotCount = 1;

        while(true)
        {
            Text_loading.text = loadingText + new string('.', dotCount);

            dotCount = dotCount % 3 + 1;

            await UniTask.Delay(400, cancellationToken: _cancelToken.Token);
        }
    }

    public async UniTask LoadAndSetTipText()
    {
        // 이전 텍스트 안 보이게
        Text_Tip.text = "";

        int randomIdx = UnityEngine.Random.Range(0, 2);

        string tipText = string.Empty;

        switch (randomIdx)
        {
            case 0:
                tipText = "TIP. 가구를 배치하면 햄찌들이 더 행복해져요!";
                break;
            case 1:
                tipText = "TIP. 햄찌들은 쉬고 있을 때도 열심히 씨앗을 모아와요!";
                break;
        }

        Text_Tip.text = tipText;

        await UniTask.Yield();
    }
    
}
