using Cysharp.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

public class HousingView : ViewBase
{
    [SerializeField] private Material Material_Ghost;
    [SerializeField] private SpriteRenderer SpriteRenderer_Grid;

    [SerializeField] private Color Color_Valid = new Color(0f, 1f, 0f, 0.4f);
    [SerializeField] private Color Color_Invalid = new Color(1f, 0f, 0f, 0.4f);

    private float _cellSize = 1.0f;
    private float _yOffset = 2.0f;

    private Camera _mainCamera;
    private Plane _mapPlane = new Plane(Vector3.forward, new Vector3(0, 0, 9f));

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

                if (_mapPlane.Raycast(ray, out float hit))
                {
                    Vector3 hitPoint = ray.GetPoint(hit);

                    int gridX = Mathf.FloorToInt(hitPoint.x / _cellSize);
                    int gridY = Mathf.FloorToInt((hitPoint.y - _yOffset) / _cellSize);
                    Vector2Int gridPos = new Vector2Int(gridX, gridY);

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
        else if (_housingVM.CurrentState == HousingState.Placing && _housingVM.FurnitureVM != null)
        {
            if (GetInputPosition(out Vector3 inputPosition))
            {
                Ray ray = _mainCamera.ScreenPointToRay(inputPosition);
                RoomViewModel roomVM = _housingVM.TargetRoom;

                if (roomVM != null)
                {
                    float floorY = (roomVM.OriginPos.y + _yOffset) * _cellSize;
                    Plane roomFloorPlane = new Plane(Vector3.up, new Vector3(0f, floorY, 0f));

                    if (roomFloorPlane.Raycast(ray, out float hit))
                    {
                        Vector3 hitPoint = ray.GetPoint(hit);

                        Vector2Int localPos = roomVM.ChangeLocalGrid(hitPoint, _cellSize);
                        _housingVM.MovePos(localPos);

                        UpdateGhostTransform(roomVM, _housingVM.FurnitureVM);
                    }
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
        GetFurniturePositionAndRotation(roomVM, furnitureVM, out Vector3 pos, out Quaternion rot);

        if (_ghostObject != null)
        {
            _ghostObject.transform.position = pos;
            _ghostObject.transform.rotation = rot;
        }

        if (SpriteRenderer_Grid != null)
        {
            float subCellSize = _cellSize / roomVM.GridFactor;

            SpriteRenderer_Grid.transform.position = new Vector3(pos.x, pos.y + 0.01f, pos.z);
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
        GetFurniturePositionAndRotation(targetRoom, furnitureVM, out Vector3 spawnPos, out Quaternion spawnRot);

        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync(furnitureVM.InstanceID, $"Prefabs/Furniture/{furnitureVM.FurnitureID}", spawnPos);

        if (prefab != null)
        {
            prefab.transform.rotation = spawnRot;
        }
    }

    private void GetFurniturePositionAndRotation(RoomViewModel roomVM, FurnitureViewModel furnitureVM, out Vector3 pos, out Quaternion rot)
    {
        float subCellSize = _cellSize / roomVM.GridFactor;

        float localX = (furnitureVM.LocalPos.x + furnitureVM.Size.x * 0.5f) * subCellSize;
        float localZ = (furnitureVM.LocalPos.y + furnitureVM.Size.y * 0.5f) * subCellSize;

        float worldX = (roomVM.OriginPos.x * _cellSize) + localX;
        float worldY = (roomVM.OriginPos.y + _yOffset) * _cellSize + 0.25f;
        float worldZ = 9.0f - localZ;

        pos = new Vector3(worldX, worldY, worldZ);
        rot = Quaternion.Euler(0f, furnitureVM.RotationAngle, 0f);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || _buildVM == null || _buildVM.Builds == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        foreach (var pair in _buildVM.Builds)
        {
            float offsetY = (pair.Value.BuildType == BuildType.Room) ? _yOffset : 0f;
            Gizmos.DrawWireCube(new Vector3(pair.Key.x + 0.5f, pair.Key.y + 0.5f, 9f), new Vector3(1f, 1f, 0.1f));
        }
    }
}
