using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

public class ShopUI : MonoBehaviour
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

    private long _slotUniqueId;
    private ItemData _itemData;
    private ShopSlotUI _shopSlotUI;

    private Dictionary<long, ShopSlotUI> _itemSlotList = new Dictionary<long, ShopSlotUI>();
    private List<ItemData> _itemDataList = new List<ItemData>();

    private void Start()
    {
        TestLoadItemData();
        SetShopLayoutByCategory(ShopCategory.All);
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
        SetShopItemSlotOnEnable(category);
    }

    private void TestLoadItemData()
    {
        GameDataManager.Instance.LoadData<ItemData>();

        _itemDataList = GameDataManager.Instance.GetAllData<ItemData>();
        if (_itemDataList == null)
        {
            Debug.LogWarning("아이템 데이터가 존재하지 않습니다.");
            return;
        }

        foreach (var item in _itemDataList)
        {
            Debug.Log($"{item.Name}");
        }
    }

    private void SetShopItemSlotOnEnable(ShopCategory category)
    {
        _slotUniqueId = 0;
        ClearSlotList();

        if (_itemDataList == null || _itemDataList.Count == 0)
        {
            Debug.LogWarning("보유한 아이템이 없습니다");
            return;
        }

        foreach(var item in _itemDataList)
        {
            if(item.Category == category.ToString() || category == ShopCategory.All)
            {
                CreateItemSlot(_slotUniqueId, item.Id);
                _slotUniqueId++;
            }
        }
    }

    private void CreateItemSlot(long slotUniqueId, string itemDataId)
    {
        var gObj = Instantiate(Prefab_ItemSlot, Transform_SlotRoot);
        if(gObj == null)
        {
            return;
        }

        _shopSlotUI = gObj.GetComponent<ShopSlotUI>();
        if(_shopSlotUI == null)
        {
            return;
        }

        _shopSlotUI.InitSlot(slotUniqueId, itemDataId);

        _itemSlotList.Add(slotUniqueId, _shopSlotUI);
        _shopSlotUI.BindSlotSelectEvent(OnChildSlotSelected);
    }

    private void OnChildSlotSelected(long slotUniqueId)
    {
        if(_itemSlotList.TryGetValue(slotUniqueId, out _shopSlotUI) == false)
        {
            return;
        }

        _itemData = _shopSlotUI.ItemData;

        Image_Icon.sprite = _shopSlotUI.IconSprite;
        Text_ItemName.text = _itemData.Name;
        Text_ItemDescription.text = _itemData.Description;
    }

    private void ClearSlotList()
    {
        if(_itemSlotList.Count > 0)
        {
            foreach (var slotKv in _itemSlotList)
            {
                var slot = slotKv.Value;
                DestroyImmediate(slot.gameObject);
            }

            _itemSlotList.Clear();
        }
    }
    
}
