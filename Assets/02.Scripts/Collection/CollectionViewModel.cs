using System.Collections.Generic;
using UnityEngine;

public class CollectionViewModel : ViewModelBase
{
    private HashSet<string> _allHamsterIdList = new HashSet<string>();
    public HashSet<string> AllHamsterIdList
    {
        get { return _allHamsterIdList; }
        set
        {
            if(_allHamsterIdList != value)
            {
                _allHamsterIdList = value;
                OnPropertyChanged(nameof(AllHamsterIdList));
            }
        }
    }

    // 테스트용 아이디 넣음
    private HashSet<string> _collectedHamsterIdList = new HashSet<string>() { "Hamster_01", "Hamster_03" };
    public HashSet<string> CollectedHamsterIdList
    {
        get { return _collectedHamsterIdList; }
        set
        {
            if(_collectedHamsterIdList != value)
            {
                _collectedHamsterIdList = value;
                OnPropertyChanged(nameof(CollectedHamsterIdList));
            }
        }
    }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(_allHamsterIdList));
        OnPropertyChanged(nameof(_collectedHamsterIdList));
    }
}
