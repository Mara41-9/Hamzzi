using UnityEngine;

public class TitleSettingsUI : ViewBase
{
    [SerializeField] private UIButton Button_BackgroundClose;
    [SerializeField] private UIButton Button_Close;

    private void OnEnable()
    {
        Button_BackgroundClose.BindOnClickButtonEvent(OnClick_Close);
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
    }

    private void OnClick_Close()
    {
        UIManager.Instance.CloseTitleSettingsUI();
    }
}
