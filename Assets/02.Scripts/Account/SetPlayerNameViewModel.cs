using System;

public class SetPlayerNameViewModel : ViewModelBase
{
    private SetPlayerNameService _service;

    public event Action OnCompleteSetName;
    public event Action OnFailSetName;

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

    private string _inputName = "";
    public string InputName
    {
        get
        {
            return _inputName;
        }
        set
        {
            if (_inputName != value)
            {
                _inputName = value;
                OnPropertyChanged(nameof(InputName));
            }
        }
    }

    public void SetService(SetPlayerNameService service)
    {
        _service = service;
    }

    public async void RequestSetPlayerName()
    {
        if (_service != null)
        {
            bool isSuccess = await _service.TrySetPlayerNameAsync(_targetUserId, _inputName);

            if (isSuccess == true)
            {
                InvokeCompleteSetName();
            }
            else
            {
                InvokeFailSetName();
            }
        }
    }

    private void InvokeCompleteSetName()
    {
        if (OnCompleteSetName != null)
        {
            OnCompleteSetName.Invoke();
        }
    }

    private void InvokeFailSetName()
    {
        if (OnFailSetName != null)
        {
            OnFailSetName.Invoke();
        }
    }
}