using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : ViewBase
{
    [SerializeField] private UIButton Button_OpenShopUI;
    [SerializeField] private UIButton Button_OpenHousingUI;
    [SerializeField] private UIButton Button_OpenFriendUI;
    [SerializeField] private UIButton Button_CollectionUI;
    [SerializeField] private UIButton Button_Gacha;
    [SerializeField] private Button Button_Garden;
    [SerializeField] private Button Button_Exit;

    private HousingViewModel _housingVM;

    private void OnEnable()
    {
        Button_OpenShopUI.BindOnClickButtonEvent(OnClick_OpenShop);
        Button_OpenHousingUI.BindOnClickButtonEvent(OnClick_OpenHousing);
        Button_OpenFriendUI.BindOnClickButtonEvent(OnClick_OpenFriend);
        Button_CollectionUI.BindOnClickButtonEvent(OnClick_OpenCollectionUI);
        Button_Gacha.BindOnClickButtonEvent(OnClick_OpenGachaUI);
        Button_Garden.onClick.AddListener(OnClick_Garden);
        Button_Exit.onClick.AddListener(OnClick_Exit);
    }

    private void Start()
    {
        _housingVM = ServiceManager.Instance.HousingService?.GetHousingViewModel();
        _housingVM.PropertyChanged += OnPropertyChanged_VM;
        UpdateExitButton();
    }

    private void OnDisable()
    {
        _housingVM.PropertyChanged -= OnPropertyChanged_VM;
    }

    private void OnPropertyChanged_VM(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_housingVM.CurrentViewMode) || e.PropertyName == nameof(_housingVM.TargetRoom))
        {
            UpdateExitButton();
        }
    }

    private void OnClick_OpenShop()
    {
        UIManager.Instance.OpenShopUI();
    }

    private void OnClick_OpenHousing()
    {
        UIManager.Instance.OpenDecorUI();
        UIManager.Instance.CloseInGameUI();
    }

    private void OnClick_OpenFriend()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.FriendListUI);
    }

    private void OnClick_OpenCollectionUI()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.CollectionUI);
    }

    private void OnClick_OpenGachaUI()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.GachaUI);
    }

    private void OnClick_Garden()
    {
        HousingViewModel housingVM = ServiceManager.Instance.HousingService.GetHousingViewModel();
        housingVM.CurrentViewMode = HousingViewMode.Garden;
    }

    private void OnClick_Exit()
    {
        HousingViewModel housingVM = ServiceManager.Instance.HousingService?.GetHousingViewModel();

        housingVM.TargetRoom = null;
        housingVM.CurrentViewMode = HousingViewMode.OverView;
    }

    private void UpdateExitButton()
    {
        bool isSubView = (_housingVM.CurrentViewMode == HousingViewMode.Garden) || (_housingVM.TargetRoom != null);
        Button_Exit.gameObject.SetActive(isSubView);
    }
}
