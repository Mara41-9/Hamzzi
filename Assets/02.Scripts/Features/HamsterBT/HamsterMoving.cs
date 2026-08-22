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
        CheckNavMesh();

        if (!_isInitialized && NavMeshAgent.enabled && NavMeshAgent.isOnNavMesh)
        {
            _isInitialized = true;
            SetRandomDestination();
        }

        if (_isInitialized && NavMeshAgent.enabled && NavMeshAgent.isOnNavMesh)
        {
            if (!NavMeshAgent.pathPending && (NavMeshAgent.remainingDistance <= _threshold || !NavMeshAgent.hasPath))
            {
                SetRandomDestination();
            }
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
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
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