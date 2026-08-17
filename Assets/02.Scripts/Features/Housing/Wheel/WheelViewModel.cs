using System.Collections.Generic;

public class WheelSlotData
{
    public string HamsterID;
    public HamsterData HamsterData;
    public bool IsAssigned;
}

public class WheelViewModel : ViewModelBase
{
    public FurnitureViewModel TargetFurniture { get; private set; }
    public List<WheelSlotData> Hamsters { get; private set; } = new List<WheelSlotData>();
    public string CurrentHamsterID
    {
        get
        {
            return TargetFurniture.AssignHamsterID;
        }
    }

    public WheelViewModel(FurnitureViewModel targetFurnitureVM)
    {
        TargetFurniture = targetFurnitureVM;
        RefreshHamsterList();
    }

    public void RefreshHamsterList()
    {
        Hamsters.Clear();

        CollectionViewModel collectionVM = NetworkManager_YMH.Instance.CollectionService.GetCollectionViewModel();
        HashSet<string> collectHamsters = collectionVM.CollectedHamsterIdList;
        HashSet<string> otherAssignID = GetAssignHamster();

        foreach (string hamsterID in collectHamsters)
        {
            if (otherAssignID.Contains(hamsterID))
            {
                continue;
            }

            HamsterData data = GameDataManager.Instance.GetData<HamsterData>(hamsterID);

            bool isCurrent = (hamsterID == TargetFurniture.AssignHamsterID);

            Hamsters.Add(new WheelSlotData { HamsterID = hamsterID, HamsterData = data, IsAssigned = isCurrent});
        }

        OnPropertyChanged(nameof(TargetFurniture));
        OnPropertyChanged(nameof(CurrentHamsterID));
    }

    private HashSet<string> GetAssignHamster()
    {
        HashSet<string> assignID = new HashSet<string>();

        BuildViewModel buildVM = ServiceManager.Instance.BuildService.GetBuildViewModel();

        if (buildVM != null && buildVM.Builds != null)
        {
            HashSet<RoomViewModel> uniqueRoom = new HashSet<RoomViewModel>(buildVM.Builds.Values);

            foreach (RoomViewModel room in uniqueRoom)
            {
                foreach (FurnitureViewModel furniture in room.FurnitureList)
                {
                    if (furniture.InstanceID != TargetFurniture.InstanceID && !string.IsNullOrEmpty(furniture.AssignHamsterID))
                    {
                        assignID.Add(furniture.AssignHamsterID);
                    }
                }
            }
        }

        HousingViewModel housingVM = ServiceManager.Instance.HousingService.GetHousingViewModel();

        if (housingVM != null && housingVM.GardenFurnitureList != null)
        {
            foreach (FurnitureViewModel furniture in housingVM.GardenFurnitureList)
            {
                if (furniture.InstanceID != TargetFurniture.InstanceID && !string.IsNullOrEmpty(furniture.AssignHamsterID))
                {
                    assignID.Add(furniture.AssignHamsterID);
                }
            }
        }

        return assignID;
    }

    public void AssignHamster(string hamsterID)
    {
        TargetFurniture.AssignHamsterID = hamsterID;

        ServiceManager.Instance.HousingService.SaveHousingData();

        RefreshHamsterList();
    }

    public void UnassignHamster(string hamsterID)
    {
        TargetFurniture.AssignHamsterID = null;

        ServiceManager.Instance.HousingService.SaveHousingData();

        RefreshHamsterList();
    }
}
