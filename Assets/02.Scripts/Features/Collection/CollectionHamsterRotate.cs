using UnityEngine;
using UnityEngine.EventSystems;

public class CollectionHamsterRotate : MonoBehaviour, IDragHandler
{
    private Transform _hamsterRoot;
    [SerializeField] private float rotationSpeed = 0.5f;

    public void SetHamsterRoot(Transform hamsterRoot)
    {
        _hamsterRoot = hamsterRoot;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _hamsterRoot.Rotate(
            Vector3.up,
            -eventData.delta.x * rotationSpeed,
            Space.World
        );
    }
}
