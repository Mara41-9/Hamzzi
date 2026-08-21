using Cysharp.Threading.Tasks;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : ViewBase
{
    [Header("버튼")]
    [SerializeField] private UIButton Button_OpenShopUI;
    [SerializeField] private UIButton Button_OpenDecorUI;
    [SerializeField] private UIButton Button_OpenFriendUI;
    [SerializeField] private UIButton Button_CollectionUI;
    [SerializeField] private UIButton Button_Gacha;
    [SerializeField] private Button Button_Garden;
    [SerializeField] private Button Button_Exit;

    [Header("DB 연동")]
    [SerializeField] private Image Image_UserIcon;
    [SerializeField] private TMP_Text Text_UserName;

    private UserViewModel _userVm;

    private HousingViewModel _housingVM;
    private CameraController _cameraController;

    private void Awake()
    {
        _cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void OnEnable()
    {
        Button_OpenShopUI.BindOnClickButtonEvent(OnClick_OpenShop);
        Button_OpenDecorUI.BindOnClickButtonEvent(OnClick_OpenDecor);
        Button_OpenFriendUI.BindOnClickButtonEvent(OnClick_OpenFriend);
        Button_CollectionUI.BindOnClickButtonEvent(OnClick_OpenCollectionUI);
        Button_Gacha.BindOnClickButtonEvent(OnClick_OpenGachaUI);
        Button_Garden.onClick.AddListener(OnClick_Garden);
        Button_Exit.onClick.AddListener(OnClick_Exit);

        if (_housingVM == null)
        {
            _housingVM = ServiceManager.Instance.HousingService?.GetHousingViewModel();
        }

        _housingVM.PropertyChanged += OnPropertyChanged_VM;
        UpdateButton();

        FindUserViewModelAndBind();
    }

    private void OnDisable()
    {
        _userVm.PropertyChanged -= OnPropChanged_UserInfoView;
        _housingVM.PropertyChanged -= OnPropertyChanged_VM;
    }

    private void FindUserViewModelAndBind()
    {
        var userVm = ServiceManager.Instance.UserService.GetUserViewModel();
        _userVm = userVm;

        _userVm.PropertyChanged += OnPropChanged_UserInfoView;
        _userVm.InvokeOnceOnInit();
    }

    private void OnPropChanged_UserInfoView(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(UserViewModel.SeedCount):
                UpdateUserInfo().Forget();
                break;
        }
    }

    private void OnPropertyChanged_VM(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_housingVM.CurrentViewMode) || e.PropertyName == nameof(_housingVM.TargetRoom))
        {
            UpdateButton();
        }
    }

    private async UniTask UpdateUserInfo()
    {
        Sprite lodedSprite = null;

        if (string.IsNullOrEmpty(_userVm.UserIconId) == false)
        {
            lodedSprite = await ResourceManager.Instance.LoadAsset<Sprite>(_userVm.UserIconId);

            Text_UserName.text = _userVm.UserName;
            Image_UserIcon.sprite = lodedSprite;
        }
    }

    private void OnClick_OpenShop()
    {
        UIManager.Instance.OpenShopUI();
    }

    private void OnClick_OpenDecor()
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
        _housingVM.TargetRoom = null;
        _housingVM.CurrentViewMode = HousingViewMode.Garden;
        _housingVM.EnterGardenMode();
    }

    private void OnClick_Exit()
    {
        _housingVM.TargetRoom = null;
        _housingVM.CurrentViewMode = HousingViewMode.OverView;
        _housingVM.EnterOverviewMode();

        _cameraController.StopFollowHamster();
    }

    private void UpdateButton()
    {
        bool isSubView = (_housingVM.CurrentViewMode == HousingViewMode.Garden) || (_housingVM.TargetRoom != null);

        Button_Exit.gameObject.SetActive(isSubView);
        Button_Garden.gameObject.SetActive(!isSubView);
    }
}
