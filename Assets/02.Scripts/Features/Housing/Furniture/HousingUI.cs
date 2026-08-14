using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class HousingUI : ViewBase
{
    [SerializeField] private HousingView HousingView;

    [SerializeField] private GameObject Panel_FurnitureBar;
    [SerializeField] private GameObject Panel_Info;

    [SerializeField] private Button Button_Exit;
    [SerializeField] private Button Button_Rotation;
    [SerializeField] private Button Button_Confirm;
    [SerializeField] private Button Button_Cancel;
    [SerializeField] private Button Button_ExitMode;
    [SerializeField] private Button Button_Remove;

    private HousingViewModel _housingVM;

    private void Awake()
    {
        Button_Exit.onClick.AddListener(OnClickExit);
        Button_Rotation.onClick.AddListener(OnClickRotation);
        Button_Confirm.onClick.AddListener(OnClickConfirm);
        Button_Cancel.onClick.AddListener(OnClickCancel);
        Button_ExitMode.onClick.AddListener(OnClickExitMode);
        Button_Remove.onClick.AddListener(OnClickRemove);
    }

    private void Start()
    {
        HousingViewModel housingVM = ServiceManager.Instance.HousingService.GetHousingViewModel();
        BindViewModel(housingVM);
    }

    public void BindViewModel(HousingViewModel housingVM)
    {
        _housingVM = housingVM;
        _housingVM.PropertyChanged += OnPropertyChanged_View;

        RefreshSlots();
        UpdateState();
    }

    private void OnDestroy()
    {
        if (_housingVM != null)
        {
            _housingVM.PropertyChanged -= OnPropertyChanged_View;
        }
    }

    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_housingVM.CurrentViewMode):
                RefreshSlots();
                UpdateState();
                break;

            case nameof(_housingVM.CurrentState):
            case nameof(_housingVM.FurnitureVM):
            case nameof(_housingVM.CanConfirm):
                UpdateState();
                break;

            case nameof(_housingVM.TargetRoom):
                RefreshSlots();
                break;
        }
    }

    private void RefreshSlots()
    {
        if (_housingVM.CurrentViewMode == HousingViewMode.Garden || _housingVM.TargetRoom != null)
        {
            InitFurnitureSlot(GetDummyFurnitureList()).Forget();
        }
    }

    private void UpdateState()
    {
        if (_housingVM.CurrentState == HousingState.SelectRoom)
        {
            Panel_Info.SetActive(true);

            Panel_FurnitureBar.SetActive(false);
            Button_Exit.gameObject.SetActive(false);

            Button_Rotation.gameObject.SetActive(false);
            Button_Confirm.gameObject.SetActive(false);
            Button_Cancel.gameObject.SetActive(false);
            Button_ExitMode.gameObject.SetActive(true);
            Button_Remove.gameObject.SetActive(false);
        }
        else if (_housingVM.FurnitureVM != null)
        {
            Panel_Info.SetActive(false);

            Panel_FurnitureBar.SetActive(false);
            Button_Exit.gameObject.SetActive(false);

            Button_Rotation.gameObject.SetActive(true);
            Button_Confirm.gameObject.SetActive(true);
            Button_Cancel.gameObject.SetActive(true);
            Button_ExitMode.gameObject.SetActive(false);

            bool isEditing = _housingVM.CurrentState == HousingState.Editing;
            Button_Remove.gameObject.SetActive(isEditing);

            Button_Confirm.interactable = _housingVM.CanConfirm;
        }
        else
        {
            Panel_Info.SetActive(false);

            Panel_FurnitureBar.SetActive(true);
            Button_Exit.gameObject.SetActive(true);

            Button_Rotation.gameObject.SetActive(false);
            Button_Confirm.gameObject.SetActive(false);
            Button_Cancel.gameObject.SetActive(false);
            Button_ExitMode.gameObject.SetActive(false);
            Button_Remove.gameObject.SetActive(false);
        }
    }

    public async UniTask InitFurnitureSlot(List<ItemData> itemList)
    {
        foreach (Transform child in Panel_FurnitureBar.transform)
        {
            GameObjectManager.Instance.RequestDestroyObject(child.gameObject);
        }

        foreach (ItemData item in itemList)
        {
            GameObject slot = await GameObjectManager.Instance.CreateObjectAsync("FurnitureSlot", $"Prefabs/UI/FurnitureSlot", Vector3.zero);
            slot.transform.SetParent(Panel_FurnitureBar.transform, false);

            FurnitureSlot furnitureSlot = slot.GetComponent<FurnitureSlot>();
            furnitureSlot.Bind(item, _housingVM).Forget();
        }
    }

    private void OnClickExit()
    {
        if (_housingVM.CurrentViewMode == HousingViewMode.Garden)
        {
            _housingVM.EnterOverviewMode();
            UIManager.Instance.CloseHousingUI();
            UIManager.Instance.OpenTestUI();
        }
        else
        {
            _housingVM.ExitRoom();
        }
    }

    private void OnClickRotation()
    {
        _housingVM.RotatePos();
    }

    private void OnClickConfirm()
    {
        FurnitureViewModel confirmVM = _housingVM.FurnitureVM;

        if (_housingVM.ConfirmPos())
        {
            HousingView.SpawnFurniture(confirmVM).Forget();
        }
    }

    private void OnClickCancel()
    {
        _housingVM.CancelPos();
        HousingView.ClearGhostObject();
    }

    private void OnClickExitMode()
    {
        if (_housingVM.FurnitureVM != null)
        {
            _housingVM.CancelPos();
            HousingView.ClearGhostObject();
        }

        _housingVM.EnterOverviewMode();

        UIManager.Instance.CloseHousingUI();
        UIManager.Instance.OpenTestUI();
    }

    private void OnClickRemove()
    {
        if (_housingVM.RemoveSelectedFurniture())
        {
            HousingView.ClearGhostObject();
        }
    }

    // 테스트용
    private List<ItemData> GetDummyFurnitureList()
    {
        return ServiceManager.Instance.HousingService.GetOwnedFurnitureList();
    }
}