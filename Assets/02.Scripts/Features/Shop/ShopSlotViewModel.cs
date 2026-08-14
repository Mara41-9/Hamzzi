using UnityEngine;

public class ShopSlotViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(ItemUniqueId));
        OnPropertyChanged(nameof(ItemDataId));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(Category));
        OnPropertyChanged(nameof(CostAmount));
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

    private string _itemDataId;
    public string ItemDataId
    {
        get => _itemDataId;
        set
        {
            if (_itemDataId != value)
            {
                _itemDataId = value;
                OnPropertyChanged(nameof(ItemDataId));
            }
        }
    }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    private string _description;
    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    private string _category;
    public string Category
    {
        get => _category;
        set
        {
            if (_category != value)
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }
    }

    private string _subCategory;
    public string SubCategory
    {
        get => _subCategory;
        set
        {
            if (_subCategory != value)
            {
                _subCategory = value;
                OnPropertyChanged(nameof(SubCategory));
            }
        }
    }

    private int _costAmount;
    public int CostAmount
    {
        get => _costAmount;
        set
        {
            if (_costAmount != value)
            {
                _costAmount = value;
                OnPropertyChanged(nameof(CostAmount));
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
