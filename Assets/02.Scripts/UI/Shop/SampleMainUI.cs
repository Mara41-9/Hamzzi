using UnityEngine;

public class SampleMainUI : UIBase
{
    [SerializeField] private UIButton Button_OpenShopUI;

    private void OnEnable()
    {
        Button_OpenShopUI.BindOnClickButtonEvent(OnClick_OpenShop);
    }

    private void OnClick_OpenShop()
    {
        UIManager.Instance.OpenShopUI();
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.SampleMainUI);
    }
}
