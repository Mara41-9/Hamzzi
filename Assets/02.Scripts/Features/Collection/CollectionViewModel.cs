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

    // 보유 중인 햄스터의 상세 데이터 저장
    private Dictionary<int, HamsterSave> _collectedHamsterList = new Dictionary<int, HamsterSave>();
    public Dictionary<int, HamsterSave> CollectedHamsterList
    {
        get { return _collectedHamsterList; }
        set
        {
            if(_collectedHamsterList != value)
            {
                _collectedHamsterList = value;
                OnPropertyChanged(nameof(CollectedHamsterList));
            }
        }
    }

    // 보유 중인 햄스터ID만 저장
    private HashSet<string> _collectedHamsterIdList = new HashSet<string>();
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

    public void InvokeContainerPropertyChanged(string containerName, ContainerEventType type, string hamsterId )
    {
        ContainerPropertyChanged?.Invoke(containerName, type, hamsterId);
    }
}

public static class HamsterViewModelExtention 
{
    public static void AddCollectedHamsterIdList(this CollectionViewModel collectionViewModelm, string hamsterId)
    {
        if (collectionViewModelm.CollectedHamsterIdList.Contains(hamsterId) == true)
            return;

        collectionViewModelm.CollectedHamsterIdList.Add(hamsterId);
        collectionViewModelm.InvokeContainerPropertyChanged(nameof(collectionViewModelm.CollectedHamsterIdList), ContainerEventType.Add, hamsterId);

        Debug.Log("햄스터 추가");
    }

    public static void RequestSelectedHamsterId(this CollectionViewModel collectionViewModel, string selectedHamsterId)
    {
        collectionViewModel.CurrentSelectHamsterId = selectedHamsterId;
    }
}