using System.Collections.Generic;
using UnityEngine;

public class ShopViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(ItemList));
        OnPropertyChanged(nameof(SelectedSlot));
    }

    private Dictionary<long, ShopSlotViewModel> _itemList = new Dictionary<long, ShopSlotViewModel>();
    public Dictionary<long, ShopSlotViewModel> ItemList
    {
        get => _itemList;
        set
        {
            if(ItemList != value)
            {
                _itemList = value;
                OnPropertyChanged(nameof(ItemList));
            }
        }
    }

    // 현재 선택된 슬롯
    private ShopSlotViewModel _selectedSlot;
    public ShopSlotViewModel SelectedSlot
    {
        get => _selectedSlot;
        set
        {
            if(_selectedSlot != value)
            {
                _selectedSlot = value;
                OnPropertyChanged(nameof(SelectedSlot));
            }
        }
    }

    public void AddItemSlotViewModel(ShopSlotViewModel slotVm)
    {
        _itemList.Add(slotVm.ItemUniqueId, slotVm);
    }

    public void NotifyItemListChanged()
    {
        OnPropertyChanged(nameof(ItemList));
    }
}
