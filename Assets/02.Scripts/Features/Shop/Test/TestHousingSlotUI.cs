using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestHousingSlotUI : ViewBase
{
    [SerializeField] private Image Image_ItemIcon;
    [SerializeField] private TMP_Text Text_StackCount;

    private TestHousingSlotViewModel _housingSlotVm;

    public void BindSlotViewModel(TestHousingSlotViewModel housingSlotVm)
    {
        _housingSlotVm = housingSlotVm;
        _housingSlotVm.PropertyChanged += OnPropChanged_View;
        _housingSlotVm.InvokeOnceOnInit();
    }

    private void OnPropChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(TestHousingSlotViewModel.IconSprite):
                if (Image_ItemIcon != null)
                {
                    Image_ItemIcon.sprite = _housingSlotVm.IconSprite;
                }
                break;
            case nameof(TestHousingSlotViewModel.StackCount):
                Text_StackCount.text = _housingSlotVm.StackCount.ToString();
                break;
        }
    }

}
