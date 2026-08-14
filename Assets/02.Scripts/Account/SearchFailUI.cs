using TMPro;
using UnityEngine;

public class SearchFailUI : UIBase
{
    [SerializeField] private UIButton Button_Exit;

    private void OnEnable()
    {
        Button_Exit.BindOnClickButtonEvent(OnClickExit);
    }

    private void OnDisable()
    {
        Button_Exit.UnBindOnClickButtonEvent(OnClickExit);
    }

    private void OnClickExit()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.SearchFailUI);
    }
}
