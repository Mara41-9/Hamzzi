using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera Camera_Main;

    [Header("기본 시야")]
    [SerializeField] private Vector3 Position_Overview = new Vector3(3f, 4f, -10);
    [SerializeField] private Vector3 Rotation_Overview = Vector3.zero;
    [SerializeField] private float Size_Ortho = 8f;

    [Header("조작감")]
    [SerializeField] private float Duration = 0.8f;
    [SerializeField] private Vector2 Bound_Min = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 Bound_Max = new Vector2(20f, 15f);

    private bool _isTransition;

    private void Awake()
    {
        SetOverview();
    }

    private void Update()
    {
        if (Camera_Main.orthographic && !_isTransition)
        {
            GetTouchInput();
        }
    }

    private void GetTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                float factor = (Camera_Main.orthographicSize * 2f) / Screen.height;
                Vector3 move = new Vector3(-delta.x * factor, -delta.y * factor, 0f);

                Vector3 targetPos = Camera_Main.transform.position + move;

                targetPos.x = Mathf.Clamp(targetPos.x, Bound_Min.x, Bound_Max.x);
                targetPos.y = Mathf.Clamp(targetPos.y, Bound_Min.y, Bound_Max.y);

                Camera_Main.transform.position = targetPos;
            }
        }
    }

    private void SetOverview()
    {
        Camera_Main.orthographic = true;
        Camera_Main.orthographicSize = Size_Ortho;
        Camera_Main.transform.position = Position_Overview;
        Camera_Main.transform.rotation = Quaternion.Euler(Rotation_Overview);
        Camera_Main.ResetProjectionMatrix();
    }
}