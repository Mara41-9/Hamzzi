using UnityEngine;

public class InGameUI : ViewBase
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
        UIManager.Instance.CloseInGameUI();
    }

    private void OnClick_OpenHousing()
    {
        UIManager.Instance.OpenTestUI();
        UIManager.Instance.CloseInGameUI();
    }
}
