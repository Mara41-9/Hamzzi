using UnityEngine;

public class SampleNetworkManager : SingletonBase<SampleNetworkManager>
{
    public NetworkShopService ShopService { get; private set; }

    public void Start()
    {
        InitNetworkService();
        ShopService.InitShop();
    }

    private void InitNetworkService()
    {
        ShopService = new NetworkShopService();
    }
}
