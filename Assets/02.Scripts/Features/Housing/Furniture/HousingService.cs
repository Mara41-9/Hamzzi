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
            },
            new ItemData
            {
                Id = "Play_Wheel",
                Name = "달려라 쳇바퀴",
                IconPath = "Image/Item/Play/Wheel",
                PrefabPath = "Prefabs/Furniture/Play_Wheel"
            }
        };
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
}
