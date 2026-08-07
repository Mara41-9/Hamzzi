using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private UIButton Button_Slot;

    public long ItemSlotUniqueId { get; private set; }

    public ItemData ItemData { get; private set; }

    public Sprite IconSprite { get; private set; }

    public event Action<long> OnClickItemSlot;

    private void OnEnable()
    {
        Button_Slot.BindOnClickButtonEvent(OnClick_ItemSlot);
    }

    private void OnDisable()
    {
        Button_Slot.UnBindOnClickButtonEvent(OnClick_ItemSlot);
    }

    private void OnClick_ItemSlot()
    {
        OnClickItemSlot?.Invoke(ItemSlotUniqueId);
        Debug.Log($"{ItemSlotUniqueId} 눌러졌다   아이템명: {ItemData.Name}");
    }

    public void SetIcon(string itemDataId)
    {
        ItemData = GameDataManager.Instance.GetData<ItemData>(itemDataId);
        if(ItemData == null)
        {
            Debug.LogWarning("아이템 데이터를 찾을 수 없습니다.");
            return;
        }

        InitImage().Forget();
    }

    public async UniTask InitImage()
    {
        string iconPath = ItemData.IconPath;
        if (string.IsNullOrEmpty(iconPath) == true)
        {
            Debug.LogWarning("아이템 경로를 찾을 수 없습니다.");
            return;
        }

        var loadedSprite = await ResourceManager.Instance.LoadAsset<Sprite>(iconPath);
        if(loadedSprite == null)
        {
            Debug.LogWarning("아이템 경로에 따른 Sprite를 찾을 수 없습니다.");
            return;
        }

        IconSprite = loadedSprite;

        Image_Icon.sprite = IconSprite;
    }

    public void InitSlot(long slotUniqueId, string itemDataId)
    {
        ItemSlotUniqueId = slotUniqueId;
        SetIcon(itemDataId);
    }

    public void BindSlotSelectEvent(Action<long> onClickItemSlot)
    {
        OnClickItemSlot += onClickItemSlot;
    }
}
