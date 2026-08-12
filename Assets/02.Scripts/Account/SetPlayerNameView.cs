using System.ComponentModel;
using UnityEngine;
using TMPro;

public class SetPlayerNameView : UIBase
{
    [SerializeField] private TMP_InputField InputField_Name;
    [SerializeField] private UIButton Button_Confirm;

    private SetPlayerNameViewModel _vm;


    // 임시 테스트용 초기화
    private void Start()
    {
        SetPlayerNameService testService = new SetPlayerNameService();
        SetPlayerNameViewModel testVm = new SetPlayerNameViewModel();

        testVm.SetService(testService);

        BindViewModel(testVm);
    }


    private void OnEnable()
    {
        Button_Confirm.BindOnClickButtonEvent(OnClickConfirm);
        InputField_Name.onValueChanged.AddListener(OnChangeName);
    }

    private void OnDisable()
    {
        InputField_Name.onValueChanged.RemoveListener(OnChangeName);
    }

    private void OnDestroy()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropChanged_View;
            _vm.OnCompleteSetName -= OnCompleteSetName_View;
            _vm.OnFailSetName -= OnFailSetName_View;
        }
    }

    public void BindViewModel(SetPlayerNameViewModel vm)
    {
        _vm = vm;

        _vm.PropertyChanged += OnPropChanged_View;
        _vm.OnCompleteSetName += OnCompleteSetName_View;
        _vm.OnFailSetName += OnFailSetName_View;
    }

    private void OnPropChanged_View(object sender, PropertyChangedEventArgs e)
    {

    }

    private void OnChangeName(string text)
    {
        if (_vm != null)
        {
            _vm.InputName = text;
        }
    }

    private void OnClickConfirm()
    {
        if (_vm != null)
        {
            _vm.RequestSetPlayerName();
        }
    }

    private void OnCompleteSetName_View()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.SetPlayerNameUI);
    }

    private void OnFailSetName_View()
    {
        InputField_Name.textComponent.color = Color.red;
    }
}