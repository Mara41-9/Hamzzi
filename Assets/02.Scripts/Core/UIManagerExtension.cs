using System;
using UnityEngine;

public enum UIRootType 
{
    None,
    BackgroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    VeryFrontUI,
}

public enum UIType
{
    TitleUI,
    TitleSettingsUI,
    InGameUI,
    ShopUI,
    BuildUI,
    LoginUI,
    AccountSearchUI,
    AccountInfoUI,
    SearchFailUI,
    SetPlayerNameUI,
    FriendListUI,
    CollectionUI,
    GachaUI,
    HousingUI,
    FeverTimeCutsceneUI,
    TestUI,  // 하우징 테스트용 (MainUI 대용)
    TestHousingUI    // [나라] TODO : 테스트용으로 만든 하우징 UI 
}

public static class UIManagerExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        string path = string.Empty;

        path = $"UI/{uiRootType}/{uiType}";
        return path;
    }

    public static void OpenTitleUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.MainUI, UIType.TitleUI);
    }

    public static void CloseTitleUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.MainUI, UIType.TitleUI);
    }

    public static void OpenTitleSettingsUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.PopupUI, UIType.TitleSettingsUI);
    }

    public static void CloseTitleSettingsUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.PopupUI, UIType.TitleSettingsUI);
    }

    public static void OpenInGameUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.MainUI, UIType.InGameUI);
    }

    public static void CloseInGameUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.MainUI, UIType.InGameUI);
    }

    public static void OpenBuildUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.ContentUI, UIType.BuildUI);
    }

    public static void CloseBuildUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.ContentUI, UIType.BuildUI);
    }

    public static void OpenShopUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.ContentUI, UIType.ShopUI);
    }

    public static void CloseShopUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.ContentUI, UIType.ShopUI);
    }

    public static void OpenFeverTimeCutsceneUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.VeryFrontUI, UIType.FeverTimeCutsceneUI);
    }

    public static void CloseFeverTimeCutsceneUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.VeryFrontUI, UIType.FeverTimeCutsceneUI);
    }

    public static void OpenTestUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.MainUI, UIType.TestUI);
    }

    public static void CloseTestUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.MainUI, UIType.TestUI);
    }

    public static void OpenHousingUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.ContentUI, UIType.HousingUI);
    }

    public static void CloseHousingUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.ContentUI, UIType.HousingUI);
    }

    public static void OpenLoginUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.PopupUI, UIType.LoginUI);
    }

    public static void CloseLoginUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.PopupUI, UIType.LoginUI);
    }

}