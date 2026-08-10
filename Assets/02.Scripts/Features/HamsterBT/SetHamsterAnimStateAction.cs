// 햄스터 애니메이션 상태를 변경하는 Behavior Graph 커스텀 액션
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Hamster Anim State", story: "[Self] sets anim state", category: "Action", id: "cfbdcf5352cbc5a2cd3a71adeee6ac9c")]
public partial class SetHamsterAnimStateAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<HamsterAnimatorController> HamsterAnimator;
    [SerializeReference] public BlackboardVariable<HamsterAnimatorController.HamsterAnimState> State;

    protected override Status OnStart()
    {
        HamsterAnimator.Value.SetState(State.Value);
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}