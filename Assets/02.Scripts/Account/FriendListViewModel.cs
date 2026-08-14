using System;
using System.Collections.Generic;

public class FriendListViewModel : ViewModelBase
{
    private FriendListService _service;

    public event Action OnCompleteLoadFriendList;

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
        if (_service != null && _myUserId != "")
        {
            List<FriendInfoData> dataList = await _service.GetFriendListAsync(_myUserId);

            if (dataList != null)
            {
                FriendList = dataList;
                InvokeCompleteLoadFriendList();
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