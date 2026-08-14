using Cysharp.Threading.Tasks;
using UnityEngine;

public class ServiceManager : SingletonBase<ServiceManager>
{
    public NetworkShopService ShopService { get; private set; }
    public BuildService BuildService { get; private set; }
    public HousingService HousingService { get; private set; }
    public TestHousingService TestHousingService { get; private set; }

    public void Start()
    {
        InitShopService();
        ShopService.InitShop().Forget();

        InitBuildService();
        InitTestHousingService();
        InitHousingService();
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
        TestHousingService = new TestHousingService();
    }

    private void InitHousingService()
    {
        HousingService = new HousingService();
    }
}