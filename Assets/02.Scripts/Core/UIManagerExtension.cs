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
    CollectionUI,
    HousingUI,
    TestMainUI,      // [나라] TODO : 테스트용으로 만든 메인 UI 
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
}