using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum HousingCategory
{
    All,
    Furniture,
    Toy,
    Decor
}

public class HousingUI : ViewBase
{
    [SerializeField] private GameObject Panel_FurnitureBar;
    [SerializeField] private GameObject Panel_Info;
    [SerializeField] private Transform Parent_Slot;
    [SerializeField] private TextMeshProUGUI Text_EmptyInfo;

    [SerializeField] private Button Button_All;
    [SerializeField] private Button Button_Furniture;
    [SerializeField] private Button Button_Toy;
    [SerializeField] private Button Button_Decor;

    [SerializeField] private Sprite Sprite_Select;
    [SerializeField] private Sprite Sprite_Unselect;

    [SerializeField] private Button Button_Exit;
    [SerializeField] private Button Button_Rotation;
    [SerializeField] private Button Button_Confirm;
    [SerializeField] private Button Button_Cancel;
    [SerializeField] private Button Button_ExitMode;
    [SerializeField] private Button Button_Remove;
    [SerializeField] private Button Button_Assign;

    private HousingViewModel _housingVM;

    private void Awake()
    {
        Button_Exit.onClick.AddListener(OnClickExit);
        Button_Rotation.onClick.AddListener(OnClickRotation);
        Button_Confirm.onClick.AddListener(OnClickConfirm);
        Button_Cancel.onClick.AddListener(OnClickCancel);
        Button_ExitMode.onClick.AddListener(OnClickExitMode);
        Button_Remove.onClick.AddListener(OnClickRemove);
        Button_Assign.onClick.AddListener(OnClickAssign);

        Button_All.onClick.AddListener(OnClickAll);
        Button_Furniture.onClick.AddListener(OnClickFurniture);
        Button_Toy.onClick.AddListener(OnClickToy);
        Button_Decor.onClick.AddListener(OnClickDecor);
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
        UpdateCategory(_housingVM.HousingCategory);
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

            case nameof(_housingVM.HousingCategory):
                RefreshSlots();
                UpdateCategory(_housingVM.HousingCategory);
                break;

            case nameof(_housingVM.CurrentState):
            case nameof(_housingVM.FurnitureVM):
            case nameof(_housingVM.CanConfirm):
                UpdateState();
                break;

            case nameof(_housingVM.TargetRoom):
                RefreshSlots();
                break;

            case nameof(_housingVM.ItemList):
                RefreshSlots();
                break;

            case nameof(_housingVM.RequestAssignHamster):
                UIManager.Instance.OpenWheelUI();
                UpdateState();
                break;
        }
    }

    private void RefreshSlots()
    {
        if (_housingVM.CurrentViewMode == HousingViewMode.Garden || _housingVM.TargetRoom != null)
        {
            InitFurnitureSlot(_housingVM.GetHousingCategory()).Forget();
        }
    }

    private void UpdateState()
    {
        if (_housingVM.RequestAssignHamster != null)
        {
            Panel_Info.SetActive(false);
            Panel_FurnitureBar.SetActive(false);

            Button_Exit.gameObject.SetActive(false);
            Button_Rotation.gameObject.SetActive(false);
            Button_Confirm.gameObject.SetActive(false);
            Button_Cancel.gameObject.SetActive(false);
            Button_ExitMode.gameObject.SetActive(false);
            Button_Remove.gameObject.SetActive(false);
            Button_Assign.gameObject.SetActive(false);
        }
        else if (_housingVM.CurrentState == HousingState.SelectRoom)
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
            Button_Assign.gameObject.SetActive(_housingVM.CanAssignCurrentFurniture);

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
            Button_Assign.gameObject.SetActive(false);
        }
    }

    public async UniTask InitFurnitureSlot(Dictionary<long, FurnitureSlotViewModel> itemList)
    {
        bool isEmpty = itemList.Count <= 0;
        Text_EmptyInfo.gameObject.SetActive(isEmpty);

        foreach (Transform child in Parent_Slot.transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }

            GameObjectManager.Instance.RequestDestroyObject(child.gameObject);
        }

        foreach (var itemKv in itemList)
        {
            var furnitureSlotVm = itemKv.Value;

            GameObject slot = await GameObjectManager.Instance.CreateObjectAsync("FurnitureSlot", $"Prefabs/UI/FurnitureSlot", Vector3.zero);
            slot.transform.SetParent(Parent_Slot.transform, false);

            FurnitureSlot furnitureSlot = slot.GetComponent<FurnitureSlot>();
            furnitureSlot.Bind(furnitureSlotVm, _housingVM);
        }
    }

    private void SetButtonImage(Button button, bool isSelect)
    {
        if (button.TryGetComponent<Image>(out Image image))
        {
            image.sprite = isSelect ? Sprite_Select : Sprite_Unselect;
        }
    }

    private void UpdateCategory(HousingCategory category)
    {
        SetButtonImage(Button_All, category == HousingCategory.All);
        SetButtonImage(Button_Furniture, category == HousingCategory.Furniture);
        SetButtonImage(Button_Toy, category == HousingCategory.Toy);
        SetButtonImage(Button_Decor, category == HousingCategory.Decor);
    }

    private void OnClickCategory(HousingCategory category)
    {
        _housingVM.HousingCategory = category;
        UpdateCategory(category);
    }

    private void OnClickAll()
    {
        OnClickCategory(HousingCategory.All);
    }

    private void OnClickFurniture()
    {
        OnClickCategory(HousingCategory.Furniture);
    }

    private void OnClickToy()
    {
        OnClickCategory(HousingCategory.Toy);
    }

    private void OnClickDecor()
    {
        OnClickCategory(HousingCategory.Decor);
    }

    private void OnClickExit()
    {
        if (_housingVM.CurrentViewMode == HousingViewMode.Garden)
        {
            _housingVM.EnterOverviewMode();
            UIManager.Instance.CloseHousingUI();
            UIManager.Instance.OpenDecorUI();
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
        _housingVM.ConfirmPos();
    }

    private void OnClickCancel()
    {
        _housingVM.CancelPos();
    }

    private void OnClickExitMode()
    {
        if (_housingVM.FurnitureVM != null)
        {
            _housingVM.CancelPos();
        }

        _housingVM.EnterOverviewMode();

        UIManager.Instance.CloseHousingUI();
        UIManager.Instance.OpenDecorUI();
    }

    private void OnClickRemove()
    {
        _housingVM.RemoveSelectedFurniture().Forget();
    }

    private void OnClickAssign()
    {
        _housingVM.OpenAssignUI();
    }
}