using UnityEngine;

public class UserViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(UserName));
        OnPropertyChanged(nameof(UserIconId));
        OnPropertyChanged(nameof(SeedCount));
    }

    private string _userName;
    public string UserName
    {
        get => _userName;
        set
        {
            if (_userName != value)
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
    }

    private string _userIconId;
    public string UserIconId
    {
        get => _userIconId;
        set
        {
            if (_userIconId != value)
            {
                _userIconId = value;
                OnPropertyChanged(nameof(UserIconId));
            }
        }
    }

    private int _seedCount;
    public int SeedCount
    {
        get => _seedCount;
        set
        {
            if (_seedCount != value)
            {
                _seedCount = value;
                OnPropertyChanged(nameof(SeedCount));
            }
        }
    }
}

public static class UserViewModelExtension
{
    private static float _seedBuffRate;

    public static void AddSeed(this UserViewModel userVm, int amount)
    {
        int resultAmount = Mathf.RoundToInt(amount * (1f + _seedBuffRate));
        userVm.SeedCount += resultAmount;
    }

    public static void AddSeedBuff(this UserViewModel userVm, float amount)
    {
        _seedBuffRate += amount;
    }

    public static bool TryUseSeed(this UserViewModel userVm, int amount)
    {
        if(userVm.SeedCount < amount)
        {
            return false;
        }

        userVm.SeedCount -= amount;

        return true;
    }
}
