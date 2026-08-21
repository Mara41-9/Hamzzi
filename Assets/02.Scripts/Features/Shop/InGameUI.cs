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

    [Header("DB 연동")]
    [SerializeField] private Image Image_UserIcon;
    [SerializeField] private TMP_Text Text_UserName;

    private UserViewModel _userVm;

    private void OnEnable()
    {
        Button_OpenShopUI.BindOnClickButtonEvent(OnClick_OpenShop);
        Button_OpenDecorUI.BindOnClickButtonEvent(OnClick_OpenDecor);
        Button_OpenFriendUI.BindOnClickButtonEvent(OnClick_OpenFriend);
        Button_CollectionUI.BindOnClickButtonEvent(OnClick_OpenCollectionUI);
        Button_Gacha.BindOnClickButtonEvent(OnClick_OpenGachaUI);

        FindUserViewModelAndBind();
    }

    private void OnDisable()
    {
        _userVm.PropertyChanged -= OnPropChanged_UserInfoView;
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
}
