using System.ComponentModel;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class SetPlayerNameView : UIBase
{
    [SerializeField] private TMP_InputField InputField_Name;
    [SerializeField] private UIButton Button_Confirm;
    [SerializeField] private UIButton Button_Close;

    private SetPlayerNameViewModel _vm;

    private void Start()
    {
        SetPlayerNameService service = ServiceManager.Instance.SetPlayerNameService;

        if (service != null)
        {
            BindViewModel(service.GetViewModel());
        }
    }

    public void BindViewModel(SetPlayerNameViewModel vm)
    {
        _vm = vm;

        _vm.PropertyChanged += OnPropChanged_View;
        _vm.OnCompleteSetName += OnCompleteSetName_View;
        _vm.OnFailSetName += OnFailSetName_View;

        _vm.OnCompleteSetName += OnCompleteEnter;
    }

    private void OnEnable()
    {
        Button_Confirm.BindOnClickButtonEvent(OnClickConfirm);
        Button_Close.BindOnClickButtonEvent(OnClickClose);
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

            _vm.OnCompleteSetName -= OnCompleteEnter;
        }
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

    private void OnClickClose()
    {
        UIManager.Instance.CloseSetNameUI();
    }

    private void OnCompleteSetName_View()
    {
        Debug.Log("닉네임 설정 성공");
        UIManager.Instance.CloseSetNameUI();
        UIManager.Instance.CloseTitleUI();
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.OpenInGameUI();
    }

    private void OnFailSetName_View()
    {
        Debug.Log("닉네임 설정 실패");
    }

    private void OnCompleteEnter()
    {
        LoginViewModel loginVm = ServiceManager.Instance.LoginService.GetViewModel();
        if (loginVm != null && loginVm.UserUID != 0)
        {
            GameManager.Instance.InitMap(loginVm.UserUID).Forget();
        }
    }
}