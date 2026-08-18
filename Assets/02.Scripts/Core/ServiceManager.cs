using Cysharp.Threading.Tasks;
using UnityEngine;

public class ServiceManager : SingletonBase<ServiceManager>
{
    public ShopService ShopService { get; private set; }
    public BuildService BuildService { get; private set; }
    public HousingService HousingService { get; private set; }

    public void Start()
    {
        InitShopService();
        ShopService.InitShop().Forget();

        InitBuildService();
        InitHousingService();
    }

    private void InitShopService()
    {
        ShopService = new ShopService();
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