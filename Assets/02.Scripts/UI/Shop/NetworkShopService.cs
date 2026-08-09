using UnityEngine;

public class NetworkShopService
{
    private ShopViewModel _shopViewModel;

    public ShopViewModel GetShopViewModel()
    {
        if(_shopViewModel == null)
        {
            CreateShopViewModel();
        }

        return _shopViewModel;
    }

    private ShopViewModel CreateShopViewModel()
    {
        var shopVm = new ShopViewModel();
        _shopViewModel = shopVm;
        return shopVm;
    }

    public void InitShop()
    {
        GameDataManager.Instance.LoadData<ShopData>();
        GameDataManager.Instance.LoadData<ItemData>();

        var shopDataList = GameDataManager.Instance.GetAllData<ShopData>();
        if(shopDataList == null)
        {
            Debug.LogWarning("상점 데이터가 없습니다.");
            return;
        }

        foreach(var shopData in shopDataList)
        {
            var itemData = GameDataManager.Instance.GetData<ItemData>(shopData.ItemId);
            if(itemData == null)
            {
                Debug.LogWarning("아이템 데이터가 없습니다.");
                return;
            }

            AddItem(shopData, itemData);
        }
    }

    public void AddItem(ShopData shopData, ItemData itemData)
    {
        long uniqueId = SampleGameUtil.GenerateUniqueId();

        var shopSlotVm = new ShopSlotViewModel();
        shopSlotVm.ItemUniqueId = uniqueId;
        shopSlotVm.ItemDataId = shopData.ItemId;
        shopSlotVm.Category = itemData.Category;
        shopSlotVm.CostAmount = shopData.CostAmount;

        var shopVm = GetShopViewModel();
        shopVm.AddItemSlotViewModel(shopSlotVm);
    }
}
