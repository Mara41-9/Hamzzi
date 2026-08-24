using UnityEngine;
using UnityEngine.AI;

public class HamsterMoving : MonoBehaviour
{
    [SerializeField] private NavMeshAgent NavMeshAgent;

    private float _radius = 30f;
    private float _threshold = 1.0f;
    private bool _isInitialized = false;

    void Update()
    {
        if (NavMeshAgent == null || !NavMeshAgent.enabled)
        {
            return;
        }

        if (!NavMeshAgent.isOnNavMesh)
        {
            CheckNavMesh();
            return;
        }

        if (!_isInitialized || (!NavMeshAgent.pathPending && (NavMeshAgent.remainingDistance <= _threshold || !NavMeshAgent.hasPath)))
        {
            _isInitialized = true;
            SetRandomDestination();
        }
    }

    private void CheckNavMesh()
    {
        if (NavMeshAgent == null || !NavMeshAgent.enabled)
        {
            return;
        }

        if (!NavMeshAgent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 50.0f, NavMesh.AllAreas))
            {
                NavMeshAgent.Warp(hit.position);
                _isInitialized = true;
                SetRandomDestination();
            }
        }
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _radius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _radius, NavMesh.AllAreas))
        {
            NavMeshAgent.SetDestination(hit.position);
        }
    }
}