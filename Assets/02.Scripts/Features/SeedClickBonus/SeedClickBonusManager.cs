// 주기적으로 확률을 판정해 보너스 씨앗을 랜덤 위치에 스폰하는 매니저
using UnityEngine;

public class SeedClickBonusManager : SingletonBase<SeedClickBonusManager>
{
    private const string BonusSeedAddress = "BonusSeed";
    private const float SpawnCheckIntervalSec = 2f;
    private const float SpawnProbability = 1f;

    [SerializeField] private Vector3 _spawnRangeMin;
    [SerializeField] private Vector3 _spawnRangeMax;

    private float _elapsedTime;

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime < SpawnCheckIntervalSec)
        {
            return;
        }

        _elapsedTime = 0f;

        if (Random.value < SpawnProbability)
        {
            SpawnBonusSeed();
        }
    }

    private void SpawnBonusSeed()
    {
        Vector3 spawnSpot = new Vector3(
            Random.Range(_spawnRangeMin.x, _spawnRangeMax.x),
            Random.Range(_spawnRangeMin.y, _spawnRangeMax.y),
            Random.Range(_spawnRangeMin.z, _spawnRangeMax.z));

        GameObjectManager.Instance.CreateObject(BonusSeedAddress, BonusSeedAddress, spawnSpot);
    }
}