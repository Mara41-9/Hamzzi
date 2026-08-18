using System.Collections.Generic;
using Unity.VisualScripting;

public class HousingService
{
    private HousingViewModel _housingVM;

    public HousingViewModel GetHousingViewModel()
    {
        if (_housingVM == null)
        {
            CreateHousingViewModel();
        }

        return _housingVM;
    }

    public HousingViewModel CreateHousingViewModel()
    {
        HousingViewModel housingVm = new HousingViewModel();
        _housingVM = housingVm;
        _housingVM.EnterHousingMode();

        return housingVm;
    }

    public List<FurnitureViewModel> GetAllPlacedFurniture()
    {
        List<FurnitureViewModel> allList = new List<FurnitureViewModel>();

        BuildViewModel buildVM = ServiceManager.Instance.BuildService.GetBuildViewModel();

        if (buildVM?.Builds != null)
        {
            var uniqueRooms = new HashSet<RoomViewModel>(buildVM.Builds.Values);

            foreach (var room in uniqueRooms)
            {
                if (room.FurnitureList != null)
                {
                    allList.AddRange(room.FurnitureList);
                }
            }
        }

        if (_housingVM?.GardenFurnitureList != null)
        {
            allList.AddRange(_housingVM.GardenFurnitureList);
        }

        return allList;
    }

    // 저장 관련
    public void SaveHousingData()
    {
        ServiceManager.Instance.BuildService.SaveBuildData();

        // TODO: Builds랑 각 RoomViewModel.FurnitureList 가져와서 저장
        // RoomInstanceID, FurnitureID, LocalPos, RotationAngle, Size 저장
    }

    public void LoadHousingData()
    {
        ServiceManager.Instance.BuildService.LoadBuildData();

        // TODO: 가구 배치 정보 가져와서 각각 RoomViewModel.AddFuniture()
        // RoomInstanceID로 RoomViewModel 찾기
        // RoomViewModel.AddFuniture(furnitureVM)로 가구 배치 및 스폰
    }

    public void LoadAllHousingData()
    {
        ServiceManager.Instance.BuildService.LoadBuildData();
        LoadHousingData();
    }

    public void AddItem(ShopSlotViewModel shopSlotVm)
    {
        var housingVm = GetHousingViewModel();

        foreach(var itemKv in housingVm.ItemList)
        {
            var furnitureVm = itemKv.Value;
            if(furnitureVm.ItemDataId == shopSlotVm.ItemDataId)
            {
                furnitureVm.StackCount++;
                return;
            }
        }

        var newFurnitureSlotVm = new FurnitureSlotViewModel();
        long uniqueId = TestGameUtil.GenerateUniqueId();

        newFurnitureSlotVm.ItemUniqueId = uniqueId;
        newFurnitureSlotVm.ItemDataId = shopSlotVm.ItemDataId;
        newFurnitureSlotVm.IconSprite = shopSlotVm.IconSprite;
        newFurnitureSlotVm.StackCount = 1;

        housingVm.AddItemSlotViewModel(newFurnitureSlotVm);
    }
}
