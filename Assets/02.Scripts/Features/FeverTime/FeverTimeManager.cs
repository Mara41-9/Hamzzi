// 쳇바퀴 피버타임 미니게임 상태와 진행을 관리하는 매니저
using System;
using UnityEngine;

public enum FeverTimeState
{
    None = 0,
    Idle,
    Ready,
    CutscenePlaying,
    TapInput,
    Result
}

public class FeverTimeManager : SingletonBase<FeverTimeManager>
{
    public event Action OnFeverTimeEnded;

    private FeverTimeState _currentState;

    private const int TEMP_TAP_DURATION_SEC = 5; // TODO: HAM-68 데이터테이블 연동되면 교체

    private int _tapCount;
    private float _tapInputElapsedTime;

    private void Update()
    {
        if (_currentState != FeverTimeState.TapInput)
        {
            return;
        }

        _tapInputElapsedTime += Time.deltaTime;

        if (_tapInputElapsedTime >= TEMP_TAP_DURATION_SEC)
        {
            SetState(FeverTimeState.Result);
        }
    }

    public void RegisterTap()
    {
        if (_currentState != FeverTimeState.TapInput)
        {
            return;
        }

        _tapCount++;

#if UNITY_EDITOR
        Debug.Log($"연타 카운트: {_tapCount}");
#endif
    }

    public void SetState(FeverTimeState state)
    {
        if (_currentState == state)
        {
            return;
        }

        _currentState = state;

        switch (_currentState)
        {
            case FeverTimeState.Idle:
                HandleIdleState();
                break;
            case FeverTimeState.Ready:
                HandleReadyState();
                break;
            case FeverTimeState.CutscenePlaying:
                HandleCutscenePlayingState();
                break;
            case FeverTimeState.TapInput:
                HandleTapInputState();
                break;
            case FeverTimeState.Result:
                HandleResultState();
                break;
        }
    }

    private void HandleIdleState()
    {
#if UNITY_EDITOR
        Debug.Log("FeverTimeState: Idle");
#endif
        UIManager.Instance.CloseFeverTimeCutsceneUI();
        OnFeverTimeEnded?.Invoke();
    }

    private void HandleReadyState()
    {
#if UNITY_EDITOR
        Debug.Log("FeverTimeState: Ready");
#endif
    }

    private void HandleCutscenePlayingState()
    {
#if UNITY_EDITOR
        Debug.Log("FeverTimeState: CutscenePlaying");
#endif
        UIManager.Instance.OpenFeverTimeCutsceneUI();

        // TODO: 실제 영상 에셋 연결되면 영상 재생이 끝나는 시점(VideoPlayer 재생 완료 콜백)에 아래 호출로 교체
        SetState(FeverTimeState.TapInput);
    }

    private void HandleTapInputState()
    {
#if UNITY_EDITOR
        Debug.Log("FeverTimeState: TapInput");
#endif
        _tapCount = 0;
        _tapInputElapsedTime = 0f;
    }

    private void HandleResultState()
    {
#if UNITY_EDITOR
        Debug.Log("FeverTimeState: Result (TODO: HAM-69 보상 계산 연결되면 이 로그를 교체, 계산 끝나면 SetState(Idle) 호출 필요)");
#endif
    }
}