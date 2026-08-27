// 도감에 등록된 햄스터를 맵(정원)에 동적으로 생성하는 매니저
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HamsterManager : SingletonBase<HamsterManager>
{
    private const string HamsterPrefabAddress = "Hamster/Hamster_00";

    [SerializeField] private Vector3 _gardenSpawnRangeMin;
    [SerializeField] private Vector3 _gardenSpawnRangeMax;

    private CollectionViewModel _collectionViewModel;
    private Dictionary<long, GameObject> _spawnedHamsterObjectDict = new Dictionary<long, GameObject>();
    private int _collectionGeneration = 0;

    public float TotalCollectSpeedPerSec { get; private set; }

    public bool IsCurrentCollectionMine()
    {
        long myUserUid = ServiceManager.Instance.LoginService.GetViewModel().UserUID;
        CollectionViewModel myCollectionViewModel = ServiceManager.Instance.CollectionService.GetCollectionViewModel(myUserUid);

        if (myCollectionViewModel == null)
        {
            return false;
        }

        return myCollectionViewModel == ServiceManager.Instance.CollectionService.GetCurrentCollectionViewModel();
    }
    public void Init()
    {
        _collectionViewModel = ServiceManager.Instance.CollectionService.GetCurrentCollectionViewModel();
        _collectionViewModel.ContainerPropertyChanged += OnContainerPropertyChanged;
        ServiceManager.Instance.CollectionService.OnChangedCurrentCollectionViewModel += ChangedCollectionViewModel;

        SyncCollectedHamsters();
    }

    private void ChangedCollectionViewModel()
    {
        RemoveAllSpawnedHamsters();
        _collectionGeneration++;

        if (_collectionViewModel != null)
        {
            _collectionViewModel.ContainerPropertyChanged -= OnContainerPropertyChanged;
        }

        _collectionViewModel = ServiceManager.Instance.CollectionService.GetCurrentCollectionViewModel();

        if (_collectionViewModel == null)
        {
            return;
        }

        _collectionViewModel.ContainerPropertyChanged += OnContainerPropertyChanged;

        SyncCollectedHamsters();
    }

    private void OnDestroy()
    {
        if (_collectionViewModel != null)
        {
            _collectionViewModel.ContainerPropertyChanged -= OnContainerPropertyChanged;
        }
    }

    private void OnContainerPropertyChanged(string propertyName, ContainerEventType eventType, string id)
    {
        if (propertyName != nameof(_collectionViewModel.CollectedHamsterIdList))
        {
            return;
        }

        if (eventType != ContainerEventType.Add)
        {
            return;
        }

        SyncCollectedHamsters();
    }

    private void SyncCollectedHamsters()
    {
        foreach (HamsterSave hamsterSave in _collectionViewModel.CollectedHamsterList.Values)
        {
            if (_spawnedHamsterObjectDict.ContainsKey(hamsterSave.HamsterUID))
            {
                continue;
            }

            _spawnedHamsterObjectDict.Add(hamsterSave.HamsterUID, null);
            SpawnHamster(hamsterSave);
        }

        RecalculateTotalCollectSpeedPerSec();
    }

    private void SpawnHamster(HamsterSave hamsterSave)
    {
        SpawnHamsterAsync(hamsterSave, _collectionGeneration).Forget();
    }

    private async UniTaskVoid SpawnHamsterAsync(HamsterSave hamsterSave, int requestedGeneration)
    {
        Vector3 spawnSpot = GetRandomGardenSpawnPosition();

        GameObject hamsterObject = await GameObjectManager.Instance.CreateObjectAsync(hamsterSave.HamsterUID.ToString(), HamsterPrefabAddress, spawnSpot);

        if (requestedGeneration != _collectionGeneration)
        {
            if (hamsterObject != null)
            {
                GameObjectManager.Instance.RequestDestroyObject(hamsterObject);
            }

            return;
        }

        if (hamsterObject == null)
        {
            _spawnedHamsterObjectDict.Remove(hamsterSave.HamsterUID);
            return;
        }

        NavMeshAgent agent = hamsterObject.GetComponent<NavMeshAgent>();
        agent.enabled = false;

        HamsterForm hamsterForm = hamsterObject.GetComponent<HamsterForm>();
        if (hamsterForm == null)
        {
            _spawnedHamsterObjectDict.Remove(hamsterSave.HamsterUID);
            return;
        }

        hamsterForm.SetBodyMesh(hamsterSave.HamsterId);
        hamsterForm.SetFaceMesh(hamsterSave.FaceId);

        agent.enabled = true;

        _spawnedHamsterObjectDict[hamsterSave.HamsterUID] = hamsterObject;
    }

    private Vector3 GetRandomGardenSpawnPosition()
    {
        return new Vector3(
            Random.Range(_gardenSpawnRangeMin.x, _gardenSpawnRangeMax.x),
            Random.Range(_gardenSpawnRangeMin.y, _gardenSpawnRangeMax.y),
            Random.Range(_gardenSpawnRangeMin.z, _gardenSpawnRangeMax.z));
    }

    private void RemoveAllSpawnedHamsters()
    {
        foreach (GameObject hamsterObject in _spawnedHamsterObjectDict.Values)
        {
            if (hamsterObject == null)
            {
                continue;
            }

            GameObjectManager.Instance.RequestDestroyObject(hamsterObject);
        }

        _spawnedHamsterObjectDict.Clear();
    }

    private void RecalculateTotalCollectSpeedPerSec()
    {
        if (IsCurrentCollectionMine() == false)
        {
            return;
        }

        float total = 0f;

        foreach (HamsterSave hamsterSave in _collectionViewModel.CollectedHamsterList.Values)
        {
            HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterSave.HamsterId);
            if (hamsterData != null)
            {
                total += hamsterData.CollectSpeed;
            }
        }

        TotalCollectSpeedPerSec = total;
    }
}