using UnityEngine;

public class InGameUI : ViewBase
{
    [SerializeField] private UIButton Button_OpenShopUI;
    [SerializeField] private UIButton Button_OpenHousingUI;
    [SerializeField] private UIButton Button_OpenFriendUI;

    private void OnEnable()
    {
        Button_OpenShopUI.BindOnClickButtonEvent(OnClick_OpenShop);
        Button_OpenHousingUI.BindOnClickButtonEvent(OnClick_OpenHousing);
        Button_OpenFriendUI.BindOnClickButtonEvent(OnClick_OpenFriend);
    }

    private void OnClick_OpenShop()
    {
        UIManager.Instance.OpenShopUI();
        UIManager.Instance.CloseInGameUI();
    }

    private void OnClick_OpenHousing()
    {
        UIManager.Instance.OpenUI(UIRootType.ContentUI, UIType.TestHousingUI);
        UIManager.Instance.CloseInGameUI();
    }

    private void OnClick_OpenFriend()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.FriendListUI);
    }
}
