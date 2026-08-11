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

    [Header("상위 카테고리")]
    [SerializeField] private UIButton Button_AllCategory;
    [SerializeField] private UIButton Button_FurnitureCategory;
    [SerializeField] private UIButton Button_PlayCategory;
    [SerializeField] private UIButton Button_DecorCategory;

    [Header("카테고리 선택 이미지")]
    [SerializeField] private Image Image_SelectedAllCategory;
    [SerializeField] private Image Image_SelectedFurnitureCategory;
    [SerializeField] private Image Image_SelectedPlayCategory;
    [SerializeField] private Image Image_SelectedDecorCategory;

    [Header("하위 카테고리")]
    [SerializeField] private GameObject SubCategoryArea;
    [SerializeField] private GameObject Button_SubCategory;
    [SerializeField] private Transform Transform_ButtonRoot;

    private ItemData _selectedItemData;
    private ShopViewModel _shopVm;
    private string _selectedSubCategory;

    private ShopCategory _selectedCategory = ShopCategory.All;

    private Dictionary<long, ShopSlotUI> _itemSlotList = new Dictionary<long, ShopSlotUI>();
    private List<string> _subCategoryNameList = new List<string>();
    private List<ShopSubCategoryUI> _subCategoryButtonList = new List<ShopSubCategoryUI>();

    private void Start()
    {
        _selectedCategory = ShopCategory.All;
        SetSelectedCategory(_selectedCategory);
        SetSubCategory(_selectedCategory);
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
        Image_SelectedAllCategory.gameObject.SetActive(true);
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
        SetSelectedCategory(_selectedCategory);
        SetSubCategory(category);

        ResetItemSlotAndCreateAll();
    }
    private void SetSelectedCategory(ShopCategory category)
    {
        Image_SelectedAllCategory.gameObject.SetActive(category == ShopCategory.All);
        Image_SelectedFurnitureCategory.gameObject.SetActive(category == ShopCategory.Furniture);
        Image_SelectedPlayCategory.gameObject.SetActive(category == ShopCategory.Play);
        Image_SelectedDecorCategory.gameObject.SetActive(category == ShopCategory.Decor);
    }

    private void SetSubCategory(ShopCategory category)
    {
        if(category == ShopCategory.All)
        {
            SubCategoryArea.SetActive(false);
        }
        else
        {
            SubCategoryArea.SetActive(true);
            CreateSubCategoryButton(category);
        }
    }

    private void CreateSubCategoryButton(ShopCategory category)
    {
        ClearSubCategoryList();

        foreach (var itemKv in _shopVm.ItemList)
        {
            var slotVm = itemKv.Value;

            if (slotVm.Category != category.ToString())
            {
                continue;
            }

            if (_subCategoryNameList.Contains(slotVm.SubCategory))
            {
                continue;
            }

            _subCategoryNameList.Add(slotVm.SubCategory);
        }

        foreach(var subCategory in _subCategoryNameList)
        {
            var gObj = Instantiate(Button_SubCategory, Transform_ButtonRoot);
            if(gObj == null)
            {
                return;
            }

            var component = gObj.GetComponent<ShopSubCategoryUI>();
            if(component == null)
            {
                return;
            }

            component.SetSubCategory(subCategory);
            component.BindSubCategorySelectEvent(OnSubCategorySelected);
            _subCategoryButtonList.Add(component);
        }

        if (_subCategoryNameList.Count > 0)
        {
            _selectedSubCategory = _subCategoryNameList[0];
            _subCategoryButtonList[0].SetSelected(true) ;
        }
    }

    private void SetShopItemSlotOnEnable()
    {
        ClearSlotList();

        FindShopViewModelAndBind();
    }

    private void FindShopViewModelAndBind()
    {
        var shopVm = ServiceManager.Instance.ShopService.GetShopViewModel();
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

    private void OnSubCategorySelected(string subCategory)
    {
        _selectedSubCategory = subCategory;

        for (int i = 0; i < _subCategoryButtonList.Count; i++)
        {
            ShopSubCategoryUI subCategoryUI = _subCategoryButtonList[i];

            if (subCategoryUI.SubCategory == _selectedSubCategory)
            {
                subCategoryUI.SetSelected(true);
            }
            else
            {
                subCategoryUI.SetSelected(false);
            }
        }

        ResetItemSlotAndCreateAll();
    }

    private void ResetItemSlotAndCreateAll()
    {
        ClearSlotList();

        foreach (var itemKv in _shopVm.ItemList)
        {
            var slotVm = itemKv.Value;

            if (_selectedCategory == ShopCategory.All)
            {
                CreateItemSlot(slotVm);
                continue;
            }

            if(slotVm.Category != _selectedCategory.ToString())
            {
                continue;
            }

            if(slotVm.SubCategory == _selectedSubCategory)
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

    private void ClearSubCategoryList()
    {
        if (_subCategoryButtonList.Count > 0)
        {
            foreach (var buttonObj in _subCategoryButtonList)
            {
                Destroy(buttonObj.gameObject);
            }

            _subCategoryButtonList.Clear();
        }

        _subCategoryNameList.Clear();
    }
}
