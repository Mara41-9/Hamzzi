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
        get => TargetFurniture.AssignHamsterID;
    }

    private string _assignHamsterID;
    public string AssignHamsterID
    {
        get => TargetFurniture.AssignHamsterID;
        set
        {
            if (TargetFurniture.AssignHamsterID != value)
            {
                TargetFurniture.AssignHamsterID = value;
                OnPropertyChanged(nameof(AssignHamsterID));
                OnPropertyChanged(nameof(CurrentHamsterID));
            }
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

        Dictionary<long, HamsterSave> collectHamsters = GetCollectHamsterID();
        Dictionary<string, int> otherAssignCounts = GetAssignHamster();

        Dictionary<string, int> remainingAssigns = new Dictionary<string, int>(otherAssignCounts);

        foreach (var kv in collectHamsters)
        {
            HamsterSave hamsterSave = kv.Value;
            string hamsterID = hamsterSave.HamsterId;

            if (remainingAssigns.TryGetValue(hamsterID, out int count) && count > 0)
            {
                remainingAssigns[hamsterID] = count - 1;
                continue;
            }

            HamsterData data = GameDataManager.Instance.GetData<HamsterData>(hamsterID);
            bool isCurrent = (hamsterID == TargetFurniture.AssignHamsterID);

            Hamsters.Add(new WheelSlotData { HamsterID = hamsterID, HamsterData = data, IsAssigned = isCurrent });
        }

        OnPropertyChanged(nameof(TargetFurniture));
        OnPropertyChanged(nameof(CurrentHamsterID));
    }

    private Dictionary<string, int> GetAssignHamster()
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();
        List<FurnitureViewModel> allFurniture = ServiceManager.Instance.HousingService.GetAllPlacedFurniture();

        foreach (var furniture in allFurniture)
        {
            if (furniture != TargetFurniture && !string.IsNullOrEmpty(furniture.AssignHamsterID))
            {
                string hamsterID = furniture.AssignHamsterID;

                if (!counts.ContainsKey(hamsterID))
                {
                    counts[hamsterID] = 0;
                }

                counts[hamsterID]++;
            }
        }

        return counts;
    }

    public void AssignHamster(string hamsterID)
    {
        TargetFurniture.AssignHamsterID = hamsterID;
        AssignHamsterID = hamsterID;

        //ServiceManager.Instance.HousingService.SaveHousingData();

        RefreshHamsterList();
    }

    public void UnassignHamster()
    {
        TargetFurniture.AssignHamsterID = null;
        AssignHamsterID = null;

        //ServiceManager.Instance.HousingService.SaveHousingData();

        RefreshHamsterList();
    }

    private Dictionary<long, HamsterSave> GetCollectHamsterID()
    {
        return ServiceManager.Instance.CollectionService.GetCollectionViewModel().CollectedHamsterList;
    }
}