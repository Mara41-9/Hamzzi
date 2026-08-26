using UnityEngine;

public class CrossView : ViewBase
{
    [Header("버튼")]
    [SerializeField] private UIButton ExitButton;
    [SerializeField] private UIButton MyHamsterSelectButton;
    [SerializeField] private UIButton FriendHamsterSelectButton;
    [SerializeField] private UIButton CrossButton;

    [Header("햄스터 선택")]
    [SerializeField] private CrossHamsterSelectView CrossHamsterSelectView;

    private CollectionViewModel _collectionViewModel;

    private void Start()
    {
        //_collectionViewModel = ServiceManager.Instance.CollectionService.GetCollectionViewModel();
    }

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
    }

    private void OnDisable()
    {
        ExitButton.UnBindOnClickButtonEvent(OnClickExitButton);
        MyHamsterSelectButton.UnBindOnClickButtonEvent(OnClickMyHamsterSelectButton);
        FriendHamsterSelectButton.UnBindOnClickButtonEvent(OnClickFirendHamsterSelectButton);
        CrossButton.UnBindOnClickButtonEvent(OnClickCrossButton);
    }

    private void OnClickExitButton()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.CrossUI);
    }

    private void OnClickMyHamsterSelectButton()
    {
        long userUID = ServiceManager.Instance.LoginService.GetViewModel().UserUID;
        CrossHamsterSelectView.OpenSelectView(userUID);
    }

    private void OnClickFirendHamsterSelectButton()
    {
        long userUID = ServiceManager.Instance.LoginService.GetViewModel().UserUID;
        CrossHamsterSelectView.OpenSelectView(userUID);
    }

    private void OnClickCrossButton()
    {

    }
}
