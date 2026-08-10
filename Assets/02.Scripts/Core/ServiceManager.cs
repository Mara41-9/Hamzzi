using UnityEngine;

public class ServiceManager : SingletonBase<ServiceManager>
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
