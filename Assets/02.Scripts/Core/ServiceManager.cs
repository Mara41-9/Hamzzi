using Cysharp.Threading.Tasks;
using UnityEngine;

public class ServiceManager : SingletonBase<ServiceManager>
{
    public NetworkShopService ShopService { get; private set; }
    public BuildService BuildService { get; private set; }

    public void Start()
    {
        InitShopService();
        ShopService.InitShop().Forget();

        InitBuildService();
    }

    private void InitShopService()
    {
        ShopService = new NetworkShopService();
    }

    private void InitBuildService()
    {
        BuildService = new BuildService();
    }
}