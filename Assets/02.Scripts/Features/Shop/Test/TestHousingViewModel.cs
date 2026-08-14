using System.Collections.Generic;
using UnityEngine;

public class TestHousingViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(ItemList));
    }

    private Dictionary<long, TestHousingSlotViewModel> _itemList = new Dictionary<long, TestHousingSlotViewModel>();
    public Dictionary<long, TestHousingSlotViewModel> ItemList
    {
        get => _itemList;
        set
        {
            if (_itemList != value)
            {
                _itemList = value;
                OnPropertyChanged(nameof(ItemList));
            }
        }
    }

    public void AddItemSlotViewModel(TestHousingSlotViewModel housingSlotVm)
    {
        _itemList.Add(housingSlotVm.ItemUniqueId, housingSlotVm);
        OnPropertyChanged(nameof(ItemList));
    }

}
