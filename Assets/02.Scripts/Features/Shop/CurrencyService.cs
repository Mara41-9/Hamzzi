using Cysharp.Threading.Tasks;
using UnityEngine;

public class CurrencyService
{
    private CurrencyViewModel _currencyViewModel;
    private float _seedBuffRate;

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

            int baseAmount = 100;

            // 버프 적용
            int resultAmount = Mathf.RoundToInt(baseAmount * (1f + _seedBuffRate));
            AddSeed(resultAmount);
        }
    }

    private void AddSeed(int amount)
    {
        GameManager.Instance.AddSeedCount(amount);

        var currencyVm = GetCurrencyViewModel();
        currencyVm.SeedCount = GameManager.Instance.PlayerModel.SeedCount;
    }

    public void AddSeedBuff(float amount)
    {
        _seedBuffRate += amount;
    }

    public bool TryUseSeed(int amount)
    {
        var playerModel = GameManager.Instance.PlayerModel;
        if(playerModel.SeedCount < 0 || playerModel.SeedCount < amount)
        {
            return false;
        }

        playerModel.SeedCount -= amount;

        var currencyVm = GetCurrencyViewModel();
        currencyVm.SeedCount = playerModel.SeedCount;

        return true;
    }
}
