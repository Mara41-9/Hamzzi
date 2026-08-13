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
    LobbyUI,
    LoadGameUI,
    MainTest,
    PopupTest,
    HudMainUI,
    InventoryUI,
    FarmingUI,
    StorageUI,
    NpcUI,
    MainUI,
    FarmSeedSelectUI,
    CraftUI,
    SettingUI,
    LoadingUI,
    FarmPlotStatusUI,
    GeneratorUI,
    LobbyBackgroundUI,
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
    FeverTimeCutsceneUI
}

public static class UIManagerExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        string path = string.Empty;

        path = $"UI/{uiRootType}/{uiType}";
        return path;
    }

    public static void OpenBuildUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.ContentUI, UIType.BuildUI);
    }

    public static void CloseBuildUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.ContentUI, UIType.BuildUI);
    }

    public static void OpenHousingUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.ContentUI, UIType.HousingUI);
    }

    public static void CloseHousingUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.ContentUI, UIType.HousingUI);
    }

    public static void OpenFeverTimeCutsceneUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.VeryFrontUI, UIType.FeverTimeCutsceneUI);
    }

    public static void CloseFeverTimeCutsceneUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.VeryFrontUI, UIType.FeverTimeCutsceneUI);
    }
}