using System.Collections.Generic;

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

    public List<ItemData> GetOwnedFurnitureList()
    {
        // TODO: 유저가 가진 가구 DB 연결 (Shop이랑)

        // 테스트용
        return new List<ItemData>
        {
            new ItemData
            {
                Id = "Armchair_01",
                Name = "기본 의자",
                IconPath = "Image/Item/Furniture/Armchair_01",
                PrefabPath = "Prefabs/Furniture/Armchair_01"
            },
            new ItemData
            {
                Id = "Fireplace_03",
                Name = "기본 벽난로",
                IconPath = "Image/Item/Furniture/Fireplace_03",
                PrefabPath = "Prefabs/Furniture/Fireplace_03"
            }
        };
    }

    public void SaveHousingData()
    {
        // TODO: Builds랑 각 RoomViewModel.FurnitureList 가져와서 저장
    }

    public void LoadHousingData()
    {
        // TODO: 가구 배치 정보 가져와서 각각 RoomViewModel.AddFuniture()
    }
}
