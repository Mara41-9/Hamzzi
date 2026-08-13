using UnityEngine;

public class ServiceManager : SingletonBase<ServiceManager>
{
    public NetworkShopService ShopService { get; private set; }
    public BuildService BuildService { get; private set; }
    public HousingService HousingService { get; private set; }

    public void Start()
    {
        InitNetworkService();
        ShopService.InitShop();

        InitBuildService();
        InitHousingService();
    }

    private void InitNetworkService()
    {
        ShopService = new NetworkShopService();
    }

    private void InitBuildService()
    {
        BuildService = new BuildService();
    }

    private void InitHousingService()
    {
        HousingService = new HousingService();
    }
}