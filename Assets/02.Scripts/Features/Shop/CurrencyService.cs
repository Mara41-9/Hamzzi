using Cysharp.Threading.Tasks;
using UnityEngine;

public class CurrencyService
{
    private CurrencyViewModel _currencyViewModel;

    public CurrencyViewModel GetCurrencyViewModel()
    {
        if(_currencyViewModel == null)
        {
            CreateCurrencyViewModel();
        }

        return _currencyViewModel;
    }

    private CurrencyViewModel CreateCurrencyViewModel()
    {
        var currencyVm = new CurrencyViewModel();
        _currencyViewModel = currencyVm;

        return currencyVm;
    }
 
    // 5초마다 씨앗 100개씩 모음
    public async UniTask SeedCollection()
    {
        var currencyVm = GetCurrencyViewModel();

        while (true)
        {
            await UniTask.Delay(5000);

            currencyVm.AddSeed(100);
        }
    }
}
