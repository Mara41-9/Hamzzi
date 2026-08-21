using UnityEngine;
using UnityEngine.EventSystems;

public class HamsterInput : MonoBehaviour
{
    private Camera _mainCamera;
    private CameraController _cameraController;

    private void Start()
    {
        _mainCamera = Camera.main;
        _cameraController = _mainCamera.GetComponent<CameraController>();
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            CheckTouch(Input.mousePosition);
        }

#elif UNITY_ANDROID
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckTouch(Input.GetTouch(0).position);
        }
#endif
    }

    private void CheckTouch(Vector2 screenPos)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                Debug.Log(gameObject.name);
                _cameraController.StartFollowHamster(transform);
            }
        }
    }
}