using UnityEngine;

public class TestHousingService
{
    private TestHousingViewModel _housingViewModel;

    public TestHousingViewModel GetHousingViewModel()
    {
        if (_housingViewModel == null)
        {
            CreateHousingViewModel();
        }

        return _housingViewModel;
    }

    private TestHousingViewModel CreateHousingViewModel()
    {
        var housingVm = new TestHousingViewModel();
        _housingViewModel = housingVm;
        return housingVm;
    }

    public void AddItem(ShopSlotViewModel shopSlotVm)
    {
        var housingVm = GetHousingViewModel();
        
        foreach(var itemKv in housingVm.ItemList)
        {
            var housingSlotVm = itemKv.Value;
            if(housingSlotVm.ItemDataId == shopSlotVm.ItemDataId)
            {
                housingSlotVm.StackCount++;
                return;
            }
        }

        var newhousingSlotVm = new TestHousingSlotViewModel();
        long uniqueId = TestGameUtil.GenerateUniqueId();

        newhousingSlotVm.ItemUniqueId = uniqueId;
        newhousingSlotVm.ItemDataId = shopSlotVm.ItemDataId;
        newhousingSlotVm.IconSprite = shopSlotVm.IconSprite;
        newhousingSlotVm.StackCount = 1;
        
        housingVm.AddItemSlotViewModel(newhousingSlotVm);
    }
}
