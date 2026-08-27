using Cysharp.Threading.Tasks;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : ViewBase
{
    [Header("버튼")]
    [SerializeField] private UIButton Button_ShopUI;
    [SerializeField] private UIButton Button_DecorUI;
    [SerializeField] private UIButton Button_FriendUI;
    [SerializeField] private UIButton Button_CollectionUI;
    [SerializeField] private UIButton Button_Gacha;
    [SerializeField] private UIButton Button_Setting;
    [SerializeField] private Button Button_Garden;
    [SerializeField] private Button Button_Exit;
    [SerializeField] private UIButton Button_GoHome;
    [SerializeField] private UIButton Button_Breeding;

    [Header("DB 연동")]
    [SerializeField] private Image Image_UserIcon;
    [SerializeField] private TMP_Text Text_UserName;

    private UserViewModel _userVm;
    private VisitedUserViewModel _visitedUserVm;

    private HousingViewModel _housingVM;
    private CameraController _cameraController;

    private void Awake()
    {
        _cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void OnEnable()
    {
        Button_ShopUI.BindOnClickButtonEvent(OnClick_OpenShop);
        Button_DecorUI.BindOnClickButtonEvent(OnClick_OpenDecor);
        Button_FriendUI.BindOnClickButtonEvent(OnClick_OpenFriend);
        Button_CollectionUI.BindOnClickButtonEvent(OnClick_OpenCollectionUI);
        Button_Gacha.BindOnClickButtonEvent(OnClick_OpenGachaUI);
        Button_Setting.BindOnClickButtonEvent(OnClick_OpenSetting);
        Button_Garden.onClick.AddListener(OnClick_Garden);
        Button_Exit.onClick.AddListener(OnClick_Exit);
        Button_GoHome.BindOnClickButtonEvent(OnClick_GoHome);
        Button_Breeding.BindOnClickButtonEvent(OnClick_Breeding);

        if (_housingVM == null)
        {
            _housingVM = ServiceManager.Instance.HousingService?.GetHousingViewModel();
        }

        _housingVM.PropertyChanged += OnPropertyChanged_VM;

        FindUserViewModelAndBind();
        FindVisitedViewModelAndBind();

        UpdateButton();
    }

    private void OnDisable()
    {
        _userVm.PropertyChanged -= OnPropChanged_UserInfoView;
        _housingVM.PropertyChanged -= OnPropertyChanged_VM;
        _visitedUserVm.PropertyChanged -= OnPropChanged_VisitedUserView;
        _visitedUserVm.OnCompleteLoadInfo -= OnCompleteLoadVisitUserInfo;
    }

    private void FindUserViewModelAndBind()
    {
        var userVm = ServiceManager.Instance.UserService.GetUserViewModel();
        _userVm = userVm;

        _userVm.PropertyChanged += OnPropChanged_UserInfoView;
        _userVm.InvokeOnceOnInit();
    }

    private void FindVisitedViewModelAndBind()
    {
        var visitedVm = ServiceManager.Instance.VisitedUserService.GetViewModel();
        _visitedUserVm = visitedVm;

        _visitedUserVm.PropertyChanged += OnPropChanged_VisitedUserView;
        _visitedUserVm.OnCompleteLoadInfo += OnCompleteLoadVisitUserInfo;
    }

    private void OnPropChanged_UserInfoView(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(UserViewModel.UserName):
                UpdateUserInfo().Forget();
                break;
            case nameof(UserViewModel.UserIconId):
                UpdateUserInfo().Forget();
                break;
        }
    }

    private void OnPropChanged_VisitedUserView(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case nameof(VisitedUserViewModel.DisplayUid):
                UpdateButton();
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

    private void OnCompleteLoadVisitUserInfo()
    {
        UpdateUserInfo().Forget();
    }

    private async UniTask UpdateUserInfo()
    {
        if(_visitedUserVm == null)
        {
            return;
        }

        if(_visitedUserVm.DisplayUid == 0)
        {
            if (string.IsNullOrEmpty(_userVm.UserIconId) == false)
            {
                Sprite loadedSprite = await ResourceManager.Instance.LoadAsset<Sprite>(_userVm.UserIconId);

                Image_UserIcon.sprite = loadedSprite;
            }

            if (string.IsNullOrEmpty(_userVm.UserName) == false)
            {
                Text_UserName.text = _userVm.UserName;
            }
        }
        else
        {
            if (_visitedUserVm.DisplayUserIcon != null)
            {
                Image_UserIcon.sprite = _visitedUserVm.DisplayUserIcon;
            }

            if (string.IsNullOrEmpty(_visitedUserVm.DisplayUserName) == false)
            {
                Text_UserName.text = _visitedUserVm.DisplayUserName;
            }
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

    private void OnClick_OpenSetting()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.InGameSettingsUI);
    }

    private void OnClick_Garden()
    {
        _housingVM.TargetRoom = null;
        _housingVM.CurrentViewMode = HousingViewMode.Garden;
        _housingVM.EnterGardenMode();
        UpdateButton();
    }

    private void OnClick_Exit()
    {
        if (_cameraController.IsFollowing)
        {
            _cameraController.StopFollowHamster();
            _cameraController.ShowOverview().Forget();

            _housingVM.CurrentViewMode = HousingViewMode.OverView;

            UpdateButton();

            return;
        }

        _housingVM.TargetRoom = null;
        _housingVM.CurrentViewMode = HousingViewMode.OverView;
        _housingVM.EnterOverviewMode();
        UpdateButton();
    }

    private void OnClick_GoHome()
    {
        UIManager.Instance.OpenLoadingUI();
        UpdateButton();
    }

    private void OnClick_Breeding()
    {
        
    }

    public void UpdateButton()
    {
        if(_visitedUserVm == null)
        {
            return;
        }

        bool isVisiting = _visitedUserVm.DisplayUid != 0;
        if(isVisiting == true)
        {
            SetVisitMode(true);
            return;
        }
        else
        {
            SetVisitMode(false);
        }

        bool isFollowing = (_cameraController != null && _cameraController.IsFollowing);
        bool isSubView = (_housingVM.CurrentViewMode == HousingViewMode.Garden) || (_housingVM.TargetRoom != null) || isFollowing;

        Button_Exit.gameObject.SetActive(isSubView);
        Button_Garden.gameObject.SetActive(!isSubView);
    }

    private void SetVisitMode(bool isVisiting)
    {
        Button_Exit.gameObject.SetActive(!isVisiting);
        Button_Garden.gameObject.SetActive(!isVisiting);
        Button_ShopUI.gameObject.SetActive(!isVisiting);
        Button_DecorUI.gameObject.SetActive(!isVisiting);
        Button_Gacha.gameObject.SetActive(!isVisiting);

        Button_GoHome.gameObject.SetActive(isVisiting);
        Button_Breeding.gameObject.SetActive(isVisiting);
    }
}
