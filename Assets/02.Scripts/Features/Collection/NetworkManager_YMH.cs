using UnityEngine;

public class NetworkManager_YMH : SingletonBase<NetworkManager_YMH>
{
    public NetworkCollectionService CollectionService { get; private set; }
    public NetworkGachaService GachaService { get; private set; }

    private void Start()
    {
        InitNetworkService();
    }

    private void InitNetworkService()
    {
        CollectionService = new NetworkCollectionService();
        GachaService = new NetworkGachaService();

        CollectionService.GetCollectionViewModel();
        GachaService.GetGachaViewModel();
    }
}