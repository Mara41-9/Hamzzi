using System.ComponentModel;
using TMPro;
using UnityEngine;

public class CurrencyView : ViewBase
{
    [SerializeField] private TMP_Text Text_SeedCount;

    private UserViewModel _userVm;

    private void OnEnable()
    {
        FindCurrencyViewModelAndBind();
    }

    private void OnDisable()
    {
        _userVm.PropertyChanged -= OnPropChanged_CurrenctView;
    }

    private void FindCurrencyViewModelAndBind()
    {
        var userVm = ServiceManager.Instance.UserService.GetUserViewModel();
        _userVm = userVm;

        _userVm.PropertyChanged += OnPropChanged_CurrenctView;
        _userVm.InvokeOnceOnInit();
    }

    private void OnPropChanged_CurrenctView(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(UserViewModel.SeedCount):
                UpdateSeedCount();
                break;
        }
    }

    private void UpdateSeedCount()
    {
        Text_SeedCount.text = _userVm.SeedCount.ToString();
    }
}
