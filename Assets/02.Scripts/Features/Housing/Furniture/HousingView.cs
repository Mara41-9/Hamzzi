using Cysharp.Threading.Tasks;
using System.ComponentModel;
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
    private Plane _gridPlane = new Plane(Vector3.forward, new Vector3(0, 0, 9f));

    private HousingViewModel _housingVM;
    private BuildViewModel _buildVM;
    private GameObject _ghostObject;

    private void Awake()
    {
        _mainCamera = Camera.main;
        SpriteRenderer_Grid.gameObject.SetActive(false);
    }

    public void BindViewModel(HousingViewModel housingVM, BuildViewModel buildVM)
    {
        _housingVM = housingVM;
        _buildVM = buildVM;

        _housingVM.PropertyChanged += OnPropertyChanged_VM;
    }

    private void OnDestroy()
    {
        if (_housingVM != null)
        {
            _housingVM.PropertyChanged -= OnPropertyChanged_VM;
        }
    }

    private void OnPropertyChanged_VM(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_housingVM.FurnitureVM):
                if (_housingVM.FurnitureVM != null)
                {
                    string prefabPath = $"Prefabs/Furniture/{_housingVM.FurnitureVM.FurnitureID}";
                    SpawnGhostObject(_housingVM.FurnitureVM.FurnitureID, prefabPath).Forget();
                }
                else
                {
                    ClearGhostObject();
                }
                break;
        }
    }

    private void Update()
    {
        if (_housingVM.CurrentState == HousingState.SelectRoom)
        {
            if (GetInputPosition(out Vector3 inputPosition))
            {
                Debug.Log($"터치 {inputPosition}");
                Ray ray = _mainCamera.ScreenPointToRay(inputPosition);

                if (_gridPlane.Raycast(ray, out float hit))
                {
                    Vector3 hitPoint = ray.GetPoint(hit);
                    Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(hitPoint.x / _cellSize), Mathf.FloorToInt(hitPoint.y / _cellSize));

                    Debug.Log($"{gridPos}, 방 존재: {_buildVM.Builds.ContainsKey(gridPos)}");

                    if (_buildVM.Builds.TryGetValue(gridPos, out RoomViewModel roomVM))
                    {
                        if (roomVM.BuildType == BuildType.Room)
                        {
                            _housingVM.TargetRoom = roomVM;
                        }
                    }
                }
            }
        }
        else if (_housingVM.CurrentState == HousingState.Placing)
        {
            if (GetInputPosition(out Vector3 inputPosition))
            {
                Ray ray = _mainCamera.ScreenPointToRay(inputPosition);

                if (_gridPlane.Raycast(ray, out float hit))
                {
                    Vector3 hitPoint = ray.GetPoint(hit);
                    RoomViewModel roomVM = _housingVM.TargetRoom;

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

    public async UniTask SpawnFurniture(FurnitureViewModel furnitureVM)
    {
        RoomViewModel targetRoom = _housingVM.TargetRoom;

        float cellSize = 1.0f;
        float subCellSize = cellSize / targetRoom.GridFactor;

        float localX = (furnitureVM.LocalPos.x + furnitureVM.Size.x * 0.5f) * subCellSize;
        float localZ = (furnitureVM.LocalPos.y + furnitureVM.Size.y * 0.5f) * subCellSize;

        Vector3 spawnPos = new Vector3((targetRoom.OriginPos.x * cellSize) + localX, 0f, (targetRoom.OriginPos.y * cellSize) + localZ);

        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync(furnitureVM.InstanceID, $"Prefabs/Furniture/{furnitureVM.FurnitureID}", spawnPos);
        prefab.transform.rotation = Quaternion.Euler(0f, furnitureVM.RotationAngle, 0f);
    }
}
