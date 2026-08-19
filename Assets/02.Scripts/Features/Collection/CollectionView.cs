using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CollectionView : UIBase
{
    [Header("UI Base")]
    [SerializeField] private UIButton ExitButton;

    [Header("햄스터 리스트")]
    [SerializeField] private UIButton BodyListButton;
    [SerializeField] private UIButton EyeListButton;

    [SerializeField] private GameObject HamsterSlotPrefab;
    [SerializeField] private GameObject FaceSlotPrefab;
    [SerializeField] private Transform SlotContent;

    [Header("햄스터 정보")]
    [SerializeField] private TextMeshProUGUI HamsterName;
    [SerializeField] private TextMeshProUGUI HamsterAbility1;

    private Dictionary<string, HamsterSlot> _spawnedHamsterSlotList = new Dictionary<string, HamsterSlot>();
    private Dictionary<string, FaceSlot> _spawnedFaceSlotList = new Dictionary<string, FaceSlot>();

    private CollectionViewModel _collectionViewModel;

    private void OnEnable()
    {
        ExitButton.BindOnClickButtonEvent(CloseCollectionUI);

        BodyListButton.BindOnClickButtonEvent(ShowHamsterList);
        EyeListButton.BindOnClickButtonEvent(ShowFaceList);

        // 수집 데이터들 View에 표시
        _collectionViewModel = ServiceManager.Instance.CollectionService.GetCollectionViewModel();
        _collectionViewModel.PropertyChanged += OnPropertyChanged;
        _collectionViewModel.ContainerPropertyChanged += OnContainerPropChanged;
        _collectionViewModel.InvokeOnceOnInit();
        
        // 슬롯이 없다면 초기화
        InitCollectionList();
        ShowHamsterList();

        UpdateHamsterSlot();
        UpdateFaceSlot();
    }

    private void OnDisable()
    {
        ExitButton.UnBindOnClickButtonEvent(CloseCollectionUI);

        _collectionViewModel.PropertyChanged -= OnPropertyChanged;
        _collectionViewModel.ContainerPropertyChanged -= OnContainerPropChanged;
    }

    private void CloseCollectionUI()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.CollectionUI);
    }

    private void ShowHamsterList()
    {
        ShowList(true);
    }

    private void ShowFaceList()
    {
        ShowList(false);
    }

    private void ShowList(bool isHamster)
    {
        foreach (var kv in _spawnedHamsterSlotList)
        {
            var slot = kv.Value;
            slot.gameObject.SetActive(isHamster);
        }

        foreach (var kv in _spawnedFaceSlotList)
        {
            var slot = kv.Value;
            slot.gameObject.SetActive(!isHamster);
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName) 
        {
            case nameof(CollectionViewModel.CollectedHamsterIdList):
                UpdateHamsterSlot();
                break;
            case nameof(CollectionViewModel.AllHamsterIdList):
                break;
            case nameof(CollectionViewModel.CurrentSelectHamsterId):
                UpdateHamsterInfo();
                break;
        }
    }

    private void OnContainerPropChanged(string propertyName, ContainerEventType eventType, string hamsterId)
    {
        if (propertyName == nameof(_collectionViewModel.CollectedHamsterIdList) == true)
        {
            switch (eventType)
            {
                case ContainerEventType.Add:
                    UpdateHamsterSlot();
                    break;
                case ContainerEventType.Remove:
                    UpdateHamsterSlot();
                    break;
                case ContainerEventType.Update:
                    break;
            }
        }

        if(propertyName == nameof(_collectionViewModel.CollectedFaceByHamsterList) == true)
        {
            switch (eventType)
            {
                case ContainerEventType.Add:

                    break;
                case ContainerEventType.Remove:
                    break;
                case ContainerEventType.Update:
                    break;
            }
        }
    }

    private void InitCollectionList()
    {
        InitHamsterList();
        InitFaceList();
    }

    private void InitHamsterList()
    {
        List<string> allHamsterList = _collectionViewModel.AllHamsterIdList;

        if (_spawnedHamsterSlotList.Count > 0)
            return;

        foreach (var hamsterId in allHamsterList)
        {
            GameObject hamsterSlotObject = Instantiate(HamsterSlotPrefab, SlotContent);
            if (hamsterSlotObject == null)
                return;

            HamsterSlot hamsterSlot = hamsterSlotObject.GetComponent<HamsterSlot>();
            if (hamsterSlot == null)
                return;

            HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);
            if (hamsterData == null)
                return;

            bool isCollected = _collectionViewModel.CollectedHamsterIdList.Contains(hamsterId);

            hamsterSlot.InitSlot(hamsterData, isCollected);
            hamsterSlot.OnSlotClicked += OnSelectedHamster;

            _spawnedHamsterSlotList.Add(hamsterId, hamsterSlot);
        }

        // 티어 기준으로 정렬
        SortSlotsByTier();
    }

    private void InitFaceList()
    {
        List<string> allFaceList = _collectionViewModel.AllFaceIdList;

        if (_spawnedFaceSlotList.Count > 0)
            return;

        foreach (var faceId in allFaceList)
        {
            GameObject faceSlotObject = Instantiate(FaceSlotPrefab, SlotContent);
            if (faceSlotObject == null)
                return;

            FaceSlot faceSlot = faceSlotObject.GetComponent<FaceSlot>();
            if (faceSlot == null)
                return;

            FaceData faceData = GameDataManager.Instance.GetData<FaceData>(faceId);
            if (faceData == null)
                return;

            bool isCollected = CheckUnlockFace(faceId);

            faceSlot.InitSlot(faceData, isCollected);
            faceSlot.OnSlotClicked += OnSelectedHamster;

            _spawnedFaceSlotList.Add(faceId, faceSlot);
        }
    }

    private bool CheckUnlockFace(string faceId)
    {
        string currentHamsterId = _collectionViewModel.CurrentSelectHamsterId;

        var faceList = _collectionViewModel.CollectedFaceByHamsterList;
        if (faceList.ContainsKey(currentHamsterId) == false)
        {
            return false;
        }
        return faceList[currentHamsterId].Contains(faceId);
    }

    private void SortSlotsByTier()
    {
        if (_spawnedHamsterSlotList.Count() <= 0)
            return;

        var slotList = _spawnedHamsterSlotList.Keys.ToList();
        slotList.Sort(CompareSlots);

        int index = 0;
        foreach(string hamsterId in slotList)
        {
            var slotComponent = _spawnedHamsterSlotList[hamsterId];
            if (slotComponent == null)
                continue;

            slotComponent.transform.SetSiblingIndex(index);
            index++;
        }
    }

    private int CompareSlots(string aHamsterId, string bHamsterId)
    {
        int aHamsterTier = GetHamsterTier(aHamsterId);
        int bHamsterTier = GetHamsterTier(bHamsterId);

        int tierComparison = aHamsterTier.CompareTo(bHamsterTier);

        if(tierComparison == 0)
        {
            return aHamsterId.CompareTo(bHamsterId);
        }
        return tierComparison;
    }

    private int GetHamsterTier(string hamsterId)
    {
        var hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);

        return (int)hamsterData.HamsterTier;
    }

    private void UpdateHamsterSlot()
    {
        HashSet<string> collectedHamsterList = _collectionViewModel.CollectedHamsterIdList;
        foreach(string hamsterId in collectedHamsterList)
        {
            Debug.Log($"슬롯 업데이트 {hamsterId}");
            HamsterSlot slot = _spawnedHamsterSlotList[hamsterId];
            slot.UpdateLockImage(true);
        }
    }

    private void UpdateFaceSlot()
    {
        string currentHamsterId = _collectionViewModel.CurrentSelectHamsterId;

        HashSet<string> collectedFaceList = _collectionViewModel.CollectedFaceByHamsterList[currentHamsterId];
        foreach (string faceId in collectedFaceList)
        {
            Debug.Log($"슬롯 업데이트 {faceId}");
            FaceSlot slot = _spawnedFaceSlotList[faceId];
            slot.UpdateLockImage(true);
        }
    }

    private void OnSelectedHamster(string hamsterId)
    {
        _collectionViewModel.RequestSelectedHamsterId(hamsterId);
    }

    private void UpdateHamsterInfo()
    {
        string hamsterId = _collectionViewModel.CurrentSelectHamsterId;
        HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);
        if (hamsterData == null)
            return;

        // 햄스터 이름
        HamsterName.text = hamsterData.Name;

        // 햄스터 디테일 정보
        HamsterAbility1.text = $"{hamsterData.CollectSpeed}";
    }
}


// 그냥 유니크 키가 발급되어야 할 때 사용하려고 만든 것 (의미가 있는 건 아니므로 사용만 하세요)
//public static long GenerateUniqueId()
//{
//    long newId = DateTime.UtcNow.Ticks;

//    // 원자적 연산으로 안전하게 ID 갱신
//    while (true)
//    {
//        long lastId = Volatile.Read(ref _lastId);

//        // 만약 현재 시간이 이전 ID보다 작거나 같다면 (루프가 너무 빠른 경우 포함)
//        // 이전 ID + 1로 강제 설정하여 중복 방지
//        long idToAssign = (newId <= lastId) ? lastId + 1 : newId;

//        // _lastId가 내가 읽은 시점과 같다면 idToAssign으로 교체 (성공 시 루프 탈출)
//        if (Interlocked.CompareExchange(ref _lastId, idToAssign, lastId) == lastId)
//        {
//            return idToAssign;
//        }
//        // 그 사이 다른 스레드가 값을 바꿨다면 다시 시도
//    }
//}