//햄스터 애니메이션 상태를 Animator 파라미터로 변환해 재생을 제어하는 컴포넌트
using UnityEngine;

public class HamsterAnimatorController : MonoBehaviour
{
    public enum HamsterAnimState
    {
        None = 0,
        Idle,
        Farm
    }

    private const string ParamIsIdle = "IsIdle";
    private const string ParamIsFarming = "IsFarming";

    [SerializeField] private Animator Animator_Hamster;

    private HamsterAnimState _currentState;

    public void SetState(HamsterAnimState state)
    {
        if (_currentState == state)
        {
            return;
        }

        _currentState = state;
        ResetAllAnimParameters();

        switch (state)
        {
            case HamsterAnimState.Idle:
                Animator_Hamster.SetBool(ParamIsIdle, true);
                break;
            case HamsterAnimState.Farm:
                Animator_Hamster.SetBool(ParamIsFarming, true);
                break;
        }
    }

    private void ResetAllAnimParameters()
    {
        Animator_Hamster.SetBool(ParamIsIdle, false);
        Animator_Hamster.SetBool(ParamIsFarming, false);
    }
}
