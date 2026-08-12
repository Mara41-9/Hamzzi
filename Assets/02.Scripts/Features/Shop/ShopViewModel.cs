using System.Collections.Generic;
using UnityEngine;

public class ShopViewModel : ViewModelBase
{
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

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(ItemList));
    }

    public void AddItemSlotViewModel(ShopSlotViewModel slotVm)
    {
        _itemList.Add(slotVm.ItemUniqueId, slotVm);
        OnPropertyChanged("ItemListAdded");
    }

    public void RemoveItemSlotViewModel(long uniqueId)
    {
        if(_itemList.ContainsKey(uniqueId) == true)
        {
            _itemList.Remove(uniqueId);
        }

        OnPropertyChanged("ItemListRemoved");
    }
}
