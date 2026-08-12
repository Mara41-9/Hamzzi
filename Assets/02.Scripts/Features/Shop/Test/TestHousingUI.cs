using UnityEngine;

public class TestHousingUI : ViewBase
{
    [SerializeField] private UIButton Button_Close;

    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
    }

    private void OnClick_Close()
    {
        UIManager.Instance.CloseUI(UIRootType.ContentUI, UIType.TestHousingUI);
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.TestMainUI);
    }
}
