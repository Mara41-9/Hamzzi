using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class HousingView : ViewBase
{
    [SerializeField] private Material Material_Ghost;
    [SerializeField] private SpriteRenderer SpriteRenderer_Tile;
    [SerializeField] private GameObject Prefab_Grid;

    [SerializeField] private Color Color_Valid = new Color(0f, 1f, 0f, 0.4f);
    [SerializeField] private Color Color_Invalid = new Color(1f, 0f, 0f, 0.4f);

    private Vector3 _gardenOrigin = new Vector3(-20f, 12f, 12f);
    private Vector2Int _gardenGridSize = new Vector2Int(150, 60);
    private float _gardenSubCellSize = 0.5f;

    private float _cellSize = 1.0f;
    private float _yOffset = 2.0f;

    private Camera _mainCamera;
    private Plane _mapPlane = new Plane(Vector3.forward, new Vector3(0, 0, 9f));

    private HousingViewModel _housingVM;
    private BuildViewModel _buildVM;
    private GameObject _ghostObject;

    private List<GameObject> _activeGridLines = new List<GameObject>();
    private Dictionary<string, GameObject> _spawnFurniture = new Dictionary<string, GameObject>();

    private void Awake()
    {
        _mainCamera = Camera.main;
        SpriteRenderer_Tile.gameObject.SetActive(false);
    }

    private void Start()
    {
        BuildViewModel buildVM = ServiceManager.Instance.BuildService.GetBuildViewModel();
        HousingViewModel housingVM = ServiceManager.Instance.HousingService.GetHousingViewModel();

        BindViewModel(housingVM, buildVM);
    }

    public void BindViewModel(HousingViewModel housingVM, BuildViewModel buildVM)
    {
        _housingVM = housingVM;
        _buildVM = buildVM;

        _housingVM.PropertyChanged += OnPropertyChanged_VM;

        if (_housingVM.CurrentViewMode == HousingViewMode.Garden)
        {
            ShowGardenGrid().Forget();
        }
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
            case nameof(_housingVM.CurrentViewMode):
                if (_housingVM.CurrentViewMode == HousingViewMode.Garden)
                {
                    ShowGardenGrid().Forget();
                }
                else if (_housingVM.CurrentViewMode == HousingViewMode.OverView)
                {
                    ClearRoomGrid();
                }
                break;

            case nameof(_housingVM.TargetRoom):
                if (_housingVM.TargetRoom != null)
                {
                    ShowRoomGrid(_housingVM.TargetRoom).Forget();
                }
                break;

            case nameof(_housingVM.FurnitureVM):
                if (_housingVM.FurnitureVM != null)
                {
                    if (_ghostObject != null)
                    {
                        if (_housingVM.CurrentViewMode == HousingViewMode.Garden)
                        {
                            UpdateGardenGhostTransform(_housingVM.FurnitureVM);
                        }
                        else if (_housingVM.TargetRoom != null)
                        {
                            UpdateGhostTransform(_housingVM.TargetRoom, _housingVM.FurnitureVM);
                        }
                    }
                    else
                    {
                        SpawnGhostObject(_housingVM.FurnitureVM.FurnitureID, _housingVM.FurnitureVM.PrefabPath).Forget();
                    }
                }
                else
                {
                    ClearGhostObject();
                }
                break;

            case nameof(_housingVM.DestroyFurniture):
                if (_housingVM.DestroyFurniture != null)
                {
                    string id = _housingVM.DestroyFurniture.InstanceID;

                    if (_spawnFurniture.TryGetValue(id, out GameObject target))
                    {
                        GameObjectManager.Instance.RequestDestroyObject(target);
                        _spawnFurniture.Remove(id);
                    }
                }
                break;
        }
    }

    private void Update()
    {
        if (_buildVM.SelectType != BuildType.None || _buildVM.CanConfirm)
        {
            return;
        }

        if (_housingVM.CurrentViewMode == HousingViewMode.Garden)
        {
            if (_housingVM.FurnitureVM == null)
            {
                if (GetInputPosition(out Vector3 inputPosition))
                {
                    Ray ray = _mainCamera.ScreenPointToRay(inputPosition);

                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.collider.TryGetComponent<FurnitureView>(out var furnitureView))
                        {
                            if (furnitureView.FurnitureVM != null)
                            {
                                string instanceID = furnitureView.FurnitureVM.InstanceID;

                                if (_spawnFurniture.TryGetValue(instanceID, out GameObject obj))
                                {
                                    GameObjectManager.Instance.RequestDestroyObject(obj);
                                    _spawnFurniture.Remove(instanceID);
                                }

                                _housingVM.SelectInstallFurniture(furnitureView.FurnitureVM);
                            }
                        }
                    }
                }
            }
            else
            {
                if (GetInputPosition(out Vector3 inputPosition))
                {
                    Ray ray = _mainCamera.ScreenPointToRay(inputPosition);
                    Plane gardenPlane = new Plane(Vector3.up, new Vector3(0f, _gardenOrigin.y, 0f));

                    if (gardenPlane.Raycast(ray, out float hit))
                    {
                        Vector3 hitPoint = ray.GetPoint(hit);

                        float halfWidth = _housingVM.FurnitureVM.Size.x * 0.5f * _gardenSubCellSize;
                        float halfDepth = _housingVM.FurnitureVM.Size.y * 0.5f * _gardenSubCellSize;

                        float localX = (hitPoint.x - halfWidth) - _gardenOrigin.x;
                        float localZ = (hitPoint.z - halfDepth) - _gardenOrigin.z;

                        int gridX = Mathf.RoundToInt(localX / _gardenSubCellSize);
                        int gridZ = Mathf.RoundToInt(localZ / _gardenSubCellSize);

                        gridX = Mathf.Clamp(gridX, 0, _gardenGridSize.x - _housingVM.FurnitureVM.Size.x);
                        gridZ = Mathf.Clamp(gridZ, 0, _gardenGridSize.y - _housingVM.FurnitureVM.Size.y);

                        _housingVM.MovePos(new Vector2Int(gridX, gridZ));
                        UpdateGardenGhostTransform(_housingVM.FurnitureVM);
                    }
                }
            }
            return;
        }

        if (_housingVM.CurrentState == HousingState.SelectRoom)
        {
            if (GetInputPosition(out Vector3 inputPosition))
            {
                Ray ray = _mainCamera.ScreenPointToRay(inputPosition);

                if (_mapPlane.Raycast(ray, out float hit))
                {
                    Vector3 hitPoint = ray.GetPoint(hit);

                    int gridX = Mathf.FloorToInt(hitPoint.x / _cellSize);
                    int gridY = Mathf.FloorToInt((hitPoint.y - _yOffset) / _cellSize);
                    Vector2Int gridPos = new Vector2Int(gridX, gridY);

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
        else if (_housingVM.CurrentState == HousingState.Placing && _housingVM.FurnitureVM == null)
        {
            if (GetInputPosition(out Vector3 inputPosition))
            {
                Ray ray = _mainCamera.ScreenPointToRay(inputPosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.TryGetComponent<FurnitureView>(out var furnitureView))
                    {
                        if (furnitureView.FurnitureVM != null)
                        {
                            string instanceID = furnitureView.FurnitureVM.InstanceID;

                            if (_spawnFurniture.TryGetValue(instanceID, out GameObject obj))
                            {
                                GameObjectManager.Instance.RequestDestroyObject(obj);
                                _spawnFurniture.Remove(instanceID);
                            }

                            _housingVM.SelectInstallFurniture(furnitureView.FurnitureVM);
                        }
                    }
                }
            }
        }
        else if (_housingVM.FurnitureVM != null)
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
                        Vector2Int localPos = roomVM.ChangeLocalGrid(hitPoint, _housingVM.FurnitureVM.Size, _cellSize);
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

        if (SpriteRenderer_Tile != null)
        {
            float subCellSize = _cellSize / roomVM.GridFactor;

            SpriteRenderer_Tile.transform.position = new Vector3(pos.x, pos.y + 0.01f, pos.z);
            SpriteRenderer_Tile.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            SpriteRenderer_Tile.transform.localScale = new Vector3(furnitureVM.Size.x * subCellSize, furnitureVM.Size.y * subCellSize, 1f);

            SpriteRenderer_Tile.color = furnitureVM.IsValid ? Color_Valid : Color_Invalid;
        }
    }

    private void UpdateGardenGhostTransform(FurnitureViewModel furnitureVM)
    {
        GetGardenFurniturePositionAndRotation(furnitureVM, out Vector3 pos, out Quaternion rot);

        if (_ghostObject != null)
        {
            _ghostObject.transform.position = pos;
            _ghostObject.transform.rotation = rot;
        }

        if (SpriteRenderer_Tile != null)
        {
            SpriteRenderer_Tile.transform.position = new Vector3(pos.x, _gardenOrigin.y + 0.02f, pos.z);
            SpriteRenderer_Tile.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            SpriteRenderer_Tile.transform.localScale = new Vector3(furnitureVM.Size.x * _gardenSubCellSize, furnitureVM.Size.y * _gardenSubCellSize, 1f);
            SpriteRenderer_Tile.color = furnitureVM.IsValid ? Color_Valid : Color_Invalid;
        }
    }

    public async UniTask SpawnGhostObject(string furnitureID, string prefabPath)
    {
        ClearGhostObject();

        _ghostObject = await GameObjectManager.Instance.CreateObjectAsync(furnitureID, prefabPath, Vector3.zero);

        if (_ghostObject.TryGetComponent<FurnitureView>(out var furnitureView))
        {
            furnitureView.SetGhostMode(Material_Ghost);

            float subCellSize = (_housingVM.CurrentViewMode == HousingViewMode.Garden) ? _gardenSubCellSize : (_cellSize / _housingVM.TargetRoom.GridFactor);
            Vector2Int calculatedSize = furnitureView.GetFurnitureSize(subCellSize);

            if (_housingVM.FurnitureVM.RotationAngle % 180 != 0)
            {
                calculatedSize = new Vector2Int(calculatedSize.y, calculatedSize.x);
            }

            _housingVM.FurnitureVM.Size = calculatedSize;
        }

        if (_housingVM.CurrentViewMode == HousingViewMode.Garden)
        {
            UpdateGardenGhostTransform(_housingVM.FurnitureVM);
        }
        else if (_housingVM.TargetRoom != null)
        {
            UpdateGhostTransform(_housingVM.TargetRoom, _housingVM.FurnitureVM);
        }

        SpriteRenderer_Tile.gameObject.SetActive(true);
    }

    public void ClearGhostObject()
    {
        if (_ghostObject != null)
        {
            GameObjectManager.Instance.RequestDestroyObject(_ghostObject);
            _ghostObject = null;
        }

        SpriteRenderer_Tile.gameObject.SetActive(false);
    }

    public async UniTask SpawnFurniture(FurnitureViewModel furnitureVM)
    {
        Vector3 spawnPos;
        Quaternion spawnRot;

        if (_housingVM.CurrentViewMode == HousingViewMode.Garden)
        {
            GetGardenFurniturePositionAndRotation(furnitureVM, out spawnPos, out spawnRot);
        }
        else
        {
            GetFurniturePositionAndRotation(_housingVM.TargetRoom, furnitureVM, out spawnPos, out spawnRot);
        }

        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync(furnitureVM.InstanceID, furnitureVM.PrefabPath, spawnPos);
        prefab.transform.rotation = spawnRot;

        FurnitureView furnitureView = prefab.GetComponent<FurnitureView>();
        furnitureView.ResetMaterial();
        furnitureView.Bind(furnitureVM);

        _spawnFurniture[furnitureVM.InstanceID] = prefab;
    }

    private void GetFurniturePositionAndRotation(RoomViewModel roomVM, FurnitureViewModel furnitureVM, out Vector3 pos, out Quaternion rot)
    {
        float subCellSize = _cellSize / roomVM.GridFactor;

        float localX = (furnitureVM.LocalPos.x + furnitureVM.Size.x * 0.5f) * subCellSize;
        float localZ = (furnitureVM.LocalPos.y + furnitureVM.Size.y * 0.5f) * subCellSize;

        float worldX = (roomVM.OriginPos.x * _cellSize) + localX;
        float worldY = (roomVM.OriginPos.y + _yOffset) * _cellSize + 0.2f;
        float worldZ = 9.0f - localZ;

        pos = new Vector3(worldX, worldY, worldZ);
        rot = Quaternion.Euler(0f, furnitureVM.RotationAngle, 0f);
    }

    private void GetGardenFurniturePositionAndRotation(FurnitureViewModel furnitureVM, out Vector3 pos, out Quaternion rot)
    {
        float localX = (furnitureVM.LocalPos.x + furnitureVM.Size.x * 0.5f) * _gardenSubCellSize;
        float localZ = (furnitureVM.LocalPos.y + furnitureVM.Size.y * 0.5f) * _gardenSubCellSize;

        float worldX = _gardenOrigin.x + localX;
        float worldY = _gardenOrigin.y;
        float worldZ = _gardenOrigin.z + localZ;

        pos = new Vector3(worldX, worldY, worldZ);
        rot = Quaternion.Euler(0f, furnitureVM.RotationAngle, 0f);
    }

    public async UniTask ShowRoomGrid(RoomViewModel roomVM)
    {
        ClearRoomGrid();

        float subCellSize = _cellSize / roomVM.GridFactor;
        float roomX = roomVM.OriginPos.x * _cellSize;
        float roomY = (roomVM.OriginPos.y + _yOffset) * _cellSize + 0.21f;

        int subX = roomVM.SubGridSize.x;
        int subY = roomVM.SubGridSize.y;

        float totalWidth = subX * subCellSize;
        float totalHeight = subY * subCellSize;

        for (int x = 0; x <= subX; x++)
        {
            float currentX = roomX + (x * subCellSize);
            Vector3 pos = new Vector3(currentX, roomY, 9.0f - (totalHeight * 0.5f));

            GameObject line = await GameObjectManager.Instance.CreateObjectAsync("GridLine", "Prefabs/UI/GridLine", pos);

            if (line != null)
            {
                line.transform.SetParent(transform);
                line.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                line.transform.localScale = new Vector3(0.02f, totalHeight, 1f);
                _activeGridLines.Add(line);
            }
        }

        for (int y = 0; y <= subY; y++)
        {
            float currentZ = 9.0f - (y * subCellSize);
            Vector3 pos = new Vector3(roomX + (totalWidth * 0.5f), roomY, currentZ);

            GameObject line = await GameObjectManager.Instance.CreateObjectAsync("GridLine", "Prefabs/UI/GridLine", pos);

            if (line != null)
            {
                line.transform.SetParent(transform);
                line.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                line.transform.localScale = new Vector3(totalWidth, 0.02f, 1f);
                _activeGridLines.Add(line);
            }
        }
    }

    public async UniTask ShowGardenGrid()
    {
        ClearRoomGrid();

        float totalWidth = _gardenGridSize.x * _gardenSubCellSize;
        float totalDepth = _gardenGridSize.y * _gardenSubCellSize;
        float gridY = _gardenOrigin.y + 0.015f;

        for (int y = 0; y <= _gardenGridSize.y; y++)
        {
            float currentZ = _gardenOrigin.z + (y * _gardenSubCellSize);
            Vector3 pos = new Vector3(_gardenOrigin.x + (totalWidth * 0.5f), gridY, currentZ);

            GameObject line = await GameObjectManager.Instance.CreateObjectAsync("GridLine", "Prefabs/UI/GridLine", pos);

            if (line != null)
            {
                line.transform.SetParent(transform);
                line.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                line.transform.localScale = new Vector3(totalWidth, 0.02f, 1f);
                _activeGridLines.Add(line);
            }
        }

        for (int x = 0; x <= _gardenGridSize.x; x++)
        {
            float currentX = _gardenOrigin.x + (x * _gardenSubCellSize);
            Vector3 pos = new Vector3(currentX, gridY, _gardenOrigin.z + (totalDepth * 0.5f));

            GameObject line = await GameObjectManager.Instance.CreateObjectAsync("GridLine", "Prefabs/UI/GridLine", pos);

            if (line != null)
            {
                line.transform.SetParent(transform);
                line.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                line.transform.localScale = new Vector3(0.02f, totalDepth, 1f);
                _activeGridLines.Add(line);
            }
        }
    }

    public void ClearRoomGrid()
    {
        for (int i = 0; i < _activeGridLines.Count; i++)
        {
            if (_activeGridLines[i] != null)
            {
                GameObjectManager.Instance.RequestDestroyObject(_activeGridLines[i]);
            }
        }

        _activeGridLines.Clear();
    }
}
