using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BuildView : ViewBase
{
    [SerializeField] private List<Vector2Int> Transform_DefaultRoom;
    [SerializeField] private List<Vector2Int> Transform_DefaultAisle;

    private float _cellSize = 1.0f;
    private Camera _mainCamera;

    private BuildViewModel _buildVM;
    private Plane _gridPlane = new Plane(Vector3.forward, new Vector3(0, 0, 9f));

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    // 임시
    private void Start()
    {
        BindViewModel(new BuildViewModel());

        _buildVM.InitDefaultRoom(Transform_DefaultRoom, Transform_DefaultAisle);
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
        // UI 구현 전 테스트용
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _buildVM.SelectType = BuildType.Room;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _buildVM.SelectType = BuildType.Aisle;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Alpha0))
        {
            _buildVM.SelectType = BuildType.None;
        }

        if (_buildVM.SelectType != BuildType.None && Input.GetMouseButtonDown(0))
        {
            BuildRoom();
        }
    }

    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(BuildViewModel.LastBuild):
                SpawnBuildPrefab(_buildVM.LastBuild).Forget();
                break;
        }
    }

    private async UniTaskVoid SpawnBuildPrefab(RoomViewModel roomVM)
    {
        float worldX = 0f;
        float worldY = 0f;

        if (roomVM.BuildType == BuildType.Room)
        {
            worldX = (roomVM.OriginPos.x + roomVM.Size.x * 0.5f) * _cellSize;
            worldY = (roomVM.OriginPos.y + roomVM.Size.y * 0.5f) * _cellSize;
        }
        else
        {
            worldX = (roomVM.OriginPos.x + 0.5f) * _cellSize;
            worldY = (roomVM.OriginPos.y + 0.5f) * _cellSize;
        }

        worldX = Mathf.Round(worldX * 100f) / 100f;
        worldY = Mathf.Round(worldY * 100f) / 100f;

        Vector3 worldPos = new Vector3(worldX, worldY, 9f);

        string path = roomVM.BuildType == BuildType.Room ? "Prefabs/Room" : "Prefabs/Aisle";
        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync(roomVM.InstanceID, path, worldPos);

        if (prefab.TryGetComponent(out Room room))
        {
            room.Bind(roomVM);
        }
        else if (prefab.TryGetComponent(out Aisle aisle))
        {
            aisle.Bind(roomVM);
        }
    }

    private void BuildRoom()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (_gridPlane.Raycast(ray, out float hit))
        {
            Vector3 hitPoint = ray.GetPoint(hit);
            Vector2Int gridPos = ChangeGridPosition(hitPoint);

            switch (_buildVM.SelectType)
            {
                case BuildType.Room:
                    _buildVM.TryBuildRoom(gridPos);
                    break;

                case BuildType.Aisle:
                    _buildVM.TryBuildAisle(gridPos);
                    break;
            }
        }
    }

    private Vector2Int ChangeGridPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / _cellSize);
        int y = Mathf.FloorToInt(worldPos.y / _cellSize);

        return new Vector2Int(x, y);
    }
}