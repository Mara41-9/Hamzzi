//햄스터 애니메이션 상태를 Animator State 재생으로 제어하는 컴포넌트
using UnityEngine;

public class HamsterAnimatorController : MonoBehaviour
{
    public enum HamsterAnimState
    {
        None = 0,
        Idle,
        Farm
    }

    private const string StateIdle = "Run";
    private const string StateFarm = "Making";
    private const float CrossFadeDuration = 0.2f;

    [SerializeField] private Animator Animator_Hamster;

    private HamsterAnimState _currentState;

    public void SetState(HamsterAnimState state)
    {
        if (_currentState == state)
        {
            return;
        }

        _currentState = state;

        switch (state)
        {
            case HamsterAnimState.Idle:
                Animator_Hamster.CrossFade(StateIdle, CrossFadeDuration);
                break;
            case HamsterAnimState.Farm:
                Animator_Hamster.CrossFade(StateFarm, CrossFadeDuration);
                break;
        }
    }
}