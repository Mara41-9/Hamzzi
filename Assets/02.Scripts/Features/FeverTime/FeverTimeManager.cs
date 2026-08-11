//쳇바퀴 피버타임 미니게임 상태와 진행을 관리하는 매니저
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
    private FeverTimeState _currentState;

    public void SetState(FeverTimeState state)
    {
        if (_currentState == state)
            return;

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
        Debug.Log("FeverTimeState: CutscenePlaying (TODO: HAM-66 컷신 UI 연결되면 이 로그를 교체)");
#endif
    }
    private void HandleTapInputState()
    {
#if UNITY_EDITOR
        Debug.Log("FeverTimeState: TapInput(TODO: HAM-67 연타 로직 연결되면 이 로그를 교체)");
#endif
    }
    private void HandleResultState()
    {
#if UNITY_EDITOR
        Debug.Log("FeverTimeState: Result(TODO: HAM-69 보상 계산 연결되면 이 로그를 교체)");
#endif
    }
}
