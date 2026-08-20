using System.ComponentModel;
using TMPro;
using UnityEngine;

public class CurrencyView : ViewBase
{
    [SerializeField] private TMP_Text Text_SeedCount;

    private CurrencyViewModel _currencyVm;

    private void OnEnable()
    {
        FindCurrencyViewModelAndBind();
    }

    private void OnDisable()
    {
        _currencyVm.PropertyChanged -= OnPropChanged_CurrenctView;
    }

    private void FindCurrencyViewModelAndBind()
    {
        var currenctVm = ServiceManager.Instance.CurrencyService.GetCurrencyViewModel();
        _currencyVm = currenctVm;

        _currencyVm.PropertyChanged += OnPropChanged_CurrenctView;
        _currencyVm.InvokeOnceOnInit();
    }

    private void OnPropChanged_CurrenctView(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CurrencyViewModel.SeedCount):
                UpdateSeedCount();
                break;
        }
    }

    private void UpdateSeedCount()
    {
        Text_SeedCount.text = _currencyVm.SeedCount.ToString();
    }
}
