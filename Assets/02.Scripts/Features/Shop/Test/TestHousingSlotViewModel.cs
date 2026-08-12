using UnityEngine;

public class TestHousingSlotViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(ItemUniqueId));
        OnPropertyChanged(nameof(StackCount));
        OnPropertyChanged(nameof(IconSprite));
    }

    private long _itemUniqueId;
    public long ItemUniqueId
    {
        get => _itemUniqueId;
        set
        {
            if(_itemUniqueId != value)
            {
                _itemUniqueId = value;
                OnPropertyChanged(nameof(ItemUniqueId));
            }
        }
    }

    private int _stackCount;
    public int StackCount
    {
        get => _stackCount;
        set
        {
            if (_stackCount != value)
            {
                _stackCount = value;
                OnPropertyChanged(nameof(StackCount));
            }
        }
    }

    private Sprite _iconSprite;
    public Sprite IconSprite
    {
        get => _iconSprite;
        set
        {
            if (_iconSprite != value)
            {
                _iconSprite = value;
                OnPropertyChanged(nameof(IconSprite));
            }
        }
    }
}
