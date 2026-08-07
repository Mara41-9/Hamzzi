using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject Prefab_ItemSlot;
    [SerializeField] private Transform Transform_SlotRoot;
    [SerializeField] private Image Image_Icon;
    [SerializeField] private TMP_Text Text_ItemName;
    [SerializeField] private TMP_Text Text_ItemDescription;
    [SerializeField] private TMP_Text Text_ItemPrice;

    private long _slotUniqueId;
    private ItemData _itemData;
    private ShopSlotUI _shopSlotUI;

    private Dictionary<long, ShopSlotUI> _itemSlotList = new Dictionary<long, ShopSlotUI>();
    private List<ItemData> _itemDataList = new List<ItemData>();

    private void Start()
    {
        TestLoadItemData();
        SetShopItemSlotOnEnable();
    }

    private void OnEnable()
    {
        
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

    private void SetShopItemSlotOnEnable()
    {
        if(_itemDataList == null || _itemDataList.Count == 0)
        {
            Debug.LogWarning("보유한 아이템이 없습니다");
            return;
        }

        foreach(var item in _itemDataList)
        {
            CreateItemSlot(_slotUniqueId, item.Id);
            _slotUniqueId++;
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
    
}
