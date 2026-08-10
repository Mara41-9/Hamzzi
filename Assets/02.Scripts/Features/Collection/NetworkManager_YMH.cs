using UnityEngine;

public class NetworkManager_YMH : SingletonBase<NetworkManager_YMH>
{
    public NetworkCollectionService CollectionService { get; private set; }

    private void Start()
    {
        InitNetworkService();
        LoadData();
    }

    private void InitNetworkService()
    {
        CollectionService = new NetworkCollectionService();
        CollectionService.GetCollectionViewModel();
    }

    private void LoadData()
    {
        GameDataManager.Instance.LoadData<HamsterData>();
        CollectionService.LoadHamsterId();
    }
}