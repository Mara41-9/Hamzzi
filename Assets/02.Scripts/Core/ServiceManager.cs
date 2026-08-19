using Cysharp.Threading.Tasks;
using UnityEngine;

public class ServiceManager : SingletonBase<ServiceManager>
{
    public ShopService ShopService { get; private set; }
    public BuildService BuildService { get; private set; }
    public HousingService HousingService { get; private set; }
    public CurrencyService CurrencyService { get; private set; }
    public NetworkCollectionService CollectionService { get; private set; }
    public NetworkGachaService GachaService { get; private set; }

    public void Start()
    {
        Debug.Log($"[ServiceManager Start] {GetHashCode()}");

        InitShopService();
        ShopService.InitShop().Forget();

        InitBuildService();
        InitHousingService();
        InitCollectionService();
        InitGachaService();
        InitCurrenyService();

        CurrencyService.SeedCollection().Forget();
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

    private void InitCollectionService()
    {
        CollectionService = new NetworkCollectionService();
        CollectionService.GetCollectionViewModel();
    }

    private void InitGachaService()
    {
        GachaService = new NetworkGachaService();
        GachaService.GetGachaViewModel();
    }
}