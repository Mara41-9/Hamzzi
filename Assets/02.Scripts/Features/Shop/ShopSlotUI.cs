using Cysharp.Threading.Tasks;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private UIButton Button_Slot;

    public event Action<ShopSlotViewModel> OnClickItemSlot;

    private ShopSlotViewModel _slotVm;

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
        OnClickItemSlot?.Invoke(_slotVm);
        Debug.Log($"{_slotVm.ItemUniqueId} 눌러졌다   아이템명: {_slotVm.Name}");
    }

    public void BindSlotViewModel(ShopSlotViewModel slotVm)
    {
        // 기존 바인딩 이벤트 구독 해제
        if(_slotVm != null)
        {
            _slotVm.PropertyChanged -= OnPropChanged_View;
        }

        _slotVm = slotVm;

        if(_slotVm != null)
        {
            _slotVm.PropertyChanged += OnPropChanged_View;
            _slotVm.InvokeOnceOnInit();
        }
    }

    private void OnPropChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ShopSlotViewModel.IconSprite):
                if (Image_Icon != null)
                {
                    Image_Icon.sprite = _slotVm.IconSprite;
                }
                break;
        }
    }

    public void BindSlotSelectEvent(Action<ShopSlotViewModel> onClickItemSlot)
    {
        OnClickItemSlot += onClickItemSlot;
    }
}
