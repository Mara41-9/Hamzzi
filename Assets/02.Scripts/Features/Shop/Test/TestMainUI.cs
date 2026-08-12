using UnityEngine;

public class TestMainUI : UIBase
{
    [SerializeField] private UIButton Button_OpenShopUI;
    [SerializeField] private UIButton Button_OpenHousingUI;

    private void OnEnable()
    {
        Button_OpenShopUI.BindOnClickButtonEvent(OnClick_OpenShop);
        Button_OpenHousingUI.BindOnClickButtonEvent(OnClick_OpenHousing);
    }

    private void OnClick_OpenShop()
    {
        UIManager.Instance.OpenShopUI();
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.TestMainUI);
    }

    private void OnClick_OpenHousing()
    {
        UIManager.Instance.OpenUI(UIRootType.ContentUI, UIType.TestHousingUI);
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.TestMainUI);
    }
}
