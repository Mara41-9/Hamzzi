using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelUI : ViewBase
{
    [SerializeField] private Button Button_Close;
    [SerializeField] private Button Button_Confirm;
    [SerializeField] private Button Button_Skip;

    [SerializeField] private Image Image_PrevHamster;
    [SerializeField] private Image Image_NextHamster;
    [SerializeField] private TextMeshProUGUI Text_PrevInfo;
    [SerializeField] private TextMeshProUGUI Text_NextInfo;
    [SerializeField] private TextMeshProUGUI Text_PrevDescription;
    [SerializeField] private TextMeshProUGUI Text_NextDescription;

    [SerializeField] private GameObject Prefab_Slot;
    [SerializeField] private Transform Parent_Hamsters;
    [SerializeField] private GameObject Text_InfoText;

    private Dictionary<string, HamsterSlot> _spawnSlotList = new Dictionary<string, HamsterSlot>();
    private WheelViewModel _wheelVM;
    private string _selectHamsterID;

    private void Start()
    {
        Button_Close.onClick.AddListener(OnClickClose);
        Button_Confirm.onClick.AddListener(OnClickConfirm);
        Button_Skip.onClick.AddListener(OnClickSkip);
    }

    private void OnEnable()
    {
        HousingViewModel housingVM = ServiceManager.Instance.HousingService.GetHousingViewModel();
        BindViewModel(new WheelViewModel(housingVM.RequestAssignHamster));
    }

    private void OnDisable()
    {
        _wheelVM.PropertyChanged -= OnPropertyChanged_VM;
    }

    public void BindViewModel(WheelViewModel wheelVM)
    {
        _wheelVM = wheelVM;
        _wheelVM.PropertyChanged += OnPropertyChanged_VM;

        _selectHamsterID = _wheelVM.CurrentHamsterID;

        InitWheelSlotList();
        RefreshInfoUI();
    }

    private void OnPropertyChanged_VM(object sender, PropertyChangedEventArgs e)
    {
        RefreshInfoUI();
    }

    private void InitWheelSlotList()
    {
        if (Parent_Hamsters.childCount > 0)
        {
            foreach (Transform child in Parent_Hamsters)
            {
                Destroy(child.gameObject);
            }

            _spawnSlotList.Clear();
        }

        foreach (WheelSlotData slotData in _wheelVM.Hamsters)
        {
            string hamsterId = slotData.HamsterID;

            GameObject prefab = Instantiate(Prefab_Slot, Parent_Hamsters);

            if (prefab.TryGetComponent<HamsterSlot>(out var hamsterSlot))
            {
                hamsterSlot.InitSlot(slotData.HamsterData, true);
                hamsterSlot.OnSlotClicked += SelectHamsterSlot;

                _spawnSlotList.Add(hamsterId, hamsterSlot);
            }
        }

        if (_spawnSlotList.Count == 0)
        {
            Text_InfoText.SetActive(true);
        }
        else
        {
            Text_InfoText.SetActive(false);
        }
    }

    private void RefreshInfoUI()
    {
        UpdatePrevHamsterInfo();
        UpdateNextHamsterInfo();

        bool hasHamster = !string.IsNullOrEmpty(_wheelVM.CurrentHamsterID);
        Button_Skip.interactable = hasHamster;
    }

    private void UpdatePrevHamsterInfo()
    {
        string currentID = _wheelVM.CurrentHamsterID;

        if (!string.IsNullOrEmpty(currentID))
        {
            HamsterData data = GameDataManager.Instance.GetData<HamsterData>(currentID);

            Text_PrevInfo.gameObject.SetActive(false);
            Text_PrevDescription.gameObject.SetActive(true);
            Text_PrevDescription.text = $"이름: {data.Name}\n해씨 수집율 {data.CollectSpeed * 100}%";

            Image_PrevHamster.gameObject.SetActive(true);
            LoadIcon(Image_PrevHamster, data.IconPath).Forget();
        }
        else
        {
            Text_PrevInfo.gameObject.SetActive(true);
            Text_PrevDescription.gameObject.SetActive(false);
            Image_PrevHamster.gameObject.SetActive(false);
        }
    }

    private void UpdateNextHamsterInfo()
    {
        if (!string.IsNullOrEmpty(_selectHamsterID))
        {
            HamsterData data = GameDataManager.Instance.GetData<HamsterData>(_selectHamsterID);

            Text_NextInfo.gameObject.SetActive(false);
            Text_NextDescription.gameObject.SetActive(true);
            Text_NextDescription.text = $"이름: {data.Name}\n해씨 수집율 {data.CollectSpeed * 100}%";

            Image_NextHamster.gameObject.SetActive(true);
            LoadIcon(Image_NextHamster, data.IconPath).Forget();
        }
        else
        {
            Text_NextInfo.gameObject.SetActive(true);
            Text_NextDescription.gameObject.SetActive(false);
            Image_NextHamster.gameObject.SetActive(false);
        }
    }

    private void SelectHamsterSlot(string hamsterID)
    {
        _selectHamsterID = hamsterID;
        UpdateNextHamsterInfo();
    }

    private async UniTask LoadIcon(Image icon, string path)
    {
        Sprite sprite = await ResourceManager.Instance.LoadAsset<Sprite>(path);
        icon.sprite = sprite;
    }

    private void OnClickClose()
    {
        HousingViewModel housingVM = ServiceManager.Instance.HousingService.GetHousingViewModel();
        housingVM.CloseAssignUI();

        UIManager.Instance.CloseWheelUI();
    }
    
    private void OnClickConfirm()
    {
        _wheelVM.AssignHamster(_selectHamsterID);

        OnClickClose();
    }

    private void OnClickSkip()
    {
        _wheelVM.UnassignHamster(_selectHamsterID);

        OnClickClose();
    }
}
