using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class HousingView : ViewBase
{
    [SerializeField] private Material Material_Ghost;
    [SerializeField] private SpriteRenderer SpriteRenderer_Grid;

    [SerializeField] private Color Color_Valid = new Color(0f, 1f, 0f, 0.4f);
    [SerializeField] private Color Color_Invalid = new Color(1f, 0f, 0f, 0.4f);

    private float _cellSize = 1.0f;

    private Camera _mainCamera;
    private Plane _gridPlane = new Plane(Vector3.up, Vector3.zero);

    private HousingViewModel _housingVM;
    private GameObject _ghostObject;

    private void Awake()
    {
        _mainCamera = Camera.main;
        SpriteRenderer_Grid.gameObject.SetActive(false);
    }

    public void BindViewModel(HousingViewModel housingVM)
    {
        _housingVM = housingVM;
    }

    private void Update()
    {
        if (GetInputPosition(out Vector3 inputPosition))
        {
            Ray ray = _mainCamera.ScreenPointToRay(inputPosition);

            if (_gridPlane.Raycast(ray, out float hit))
            {
                Vector3 hitPoint = ray.GetPoint(hit);
                RoomViewModel roomVM = _housingVM.TargetRoom;

                if (roomVM != null)
                {
                    Vector2Int localPos = roomVM.ChangeLocalGrid(hitPoint, _cellSize);
                    _housingVM.MovePos(localPos);

                    UpdateGhostTransform(roomVM, _housingVM.FurnitureVM);
                }
            }
        }
    }

    private bool GetInputPosition(out Vector3 inputPosition)
    {
        inputPosition = Vector3.zero;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            inputPosition = Input.mousePosition;
            return true;
        }
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return false;
            }

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                inputPosition = touch.position;
                return true;
            }
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return false;
            }

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                inputPosition = touch.position;
                return true;
            }
        }
#endif

        return false;
    }

    private void UpdateGhostTransform(RoomViewModel roomVM, FurnitureViewModel furnitureVM)
    {
        float subCellSize = _cellSize / roomVM.GridFactor;

        float localX = (furnitureVM.LocalPos.x + furnitureVM.Size.x * 0.5f) * subCellSize;
        float localZ = (furnitureVM.LocalPos.y + furnitureVM.Size.y * 0.5f) * subCellSize;

        float worldX = (roomVM.OriginPos.x * _cellSize) + localX;
        float worldZ = (roomVM.OriginPos.y * _cellSize) + localZ;

        if (_ghostObject != null)
        {
            _ghostObject.transform.position = new Vector3(worldX, 0, worldZ);
            _ghostObject.transform.rotation = Quaternion.Euler(0, furnitureVM.RotationAngle, 0);
        }

        if (SpriteRenderer_Grid !=  null)
        {
            SpriteRenderer_Grid.transform.position = new Vector3(worldX, 0.01f, worldZ);
            SpriteRenderer_Grid.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            SpriteRenderer_Grid.transform.localScale = new Vector3(furnitureVM.Size.x * subCellSize, furnitureVM.Size.y * subCellSize, 1f);

            SpriteRenderer_Grid.color = furnitureVM.IsValid ? Color_Valid : Color_Invalid;
        }
    }

    public async UniTask SpawnGhostObject(string furnitureID, string prefabPath)
    {
        ClearGhostObject();

        _ghostObject = await GameObjectManager.Instance.CreateObjectAsync(furnitureID, prefabPath, Vector3.zero);

        if (_ghostObject.TryGetComponent<FurnitureView>(out var furnitureView))
        {
            furnitureView.SetGhostMode(Material_Ghost);
        }

        UpdateGhostTransform(_housingVM.TargetRoom, _housingVM.FurnitureVM);

        SpriteRenderer_Grid.gameObject.SetActive(true);
    }

    public void ClearGhostObject()
    {
        if (_ghostObject != null)
        {
            GameObjectManager.Instance.RequestDestroyObject(_ghostObject);
            _ghostObject = null;
        }

        SpriteRenderer_Grid.gameObject.SetActive(false);
    }
}
