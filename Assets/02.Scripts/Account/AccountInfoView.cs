using System.ComponentModel;
using UnityEngine;
using TMPro;

public class AccountInfoView : UIBase
{
    [SerializeField] private TextMeshProUGUI TextMesh_UserId;
    [SerializeField] private TextMeshProUGUI TextMesh_UserName;
    [SerializeField] private UIButton Button_Close;

    [SerializeField] private UIButton Button_AddFriend;
    [SerializeField] private UIButton Button_Visit;

    private AccountInfoViewModel _vm;



    // 임시 테스트용 초기화
    private void Start()
    {
        AccountInfoService testService = new AccountInfoService();
        FriendService testService_ = new FriendService();
        AccountInfoViewModel testVm = new AccountInfoViewModel();

        testVm.SetInfoService(testService);
        testVm.SetFriendService(testService_);

        BindViewModel(testVm);
    }

    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClickClose);
        Button_AddFriend.BindOnClickButtonEvent(OnClickAddFriend);
        Button_Visit.BindOnClickButtonEvent(OnClickVisit);

        if (_vm != null)
        {
            _vm.RequestLoadAccountInfo();
        }
    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropChanged_View;
            _vm.OnCompleteLoadInfo -= OnCompleteLoadInfo_View;

            _vm.OnCompleteAddFriend -= OnCompleteAddFriend_View;
            _vm.OnFailAddFriend -= OnFailAddFriend_View;
        }
    }

    public void BindViewModel(AccountInfoViewModel vm)
    {
        _vm = vm;

        _vm.PropertyChanged += OnPropChanged_View;
        _vm.OnCompleteLoadInfo += OnCompleteLoadInfo_View;

        _vm.OnCompleteAddFriend += OnCompleteAddFriend_View;
        _vm.OnFailAddFriend += OnFailAddFriend_View;
    }

    private void OnPropChanged_View(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AccountInfoViewModel.DisplayUserId))
        {
            TextMesh_UserId.text = _vm.DisplayUserId;
        }
        else if (e.PropertyName == nameof(AccountInfoViewModel.DisplayUserName))
        {
            TextMesh_UserName.text = _vm.DisplayUserName;
        }
    }

    private void OnClickClose()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.AccountInfoUI);
    }

    private void OnClickAddFriend()
    {
        if (_vm != null)
        {
            _vm.RequestAddFriend();
        }
    }

    private void OnClickVisit()
    {
        Debug.Log("방문하기");
    }

    private void OnCompleteLoadInfo_View()
    {
        Debug.Log("계정 정보 로드");
    }

    private void OnCompleteAddFriend_View()
    {
        Debug.Log("친구 추가 성공");
    }

    private void OnFailAddFriend_View()
    {
        Debug.Log("친구 추가 실패");
    }
}