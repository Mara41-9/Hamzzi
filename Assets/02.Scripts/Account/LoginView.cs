using System.ComponentModel;
using UnityEngine;
using TMPro;

public class LoginView : UIBase
{
    [SerializeField] private TMP_InputField InputField_Id;
    [SerializeField] private TMP_InputField InputField_Password;

    [SerializeField] private UIButton Button_Login;
    [SerializeField] private UIButton Button_CreateAccount;

    [SerializeField] private TextMeshProUGUI TextMesh_Feedback;

    private LoginViewModel _vm;

    // 임시 테스트용 초기화
    private void Start()
    {
        LoginService testService = new LoginService();
        LoginViewModel testVm = new LoginViewModel();

        testVm.SetLoginService(testService);

        BindViewModel(testVm);
    }

    private void OnEnable()
    {
        Button_Login.BindOnClickButtonEvent(OnClickLogin);
        Button_CreateAccount.BindOnClickButtonEvent(OnClickCreateAccount);

        InputField_Id.onValueChanged.AddListener(OnChangeId);
        InputField_Password.onValueChanged.AddListener(OnChangePassword);
    }

    private void OnDisable()
    {
        _vm.FeedbackMessage = "";

        Button_Login.UnBindOnClickButtonEvent(OnClickLogin);
        Button_CreateAccount.UnBindOnClickButtonEvent(OnClickCreateAccount);

        InputField_Id.onValueChanged.RemoveListener(OnChangeId);
        InputField_Password.onValueChanged.RemoveListener(OnChangePassword);
    }

    private void OnDestroy()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropChanged_View;
            _vm.OnCompleteLogin -= OnCompleteLogin_View;
            _vm.OnFailLogin -= OnFailLogin_View;
            _vm.OnCompleteCreateAccount -= OnCompleteCreateAccount_View;
            _vm.OnFailCreateAccount -= OnFailCreateAccount_View;
        }
    }


    public void BindViewModel(LoginViewModel vm)
    {
        _vm = vm;

        _vm.PropertyChanged += OnPropChanged_View;
        _vm.OnCompleteLogin += OnCompleteLogin_View;
        _vm.OnFailLogin += OnFailLogin_View;
        _vm.OnCompleteCreateAccount += OnCompleteCreateAccount_View;
        _vm.OnFailCreateAccount += OnFailCreateAccount_View;

        TextMesh_Feedback.text = "";
    }

    private void OnPropChanged_View(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LoginViewModel.FeedbackMessage))
        {
            TextMesh_Feedback.text = _vm.FeedbackMessage;
        }
    }

    private void OnChangeId(string text)
    {
        if (_vm != null)
        {
            _vm.InputId = text;
        }
    }

    private void OnChangePassword(string text)
    {
        if (_vm != null)
        {
            _vm.InputPassword = text;
        }
    }

    private void OnClickLogin()
    {
        Debug.Log("aa");
        if (_vm != null)
        {
            Debug.Log("bb");
            _vm.RequestLogin();
        }
    }

    private void OnClickCreateAccount()
    {
        if (_vm != null)
        {
            _vm.RequestCreateAccount();
        }
    }

    private void OnCompleteLogin_View()
    {
        Debug.Log("로그인 성공");
    }

    private void OnFailLogin_View()
    {
        Debug.Log("로그인 실패");
    }

    private void OnCompleteCreateAccount_View()
    {
        Debug.Log("계정 생성 완료");
    }

    private void OnFailCreateAccount_View()
    {
        Debug.Log("계정 생성 실패");
    }
}