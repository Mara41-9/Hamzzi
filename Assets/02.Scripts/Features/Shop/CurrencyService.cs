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

    public void InitCurrency()
    {
        var playerModel = GameManager.Instance.PlayerModel;
        if(playerModel == null)
        {
            return;
        }

        var currencyVm = GetCurrencyViewModel();
        currencyVm.SeedCount = playerModel.SeedCount;

        SeedCollection().Forget();
    }
 
    // 5초마다 씨앗 100개씩 모음
    private async UniTask SeedCollection()
    {
        while (true)
        {
            await UniTask.Delay(5000);

            AddSeed(100);
        }
    }

    private void AddSeed(int amount)
    {
        GameManager.Instance.AddSeedCount(amount);

        var currencyVm = GetCurrencyViewModel();
        currencyVm.SeedCount = GameManager.Instance.PlayerModel.SeedCount;
    }
}
