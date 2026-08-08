using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShopCategory
{
    None,
    All,
    Furniture,
    Play,
    Decor
}

public class ShopUI : UIBase
{
    [Header("아이템 슬롯 정보")]
    [SerializeField] private GameObject Prefab_ItemSlot;
    [SerializeField] private Transform Transform_SlotRoot;
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TMP_Text Text_ItemName;
    [SerializeField] private TMP_Text Text_ItemDescription;
    [SerializeField] private TMP_Text Text_ItemPrice;

    [Header("카테고리")]
    [SerializeField] private UIButton Button_AllCategory;
    [SerializeField] private UIButton Button_FurnitureCategory;
    [SerializeField] private UIButton Button_PlayCategory;
    [SerializeField] private UIButton Button_DecorCategory;

    private ItemData _selectedItemData;
    private ShopViewModel _shopVm;

    private ShopCategory _selectedCategory = ShopCategory.All;

    private Dictionary<long, ShopSlotUI> _itemSlotList = new Dictionary<long, ShopSlotUI>();

    private void Start()
    {
        _selectedCategory = ShopCategory.All;
        SetShopItemSlotOnEnable();
    }

    private void OnEnable()
    {
        Button_AllCategory.BindOnClickButtonEvent(OnClick_AllCategory);
        Button_FurnitureCategory.BindOnClickButtonEvent(OnClick_FurnitureCategory);
        Button_PlayCategory.BindOnClickButtonEvent(OnClick_PlayCategory);
        Button_DecorCategory.BindOnClickButtonEvent(OnClick_DecorCategory);
    }

    private void OnClick_AllCategory()
    {
        SetShopLayoutByCategory(ShopCategory.All);
    }

    private void OnClick_FurnitureCategory()
    {
        SetShopLayoutByCategory(ShopCategory.Furniture);
    }

    private void OnClick_PlayCategory()
    {
        SetShopLayoutByCategory(ShopCategory.Play);
    }

    private void OnClick_DecorCategory()
    {
        SetShopLayoutByCategory(ShopCategory.Decor);
    }

    private void SetShopLayoutByCategory(ShopCategory category)
    {
        _selectedCategory = category;

        ResetItemSlotAndCreateAll();
    }

    private void SetShopItemSlotOnEnable()
    {
        ClearSlotList();

        FindShopViewModelAndBind();
    }

    private void FindShopViewModelAndBind()
    {
        var shopVm = SampleNetworkManager.Instance.ShopService.GetShopViewModel();
        if(shopVm.ItemList == null || shopVm.ItemList.Count == 0)
        {
            Debug.LogWarning("보유한 아이템이 없습니다");
            return;
        }

        _shopVm = shopVm;
        _shopVm.PropertyChanged += OnPropChanged_ShopView;
        _shopVm.InvokeOnceOnInit();
    }

    private void OnPropChanged_ShopView(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case nameof(ShopViewModel.ItemList):
                ResetItemSlotAndCreateAll();
                break;
        }
    }

    private void ResetItemSlotAndCreateAll()
    {
        ClearSlotList();

        foreach (var itemKv in _shopVm.ItemList)
        {
            var slotVm = itemKv.Value;

            if (slotVm.Category == _selectedCategory.ToString() || _selectedCategory == ShopCategory.All)
            {
                CreateItemSlot(slotVm);
            }
        }
    }

    private void CreateItemSlot(ShopSlotViewModel slotVm)
    {
        var gObj = Instantiate(Prefab_ItemSlot, Transform_SlotRoot);
        if(gObj == null)
        {
            return;
        }

        var slotView = gObj.GetComponent<ShopSlotUI>();
        if(slotView == null)
        {
            return;
        }

        slotView.BindSlotViewModel(slotVm);

        _itemSlotList.Add(slotVm.ItemUniqueId, slotView);
        slotView.BindSlotSelectEvent(OnChildSlotSelected);
    }

    private void OnChildSlotSelected(long slotUniqueId)
    {
        if(_itemSlotList.TryGetValue(slotUniqueId, out ShopSlotUI slotView) == false)
        {
            return;
        }

        _selectedItemData = slotView.ItemData;

        Image_Icon.sprite = slotView.IconSprite;
        Text_ItemName.text = _selectedItemData.Name;
        Text_ItemDescription.text = _selectedItemData.Description;
        Text_ItemPrice.text = slotView.CostAmount.ToString();
    }

    private void ClearSlotList()
    {
        if(_itemSlotList.Count > 0)
        {
            foreach (var slotKv in _itemSlotList)
            {
                var slot = slotKv.Value;
                Destroy(slot.gameObject);
            }

            _itemSlotList.Clear();
        }
    }
    
}
