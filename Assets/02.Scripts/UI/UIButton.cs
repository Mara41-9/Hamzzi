using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private Button Button_Base;
    [SerializeField] private Image Image_Base;
    [SerializeField] private TMP_Text Text_Base;

    // 자동으로 이벤트를 제거할지 말지 구분하는 변수
    private bool _isSlotManualUnbindEvent;

    private void Awake()
    {
        InitUIButton();
    }

    private void OnDisable()
    {
        if (_isSlotManualUnbindEvent == false)
        {
            Button_Base.onClick.RemoveAllListeners();
        }
    }

    private void InitUIButton()
    {
        if (Button_Base != null)
        {
            return;
        }

        var button = this.gameObject.GetComponentInChildren<Button>();
        if (button != null)
        {
            this.Button_Base = button;
        }
    }

    public void BindOnClickButtonEvent(Action onClickCallback, bool isMenualUnbindEvent = false)
    {
        if (Button_Base == null) return;

        Button_Base.onClick.AddListener(new UnityEngine.Events.UnityAction(onClickCallback));
        _isSlotManualUnbindEvent = isMenualUnbindEvent;
    }

    public void UnBindOnClickButtonEvent(Action onClickCallback)
    {
        if (Button_Base == null) return;

        Button_Base.onClick.RemoveListener(new UnityEngine.Events.UnityAction(onClickCallback));
    }

}
