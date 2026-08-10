using System.ComponentModel;
using UnityEngine;
using TMPro;

public class AccountSearchView : ViewBase
{
    [SerializeField] private TMP_InputField InputField_Id;
    [SerializeField] private UIButton Button_Search;

    private AccountSearchViewModel _vm;


    // 임시 테스트용 초기화
    private void Start()
    {
        AccountSearchService testService = new AccountSearchService();
        AccountSearchViewModel testVm = new AccountSearchViewModel();

        testVm.SetSearchService(testService);

        BindViewModel(testVm);
    }

    private void OnEnable()
    {
        Button_Search.BindOnClickButtonEvent(OnClickSearch);
        InputField_Id.onValueChanged.AddListener(OnChangeId);
    }

    private void OnDisable()
    {
        InputField_Id.onValueChanged.RemoveListener(OnChangeId);
    }

    private void OnDestroy()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropChanged_View;
            _vm.OnCompleteSearch -= OnCompleteSearch_View;
            _vm.OnFailSearch -= OnFailSearch_View;
        }
    }

    public void BindViewModel(AccountSearchViewModel vm)
    {
        _vm = vm;

        _vm.PropertyChanged += OnPropChanged_View;
        _vm.OnCompleteSearch += OnCompleteSearch_View;
        _vm.OnFailSearch += OnFailSearch_View;
    }

    private void OnPropChanged_View(object sender, PropertyChangedEventArgs e)
    {
    }

    private void OnChangeId(string text)
    {
        if (_vm != null)
        {
            _vm.InputId = text;
        }
    }

    private void OnClickSearch()
    {
        if (_vm != null)
        {
            _vm.RequestSearch();
        }
    }

    private void OnCompleteSearch_View()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.AccountInfoUI);
    }

    private void OnFailSearch_View()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.SearchFailUI);
    }
}