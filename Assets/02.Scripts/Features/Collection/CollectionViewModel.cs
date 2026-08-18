using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectionViewModel : ViewModelBase, IContainerPropertyChanged<string>
{
    public event Action<string, ContainerEventType, string> ContainerPropertyChanged;

    private List<string> _allHamsterIdList = new List<string>();
    public List<string> AllHamsterIdList
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

    private List<string> _allFaceIdList = new List<string>();
    public List<string> AllFaceIdList
    {
        get { return _allFaceIdList; }
        set
        {
            if(_allFaceIdList != value)
            {
                _allFaceIdList = value;
                OnPropertyChanged(nameof(AllFaceIdList));
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

    private string _currentSelectedHamsterFaceId = "Face_01";
    public string CurrentSelectedHamsterFaceId
    {
        get { return _currentSelectedHamsterFaceId; }
        set
        {
            if (_currentSelectedHamsterFaceId != value)
            {
                _currentSelectedHamsterFaceId = value;
                OnPropertyChanged(nameof(CurrentSelectedHamsterFaceId));
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
    public static void AddCollectedHamsterIdList(this CollectionViewModel collectionViewModel, string hamsterId)
    {
        if (collectionViewModel.CollectedHamsterIdList.Contains(hamsterId) == true)
            return;

        collectionViewModel.CollectedHamsterIdList.Add(hamsterId);
        collectionViewModel.InvokeContainerPropertyChanged(nameof(collectionViewModel.CollectedHamsterIdList), ContainerEventType.Add, hamsterId);

        Debug.Log("햄스터 추가");
    }

    public static void AddCollectedHamsterList(this CollectionViewModel collectionViewModel, HamsterSave hamsterSave)
    {
        if (collectionViewModel.CollectedHamsterList.ContainsKey(hamsterSave.HamsterUID) == true)
            return;

        collectionViewModel.CollectedHamsterList.Add(hamsterSave.HamsterUID, hamsterSave);
        //collectionViewModel.InvokeContainerPropertyChanged(nameof(collectionViewModel.CollectedHamsterList), ContainerEventType.Add, )
    }

    public static void RequestSelectedHamsterId(this CollectionViewModel collectionViewModel, string selectedHamsterId)
    {
        collectionViewModel.CurrentSelectHamsterId = selectedHamsterId;
    }
}