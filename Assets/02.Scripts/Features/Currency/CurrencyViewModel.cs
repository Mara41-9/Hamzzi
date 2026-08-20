using UnityEngine;

public class CurrencyViewModel : ViewModelBase
{
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(SeedCount));
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
