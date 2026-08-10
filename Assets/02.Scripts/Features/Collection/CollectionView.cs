using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;


public class CollectionView : UIBase
{
    [Header("UI Base")]
    [SerializeField] private Button ExitButton;

    [Header("햄스터 리스트")]
    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private Transform SlotContent;

    [Header("햄스터 정보")]
    [SerializeField] private Image HamsterIcon;
    [SerializeField] private TextMeshProUGUI HamsterName;
    [SerializeField] private TextMeshProUGUI HamsterDescription;

    [SerializeField] private TextMeshProUGUI HamsterAbility1;
    [SerializeField] private TextMeshProUGUI HamsterAbiltiy2;

    private Dictionary<string, HamsterSlot> _spawnSlotList = new Dictionary<string, HamsterSlot>();
    private CollectionViewModel _collectionViewModel;

    private void OnEnable()
    {
        ExitButton.onClick.AddListener(CloseCollectionUI);

        // 수집 데이터들 View에 표시
        _collectionViewModel = NetworkManager_YMH.Instance.CollectionService.GetCollectionViewModel();
        _collectionViewModel.PropertyChanged += OnPropertyChanged;
        _collectionViewModel.InvokeOnceOnInit();
        
        // 슬롯이 없다면 초기화
        InitCollectionList();

        // 첫번 째 슬롯으로 표시
        ShowFirstSlot();
    }

    private void OnDisable()
    {
        ExitButton.onClick.RemoveListener(CloseCollectionUI);
    }

    private void CloseCollectionUI()
    {
        // TODO : UIManager에 UIType 추가 후 주석 제거
        //UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.CollectionUI);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName) 
        {
            case nameof(CollectionViewModel.CollectedHamsterIdList):
                break;
            case nameof(CollectionViewModel.AllHamsterIdList):
                break;
        }
    }

    private void InitCollectionList()
    {
        HashSet<string> allHamsterList = _collectionViewModel.AllHamsterIdList;

        if (_spawnSlotList.Count > 0)
            return;

        foreach(var hamsterId in allHamsterList)
        {
            GameObject hamsterSlotObject = Instantiate(SlotPrefab, SlotContent);
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
            hamsterSlot.OnSlotClicked += UpdateHamsterInfo;

            _spawnSlotList.Add(hamsterId, hamsterSlot);
        }
    }

    private void UpdateHamsterInfo(string hamsterId)
    {
        HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);
        if (hamsterData == null)
            return;

        // 아이콘 로드

        // 햄스터 이름
        HamsterName.text = hamsterData.Name;
        // 햄스터 설명
        HamsterDescription.text = hamsterData.Description;

        // 햄스터 디테일 정보
        HamsterAbility1.text = $"{hamsterData.CollectSpeed}";
    }

    private void ShowFirstSlot()
    {
        string hamsterId = _spawnSlotList.Keys.First();
        UpdateHamsterInfo(hamsterId);
    }
}
