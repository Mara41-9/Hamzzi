using UnityEngine;

public class CrossView : MonoBehaviour
{
    [SerializeField] private UIButton ExitButton;
    [SerializeField] private UIButton MyHamsterSelectButton;
    [SerializeField] private UIButton FriendHamsterSelectButton;

    [SerializeField] private UIButton CrossButton;

    private CollectionViewModel _collectionViewModel;

    private void Start()
    {
        _collectionViewModel = ServiceManager.Instance.CollectionService.GetCollectionViewModel();
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

    }

    private void OnClickFirendHamsterSelectButton()
    {

    }

    private void OnClickCrossButton()
    {

    }
}
