using UnityEngine;

public enum HousingState
{
    SelectRoom,
    Placing
}

public enum HousingViewMode
{
    OverView,
    FocusRoom,
    Garden
}

public class HousingViewModel : ViewModelBase
{
    public bool CanConfirm
    {
        get
        {
            if (FurnitureVM.IsValid)
            {
                return true;
            }

            return false;
        }
    }

    private HousingViewMode _currentViewMode = HousingViewMode.OverView;
    public HousingViewMode CurrentViewMode
    {
        get => _currentViewMode;
        set
        {
            if (_currentViewMode != value)
            {
                _currentViewMode = value;
                OnPropertyChanged(nameof(CurrentViewMode));
            }
        }
    }

    private HousingState _currentState = HousingState.SelectRoom;
    public HousingState CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnPropertyChanged(nameof(CurrentState));
            }
        }
    }

    private FurnitureViewModel _furnitureVM;
    public FurnitureViewModel FurnitureVM
    {
        get => _furnitureVM;
        set
        {
            if (_furnitureVM != value)
            {
                _furnitureVM = value;
                OnPropertyChanged(nameof(FurnitureVM));
                OnPropertyChanged(nameof(CanConfirm));
            }
        }
    }

    private RoomViewModel _targetRoom;
    public RoomViewModel TargetRoom
    {
        get => _targetRoom;
        set
        {
            if (_targetRoom != value)
            {
                _targetRoom = value;
                OnPropertyChanged(nameof(TargetRoom));

                if (_targetRoom != null)
                {
                    CurrentState = HousingState.Placing;
                }
                else
                {
                    CurrentState = HousingState.SelectRoom;
                    FurnitureVM = null;
                }
            }
        }
    }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(CurrentViewMode));
        OnPropertyChanged(nameof(CurrentState));
        OnPropertyChanged(nameof(FurnitureVM));
        OnPropertyChanged(nameof(TargetRoom));
    }

    public void EnterHousingMode()
    {
        _targetRoom = null;
        _furnitureVM = null;
        _currentState = HousingState.SelectRoom;

        InvokeOnceOnInit();
    }

    public void SelectFurniture(string furnitureID, Vector2Int subSize, RoomViewModel targetRoom)
    {
        TargetRoom = targetRoom;

        Vector2Int initialPos = new Vector2Int(TargetRoom.SubGridSize.x / 2 - subSize.x / 2, TargetRoom.SubGridSize.y / 2 - subSize.y / 2);

        FurnitureVM = new FurnitureViewModel(furnitureID, initialPos, subSize);
        CheckCurrentPos();
    }

    public void MovePos(Vector2Int pos)
    {
        FurnitureVM.LocalPos = pos;
        CheckCurrentPos();
    }

    public void RotatePos()
    {
        FurnitureVM.Rotate();
        CheckCurrentPos();

        OnPropertyChanged(nameof(FurnitureVM));
    }

    public bool ConfirmPos()
    {
        if (FurnitureVM == null || !FurnitureVM.IsValid || TargetRoom == null)
        {
            return false;
        }

        FurnitureVM.RoomInstanceID = TargetRoom.InstanceID;

        if (TargetRoom.AddFuniture(FurnitureVM))
        {
            FurnitureVM = null;
            return true;
        }

        return false;
    }

    public void CancelPos()
    {
        FurnitureVM = null;
    }

    public void ExitRoom()
    {
        TargetRoom = null;
    }

    private void CheckCurrentPos()
    {
        FurnitureVM.IsValid = TargetRoom.IsValidPlace(FurnitureVM.LocalPos, FurnitureVM.Size);
        OnPropertyChanged(nameof(CanConfirm));
    }

    public void EnterGardenMode()
    {
        CurrentViewMode = HousingViewMode.Garden;
    }

    public void EnterOverviewMode()
    {
        TargetRoom = null;
        CurrentViewMode = HousingViewMode.OverView;
    }
}