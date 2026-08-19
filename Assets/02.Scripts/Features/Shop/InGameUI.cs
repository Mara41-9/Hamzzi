using System.ComponentModel;
using TMPro;
using UnityEngine;

public class InGameUI : ViewBase
{
    [SerializeField] private UIButton Button_OpenShopUI;
    [SerializeField] private UIButton Button_OpenHousingUI;
    [SerializeField] private UIButton Button_OpenFriendUI;
    [SerializeField] private UIButton Button_CollectionUI;
    [SerializeField] private UIButton Button_Gacha;

    [SerializeField] private TMP_Text Text_SeedCount;

    private CurrencyViewModel _currenyVm;

    private void OnEnable()
    {
        Button_OpenShopUI.BindOnClickButtonEvent(OnClick_OpenShop);
        Button_OpenHousingUI.BindOnClickButtonEvent(OnClick_OpenHousing);
        Button_OpenFriendUI.BindOnClickButtonEvent(OnClick_OpenFriend);
        Button_CollectionUI.BindOnClickButtonEvent(OnClick_OpenCollectionUI);
        Button_Gacha.BindOnClickButtonEvent(OnClick_OpenGachaUI);

        FindCurrencyViewModelAndBind();
    }

    private void OnDisable()
    {
        if(_currenyVm != null)
        {
            _currenyVm.PropertyChanged -= OnPropChanged_CurrenctView;
        }
    }

    private void FindCurrencyViewModelAndBind()
    {
        var currenyVm = ServiceManager.Instance.CurrencyService.GetCurrencyViewModel();
        _currenyVm = currenyVm;

        _currenyVm.PropertyChanged += OnPropChanged_CurrenctView;
    }

    private void OnPropChanged_CurrenctView(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CurrencyViewModel.SeedCount):
                UpdateSeedCount();
                break;
        }
    }

    private void UpdateSeedCount()
    {
        Text_SeedCount.text = _currenyVm.SeedCount.ToString();
    }

    private void OnClick_OpenShop()
    {
        UIManager.Instance.OpenShopUI();
    }

    private void OnClick_OpenHousing()
    {
        UIManager.Instance.OpenDecorUI();
    }

    private void OnClick_OpenFriend()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.FriendListUI);
    }

    private void OnClick_OpenCollectionUI()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.CollectionUI);
    }

    private void OnClick_OpenGachaUI()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.GachaUI);
    }
}
