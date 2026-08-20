// 햄스터가 파밍 중 씨앗을 채집하는 Behavior Graph 커스텀 액션
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Collect Seed", story: "[Self] collects Seeds", category: "Action", id: "703319309d47c61b022929dec46e2428")]
public partial class CollectSeedAction : Action
{
    private const int SeedPerCollect = 1;

    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        ServiceManager.Instance.CurrencyService.AddSeed(SeedPerCollect);

#if UNITY_EDITOR
        Debug.Log($"씨앗 채집 (+{SeedPerCollect})");
#endif

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

