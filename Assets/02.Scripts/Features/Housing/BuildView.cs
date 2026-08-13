using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildView : ViewBase
{
    [SerializeField] public List<Vector2Int> Transform_DefaultRoom;
    [SerializeField] public List<Vector2Int> Transform_DefaultAisle;

    private Dictionary<string, GameObject> _spawnRoom = new Dictionary<string, GameObject>();

    private float _cellSize = 1.0f;
    private Camera _mainCamera;

    private BuildViewModel _buildVM;
    private Plane _gridPlane = new Plane(Vector3.forward, new Vector3(0, 0, 9f));

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        if (_buildVM == null)
        {
            BuildViewModel buildVM = ServiceManager.Instance.BuildService.GetBuildViewModel();
            BindViewModel(buildVM);
        }

        //_buildVM.InitDefaultRoom(Transform_DefaultRoom, Transform_DefaultAisle);
    }

    public void BindViewModel(BuildViewModel buildVM)
    {
        _buildVM = buildVM;
        _buildVM.PropertyChanged += OnPropertyChanged_View;
    }

    private void OnDestroy()
    {
        if (_buildVM != null)
        {
            _buildVM.PropertyChanged -= OnPropertyChanged_View;
        }
    }

    private void Update()
    {
        if (_buildVM.SelectType == BuildType.None)
        {
            return;
        }

        if (GetInputPosition(out Vector3 inputPosition))
        {
            Ray ray = _mainCamera.ScreenPointToRay(inputPosition);

            if (_gridPlane.Raycast(ray, out var hit))
            {
                Vector3 hitPoint = ray.GetPoint(hit);
                Vector2Int gridPos = ChangeGridPosition(hitPoint);

                if (_buildVM.SelectType == BuildType.Room)
                {
                    _buildVM.TryBuildRoom(gridPos);
                }
                else if (_buildVM.SelectType == BuildType.Aisle)
                {
                    _buildVM.TryBuildAisle(gridPos);
                }
            }
        }
    }

    private bool GetInputPosition (out Vector3 inputPosition)
    {
        inputPosition = Vector3.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            inputPosition = Input.mousePosition;
            return true;
        }
#else
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return false;
        }

        if (touch.phase == TouchPhase.Began)
        {
            inputPosition = touch.position;
            return true;
        }
    }
#endif

        return false;
    }

    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_buildVM.LastBuild):
                SpawnBuildPrefab(_buildVM.LastBuild).Forget();
                break;

            case nameof(_buildVM.DestroyBuild):
                if (_spawnRoom.TryGetValue(_buildVM.DestroyBuild.InstanceID, out GameObject target))
                {
                    GameObjectManager.Instance.RequestDestroyObject(target);

                    _spawnRoom.Remove(_buildVM.DestroyBuild.InstanceID);
                }
                break;
        }
    }

    private async UniTaskVoid SpawnBuildPrefab(RoomViewModel roomVM)
    {
        float worldX;
        float worldY;

        if (roomVM.BuildType == BuildType.Room)
        {
            worldX = (roomVM.OriginPos.x + roomVM.Size.x * (_cellSize / 2));
            worldY = roomVM.OriginPos.y + 2.0f;
        }
        else
        {
            worldX = (roomVM.OriginPos.x + roomVM.Size.x * (_cellSize / 2));
            worldY = roomVM.OriginPos.y;
        }

        worldX = Mathf.Round(worldX * 100f) / 100f;
        worldY = Mathf.Round(worldY * 100f) / 100f;

        Vector3 worldPos = new Vector3(worldX, worldY, 9f);

        string path = roomVM.BuildType == BuildType.Room ? "Prefabs/Room" : "Prefabs/Aisle";
        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync(roomVM.InstanceID, path, worldPos);

        _spawnRoom[roomVM.InstanceID] = prefab;

        if (prefab.TryGetComponent(out Room room))
        {
            room.Bind(roomVM);
        }
        else if (prefab.TryGetComponent(out Aisle aisle))
        {
            aisle.Bind(roomVM);
        }
    }

    private Vector2Int ChangeGridPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / _cellSize);
        int y = Mathf.FloorToInt(worldPos.y / _cellSize);

        return new Vector2Int(x, y);
    }
}