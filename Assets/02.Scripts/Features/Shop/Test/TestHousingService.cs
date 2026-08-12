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
        var housingSlotVm = new TestHousingSlotViewModel();
        long uniqueId = TestGameUtil.GenerateUniqueId();

        housingSlotVm.ItemUniqueId = uniqueId;
        housingSlotVm.IconSprite = shopSlotVm.IconSprite;
        housingSlotVm.StackCount = 1;

        var housingVm = GetHousingViewModel();
        housingVm.AddItemSlotViewModel(housingSlotVm);
    }
}
