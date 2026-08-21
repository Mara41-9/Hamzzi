// 도감에 등록된 햄스터를 맵(정원)에 동적으로 생성하는 매니저
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class HamsterManager : SingletonBase<HamsterManager>
{
    private const string HamsterPrefabAddress = "Hamster/Hamster_00";

    [SerializeField] private Vector3 _gardenSpawnRangeMin;
    [SerializeField] private Vector3 _gardenSpawnRangeMax;

    private CollectionViewModel _collectionViewModel;
    private HashSet<long> _spawnedHamsterUidSet = new HashSet<long>();

    private void Start()
    {
        _collectionViewModel = ServiceManager.Instance.CollectionService.GetCollectionViewModel();
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
            if (_spawnedHamsterUidSet.Contains(hamsterSave.HamsterUID))
            {
                continue;
            }

            _spawnedHamsterUidSet.Add(hamsterSave.HamsterUID);
            SpawnHamster(hamsterSave);
        }
    }

    private void SpawnHamster(HamsterSave hamsterSave)
    {
        SpawnHamsterAsync(hamsterSave).Forget();
    }

    private async UniTaskVoid SpawnHamsterAsync(HamsterSave hamsterSave)
    {
        Vector3 spawnSpot = GetRandomGardenSpawnPosition();

        GameObject hamsterObject = await GameObjectManager.Instance.CreateObjectAsync(hamsterSave.HamsterUID.ToString(), HamsterPrefabAddress, spawnSpot);
        if (hamsterObject == null)
        {
            return;
        }

        HamsterForm hamsterForm = hamsterObject.GetComponent<HamsterForm>();
        if (hamsterForm == null)
        {
            return;
        }

        hamsterForm.SetBodyMesh(hamsterSave.HamsterId);
        hamsterForm.SetFaceMesh(hamsterSave.FaceId);
    }

    private Vector3 GetRandomGardenSpawnPosition()
    {
        return new Vector3(
            Random.Range(_gardenSpawnRangeMin.x, _gardenSpawnRangeMax.x),
            Random.Range(_gardenSpawnRangeMin.y, _gardenSpawnRangeMax.y),
            Random.Range(_gardenSpawnRangeMin.z, _gardenSpawnRangeMax.z));
    }
}