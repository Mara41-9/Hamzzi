using UnityEngine;

public class ServiceManager : SingletonBase<ServiceManager>
{
    public NetworkShopService ShopService { get; private set; }
    public BuildService BuildService { get; private set; }

    public void Start()
    {
        InitNetworkService();
        ShopService.InitShop();

        InitBuildService();
    }

    private void InitNetworkService()
    {
        ShopService = new NetworkShopService();
    }

    private void InitBuildService()
    {
        BuildService = new BuildService();
    }
}