using Cysharp.Threading.Tasks;
using UnityEngine;

public class ServiceManager : SingletonBase<ServiceManager>
{
    public NetworkShopService ShopService { get; private set; }
    public BuildService BuildService { get; private set; }
    public TestHousingService HousingService { get; private set; }

    public void Start()
    {
        InitShopService();
        ShopService.InitShop().Forget();

        InitBuildService();
        InitTestHousingService();
    }

    private void InitShopService()
    {
        ShopService = new NetworkShopService();
    }

    private void InitBuildService()
    {
        BuildService = new BuildService();
    }

    private void InitTestHousingService()
    {
        HousingService = new TestHousingService();
    }
}