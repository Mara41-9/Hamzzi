using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum HamsterOwnerType
{
    None,
    User,
    Friend,
}

public class CrossView : ViewBase
{
    [Header("버튼")]
    [SerializeField] private UIButton ExitButton;
    [SerializeField] private UIButton MyHamsterSelectButton;
    [SerializeField] private UIButton FriendHamsterSelectButton;
    [SerializeField] private UIButton CrossButton;

    [Header("햄스터 선택")]
    [SerializeField] private CrossHamsterSelectView CrossHamsterSelectView;
    [SerializeField] private Image MyHamsterImage;
    [SerializeField] private Image FriendHamsterImage;

    public void OpenUI()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.CrossUI);
    }

    private void OnEnable()
    {
        ExitButton.BindOnClickButtonEvent(OnClickExitButton);
        MyHamsterSelectButton.BindOnClickButtonEvent(OnClickMyHamsterSelectButton);
        FriendHamsterSelectButton.BindOnClickButtonEvent(OnClickFirendHamsterSelectButton);
        CrossButton.BindOnClickButtonEvent(OnClickCrossButton);

        CrossHamsterSelectView.OnSlotSelect += OnSelectHamster;

        CrossHamsterSelectView.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ExitButton.UnBindOnClickButtonEvent(OnClickExitButton);
        MyHamsterSelectButton.UnBindOnClickButtonEvent(OnClickMyHamsterSelectButton);
        FriendHamsterSelectButton.UnBindOnClickButtonEvent(OnClickFirendHamsterSelectButton);
        CrossButton.UnBindOnClickButtonEvent(OnClickCrossButton);

        CrossHamsterSelectView.OnSlotSelect -= OnSelectHamster;
    }

    private void OnClickExitButton()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.CrossUI);
    }

    private void OnSelectHamster(string hamsterId, HamsterOwnerType ownerType)
    {
        Image iconImage = null;

        switch (ownerType)
        {
            case HamsterOwnerType.User:
                iconImage = MyHamsterImage;
                break;
            case HamsterOwnerType.Friend:
                iconImage = FriendHamsterImage;
                break;
        }

        UpdateIcon(hamsterId, iconImage).Forget();
    }

    private async UniTask UpdateIcon(string hamsterId, Image iconImage)
    {
        HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);
        if (hamsterData == null)
            return;

        var icon = await ResourceManager.Instance.LoadAsset<Sprite>(hamsterData.IconPath);
        iconImage.sprite = icon;
    }

    private void OnClickMyHamsterSelectButton()
    {
        long userUID = ServiceManager.Instance.LoginService.GetViewModel().UserUID;
        CrossHamsterSelectView.OpenSelectView(userUID, HamsterOwnerType.User);
    }

    private void OnClickFirendHamsterSelectButton()
    {
        long friendUID = ServiceManager.Instance.VisitedUserService.CurrentVisitedUid;
        CrossHamsterSelectView.OpenSelectView(friendUID, HamsterOwnerType.Friend);
    }

    private void OnClickCrossButton()
    {

    }
}
