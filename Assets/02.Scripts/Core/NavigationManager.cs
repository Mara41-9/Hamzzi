// 지하굴 통로의 NavMesh 생성, 재베이크를 관리하는 매니저
using UnityEngine;
using Unity.AI.Navigation;

public class NavigationManager : SingletonBase<NavigationManager>
{
    private const KeyCode RebakeKey = KeyCode.N;

    [SerializeField] private NavMeshSurface _navMeshSurface;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(RebakeKey))
        {
            RebuildNavMesh();
        }
#endif
    }

    public void RebuildNavMesh()
    {
        _navMeshSurface.BuildNavMesh();
    }
}