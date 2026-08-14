using System;

public class AccountSearchViewModel : ViewModelBase
{
    private AccountSearchService _searchService;
    
    public event Action OnCompleteSearch;
    public event Action OnFailSearch;

    private string _inputId = "";
    public string InputId
    {
        get
        {
            return _inputId;
        }
        set
        {
            if (_inputId != value)
            {
                _inputId = value;
                OnPropertyChanged(nameof(InputId));
            }
        }
    }

    public void SetSearchService(AccountSearchService service)
    {
        _searchService = service;
    }

    public async void RequestSearch()
    {
        if (_searchService != null)
        {
            bool isSuccess = await _searchService.TrySearchAccountAsync(_inputId);

            if (isSuccess == true)
            {
                InvokeCompleteSearch();
            }
            else
            {
                InvokeFailSearch();
            }
        }
    }

    private void InvokeCompleteSearch()
    {
        if (OnCompleteSearch != null)
        {
            OnCompleteSearch.Invoke();
        }
    }

    private void InvokeFailSearch()
    {
        if (OnFailSearch != null)
        {
            OnFailSearch.Invoke();
        }
    }
}