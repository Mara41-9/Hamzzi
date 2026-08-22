using Unity.AI.Navigation;
using UnityEngine;

public class AisleNavMeshLink : MonoBehaviour
{
    [SerializeField] private NavMeshLink NavMeshLink;

    [SerializeField] private Transform Transform_Start;
    [SerializeField] private Transform Transform_End;

    private void Awake()
    {
        if (NavMeshLink == null)
        {
            NavMeshLink = GetComponent<NavMeshLink>();
        }
    }

    public void SetPoint(Transform start, Transform end)
    {
        Transform_Start = start;
        Transform_End = end;

        Refresh();
    }

    public void Refresh()
    {
        if (NavMeshLink == null || Transform_Start == null || Transform_End == null)
        {
            return;
        }

        transform.position = Transform_Start.position;
        transform.rotation = Quaternion.identity;

        NavMeshLink.startPoint = Vector3.zero;
        NavMeshLink.endPoint = transform.InverseTransformPoint(Transform_End.position);

        NavMeshLink.bidirectional = true;
        NavMeshLink.activated = true;

        NavMeshLink.UpdateLink();
    }
}