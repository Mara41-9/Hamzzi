using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    }

    public void BindViewModel(BuildViewModel buildVM)
    {
        if (_buildVM != null)
        {
            _buildVM.PropertyChanged -= OnPropertyChanged_View;
        }

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

                Vector3 adjustedHitPoint = new Vector3(hitPoint.x, hitPoint.y - 2.0f, hitPoint.z);
                Vector2Int gridPos = ChangeGridPosition(adjustedHitPoint);

                if (_buildVM.SelectType == BuildType.Aisle)
                {
                    _buildVM.TryBuildAisle(gridPos);
                }
                else if (_buildVM.SelectType == BuildType.Room)
                {
                    if (_buildVM.Builds.TryGetValue(gridPos, out RoomViewModel clickedRoom) && clickedRoom.BuildType == BuildType.Room && clickedRoom.IsReady)
                    {
                        _buildVM.ChooseRoom(clickedRoom);
                    }
                    else
                    {
                        if (_buildVM.SelectRoom != null)
                        {
                            _buildVM.DeselectRoom();
                        }
                        else
                        {
                            _buildVM.TryBuildRoom(gridPos);
                        }
                    }
                }
            }
        }
    }

    private bool GetInputPosition(out Vector3 inputPosition)
    {
        inputPosition = Vector3.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Input.mousePosition;
            return true;
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return false;
            }

            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = touch.position };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
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
        Debug.Log($"BuildView가 PropertyChanged 이벤트 감지함! 속성 이름: {e.PropertyName}");

        switch (e.PropertyName)
        {
            case nameof(_buildVM.LastBuild):
                SpawnBuildPrefab(_buildVM.LastBuild).Forget();
                break;

            case nameof(_buildVM.DestroyBuild):
                Debug.Log($"DestroyBuild 이벤트 들어옴. 대상 ID: {(_buildVM.DestroyBuild != null ? _buildVM.DestroyBuild.InstanceID : "null")}");

                if (_buildVM.DestroyBuild != null)
                {
                    string targetID = _buildVM.DestroyBuild.InstanceID;
                    bool hasKey = _spawnRoom.TryGetValue(targetID, out GameObject target);
                    Debug.Log($"_spawnRoom 딕셔너리에 ID '{targetID}'가 존재함? {hasKey}");

                    if (hasKey && target != null)
                    {
                        GameObjectManager.Instance.RequestDestroyObject(target);
                        _spawnRoom.Remove(targetID);
                        Debug.Log("오브젝트 파괴 요청 성공!");
                    }
                    else
                    {
                        Debug.LogWarning("_spawnRoom에서 오브젝트를 찾지 못했습니다!");
                    }
                }
                break;

            case nameof(_buildVM.DestroyedInstanceIDs):
                Debug.Log($"DestroyedInstanceIDs 이벤트 수신함! 전달받은 ID 개수: {_buildVM.DestroyedInstanceIDs.Count}");

                foreach (string id in _buildVM.DestroyedInstanceIDs)
                {
                    if (_spawnRoom.TryGetValue(id, out GameObject obj) && obj != null)
                    {
                        GameObjectManager.Instance.RequestDestroyObject(obj);

                        _spawnRoom.Remove(id);
                        Debug.Log($"오브젝트 파괴 완료: {id}");
                    }
                    else
                    {
                        Debug.LogWarning($"경고: _spawnRoom 딕셔너리에서 ID '{id}'에 해당하는 GameObject를 찾지 못했습니다!");
                    }
                }
                break;
        }
    }

    private async UniTaskVoid SpawnBuildPrefab(RoomViewModel roomVM)
    {
        float worldX = roomVM.OriginPos.x + (roomVM.Size.x * (1f * 0.5f));
        float worldY = roomVM.OriginPos.y + (roomVM.BuildType == BuildType.Room ? 2f : 0f);

        worldX = Mathf.Round(worldX * 100f) / 100f;
        worldY = Mathf.Round(worldY * 100f) / 100f;

        Vector3 worldPos = new Vector3(worldX, worldY, 9f);

        string path = roomVM.BuildType == BuildType.Room ? "Prefabs/Room" : "Prefabs/Aisle";
        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync(roomVM.InstanceID, path, worldPos);
        Debug.Log($"프리팹 생성 완료 및 딕셔너리 등록. Key(InstanceID): {roomVM.InstanceID}, GameObject: {prefab.name}");

        _spawnRoom[roomVM.InstanceID] = prefab;

        if (prefab.TryGetComponent(out Room room))
        {
            room.Bind(roomVM);
        }
        else if (prefab.TryGetComponent(out Aisle aisle))
        {
            aisle.Bind(roomVM);
        }

        if (!_buildVM.IsLoading && !roomVM.IsDefault)
        {
            if (roomVM.BuildType == BuildType.Room)
            {
                SoundManager.Instance.PlaySFX("Build_Room");
            }
            else
            {
                SoundManager.Instance.PlaySFX("Build_Aisle", 0.1f);
            }

            prefab.transform.DOPunchScale(Vector3.one * 0.15f, 0.25f, 8, 1f);
        }
    }

    public void SpawnAllLoadBuilds()
    {
        HashSet<RoomViewModel> uniqueBuilds = new HashSet<RoomViewModel>(_buildVM.Builds.Values);

        foreach (var roomVM in uniqueBuilds)
        {
            if (roomVM != null && !_spawnRoom.ContainsKey(roomVM.InstanceID))
            {
                SpawnBuildPrefab(roomVM).Forget();
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