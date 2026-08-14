using System;

public class AccountInfoViewModel : ViewModelBase
{
    private AccountInfoService _infoService;
    private FriendService _friendService;

    public event Action OnCompleteLoadInfo;

    public event Action OnCompleteAddFriend;
    public event Action OnFailAddFriend;

    private string _myUserId = "";
    public string MyUserId
    {
        get
        {
            return _myUserId;
        }
        set
        {
            if (_myUserId != value)
            {
                _myUserId = value;
                OnPropertyChanged(nameof(MyUserId));
            }
        }
    }

    private string _targetUserId = "";
    public string TargetUserId
    {
        get
        {
            return _targetUserId;
        }
        set
        {
            if (_targetUserId != value)
            {
                _targetUserId = value;
                OnPropertyChanged(nameof(TargetUserId));
            }
        }
    }

    private string _displayUserId = "";
    public string DisplayUserId
    {
        get
        {
            return _displayUserId;
        }
        set
        {
            if (_displayUserId != value)
            {
                _displayUserId = value;
                OnPropertyChanged(nameof(DisplayUserId));
            }
        }
    }

    private string _displayUserName = "";
    public string DisplayUserName
    {
        get
        {
            return _displayUserName;
        }
        set
        {
            if (_displayUserName != value)
            {
                _displayUserName = value;
                OnPropertyChanged(nameof(DisplayUserName));
            }
        }
    }

    public void SetInfoService(AccountInfoService service)
    {
        _infoService = service;
    }

    public void SetFriendService(FriendService service)
    {
        _friendService = service;
    }

    public async void RequestLoadAccountInfo()
    {
        if (_infoService != null && _targetUserId != "")
        {
            AccountInfoData data = await _infoService.GetAccountInfoAsync(_targetUserId);

            if (data != null)
            {
                DisplayUserId = data.UserId;
                DisplayUserName = data.UserName;
                InvokeCompleteLoadInfo();
            }
        }
    }

    public async void RequestAddFriend()
    {
        if (_friendService != null && _myUserId != "" && _targetUserId != "")
        {
            bool isSuccess = await _friendService.TryAddFriendAsync(_myUserId, _targetUserId);

            if (isSuccess == true)
            {
                InvokeCompleteAddFriend();
            }
            else
            {
                InvokeFailAddFriend();
            }
        }
    }

    private void InvokeCompleteLoadInfo()
    {
        if (OnCompleteLoadInfo != null)
        {
            OnCompleteLoadInfo.Invoke();
        }
    }

    private void InvokeCompleteAddFriend()
    {
        if (OnCompleteAddFriend != null)
        {
            OnCompleteAddFriend.Invoke();
        }
    }

    private void InvokeFailAddFriend()
    {
        if (OnFailAddFriend != null)
        {
            OnFailAddFriend.Invoke();
        }
    }
}