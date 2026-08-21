using System;
using System.Collections.Generic;

public class FriendListViewModel : ViewModelBase
{
    private FriendListService _service;

    public event Action OnCompleteLoadFriendList;

    private List<FriendInfoData> _friendList = new List<FriendInfoData>();
    public List<FriendInfoData> FriendList
    {
        get
        {
            return _friendList;
        }
        set
        {
            if (_friendList != value)
            {
                _friendList = value;
                OnPropertyChanged(nameof(FriendList));
            }
        }
    }

    public void SetService(FriendListService service)
    {
        _service = service;
    }

    public async void RequestLoadFriendList()
    {
        if (_service != null)
        {
            LoginService loginService = ServiceManager.Instance.LoginService;

            if (loginService != null)
            {
                LoginViewModel loginVm = loginService.GetViewModel();

                if (loginVm != null)
                {
                    long myUid = loginVm.UserUID;

                    if (myUid != 0)
                    {
                        List<FriendInfoData> dataList = await _service.GetFriendListAsync(myUid);

                        if (dataList != null)
                        {
                            FriendList = dataList;
                            InvokeCompleteLoadFriendList();
                        }
                    }
                }
            }
        }
    }

    private void InvokeCompleteLoadFriendList()
    {
        if (OnCompleteLoadFriendList != null)
        {
            OnCompleteLoadFriendList.Invoke();
        }
    }
}