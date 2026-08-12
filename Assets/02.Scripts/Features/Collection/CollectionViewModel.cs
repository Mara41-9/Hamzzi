using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectionViewModel : ViewModelBase, IContainerPropertyChanged<string>
{
    public event Action<string, ContainerEventType, string> ContainerPropertyChanged;

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

    private string _currentSelectHamsterId = "Hamster_01";
    public string CurrentSelectHamsterId
    {
        get { return _currentSelectHamsterId; }
        set
        {
            if (_currentSelectHamsterId != value)
            {
                _currentSelectHamsterId = value;
                OnPropertyChanged(nameof(CurrentSelectHamsterId));
            }
        }
    }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(_allHamsterIdList));
        OnPropertyChanged(nameof(_collectedHamsterIdList));
        OnPropertyChanged(nameof(_currentSelectHamsterId));
    }

    public void AddCollectedHamsterIdList(string hamsterId)
    {
        if (CollectedHamsterIdList.Contains(hamsterId) == true)
            return;

        CollectedHamsterIdList.Add(hamsterId);
        ContainerPropertyChanged?.Invoke(nameof(CollectedHamsterIdList), ContainerEventType.Add, hamsterId);
        Debug.Log("햄스터 추가");
    }
}


public static class HamsterViewModelExtention 
{


    public static void RequestSelectedHamsterId(this CollectionViewModel collectionViewModel, string selectedHamsterId)
    {
        collectionViewModel.CurrentSelectHamsterId = selectedHamsterId;
    }
}