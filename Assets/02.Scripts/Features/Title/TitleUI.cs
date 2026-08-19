using System.Runtime.CompilerServices;
using UnityEngine;

public class TitleUI : ViewBase
{
    [SerializeField] private UIButton Button_Settings;
    [SerializeField] private UIButton Button_Quit;
    [SerializeField] private UIButton Button_Login;

    private void OnEnable()
    {
        Button_Settings.BindOnClickButtonEvent(OnClick_Settings);
        Button_Quit.BindOnClickButtonEvent(OnClick_Quit);
        Button_Login.BindOnClickButtonEvent(OnClick_Login);
    }

    private void OnClick_Settings()
    {
        UIManager.Instance.OpenTitleSettingsUI();
    }

    private void OnClick_Quit()
    {

    }

    private void OnClick_Login()
    {
        UIManager.Instance.OpenLoginUI();
    }
}
