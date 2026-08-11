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

    [SerializeField] private Button Button_Rotation;
    [SerializeField] private Button Button_Confirm;
    [SerializeField] private Button Button_Cancel;

    private HousingViewModel _housingVM;

    private void Awake()
    {
        Button_Rotation.onClick.AddListener(OnClickRotation);
        Button_Confirm.onClick.AddListener(OnClickConfirm);
        Button_Cancel.onClick.AddListener(OnClickCancel);
    }

    public void BindViewModel(HousingViewModel housingVM)
    {
        _housingVM = housingVM;
        _housingVM.PropertyChanged += OnPropertyChanged_View;

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
            case nameof(_housingVM.CurrentState):
            case nameof(_housingVM.FurnitureVM):
            case nameof(_housingVM.CanConfirm):
                UpdateState();
                break;

            case nameof(_housingVM.TargetRoom):
                InitFurnitureSlot(GetDummyFurnitureList()).Forget();
                break;
        }
    }

    private void UpdateState()
    {
        if (_housingVM.CurrentState == HousingState.SelectRoom)
        {
            Panel_Info.SetActive(true);

            Panel_FurnitureBar.SetActive(false);

            Button_Rotation.gameObject.SetActive(false);
            Button_Confirm.gameObject.SetActive(false);
            Button_Cancel.gameObject.SetActive(false);
        }
        else if (_housingVM.FurnitureVM != null)
        {
            Panel_Info.SetActive(false);

            Panel_FurnitureBar.SetActive(false);

            Button_Rotation.gameObject.SetActive(true);
            Button_Confirm.gameObject.SetActive(true);
            Button_Cancel.gameObject.SetActive(true);
        }
        else
        {
            Panel_Info.SetActive(false);

            Panel_FurnitureBar.SetActive(true);

            Button_Rotation.gameObject.SetActive(false);
            Button_Confirm.gameObject.SetActive(false);
            Button_Cancel.gameObject.SetActive(false);
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

    private void OnClickRotation()
    {
        _housingVM.RotatePos();
    }

    private void OnClickConfirm()
    {
        FurnitureViewModel confirmVM = _housingVM.FurnitureVM;

        _housingVM.ConfirmPos();

        HousingView.SpawnFurniture(confirmVM).Forget();
    }

    private void OnClickCancel()
    {
        _housingVM.CancelPos();
        HousingView.ClearGhostObject();
    }

    // 테스트용
    private List<ItemData> GetDummyFurnitureList()
    {
        return new List<ItemData>
        {
            new ItemData
            {
                Id = "Armchair_01",
                Name = "기본 의자",
                IconPath = "Image/Item/Furniture/Armchair_01",
                PrefabPath = "Prefabs/Furniture/Armchair_01",
                SizeX = 2,
                SizeY = 2
            },
            new ItemData
            {
                Id = "Fireplace_03",
                Name = "원목 탁자",
                IconPath = "Image/Item/Furniture/Fireplace_03",
                PrefabPath = "Prefabs/Furniture/Fireplace_03",
                SizeX = 2,
                SizeY = 3
            }
        };
    }
}