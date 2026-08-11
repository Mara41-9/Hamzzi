using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class HousingUI : ViewBase
{
    [SerializeField] private HousingView HousingView;

    [SerializeField] private GameObject Panel_FurnitureBar;

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

    private void BindViewModel(HousingViewModel housingVM)
    {
        _housingVM = housingVM;
        _housingVM.PropertyChanged += OnPropertyChanged_View;
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
            case nameof(_housingVM.FurnitureVM):
            case nameof(_housingVM.CanConfirm):

                break;
        }
    }

    private async UniTask InitFurnitureSlot(List<ItemData> itemList)
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
            furnitureSlot.Bind(item, _housingVM, HousingView).Forget();
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

        HousingView.ClearGhostObject();
        SpawnFurniture(confirmVM).Forget();
    }

    private void OnClickCancel()
    {
        _housingVM.CancelPos();
        HousingView.ClearGhostObject();
    }

    private async UniTask SpawnFurniture(FurnitureViewModel furnitureVM)
    {
        RoomViewModel targetRoom = _housingVM.TargetRoom;

        float cellSize = 1.0f;
        float subCellSize = cellSize / targetRoom.GridFactor;

        float localX = (furnitureVM.LocalPos.x + furnitureVM.Size.x * 0.5f) * subCellSize;
        float localZ = (furnitureVM.LocalPos.y + furnitureVM.Size.y * 0.5f) * subCellSize;

        Vector3 spawnPos = new Vector3((targetRoom.OriginPos.x * cellSize) + localX, 0f, (targetRoom.OriginPos.y * cellSize) + localZ);

        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync(furnitureVM.InstanceID, $"Prefabs/Furniture/{furnitureVM.FurnitureID}", spawnPos);
        prefab.transform.rotation = Quaternion.Euler(0f, furnitureVM.RotationAngle, 0f);
    }
}