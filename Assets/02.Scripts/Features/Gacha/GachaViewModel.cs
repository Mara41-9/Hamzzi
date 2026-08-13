using System.Collections.Generic;
using UnityEngine;

public class GachaViewModel : ViewModelBase
{
    public const int SSProbability = 2;
    public const int SProbability = 13;
    public const int AProbability = 85;

    private Dictionary<HamsterTier, List<string>> _hamsterIdByTierList = new Dictionary<HamsterTier, List<string>>();
    public Dictionary<HamsterTier, List<string>> HamsterIdByTierList
    {
        get { return _hamsterIdByTierList; }
        set
        {
            if (_hamsterIdByTierList != value)
            {
                _hamsterIdByTierList = value;
                OnPropertyChanged(nameof(HamsterIdByTierList));
            }
        }
    }

    public void InvokeOnceOnInit()
    {
    }
}