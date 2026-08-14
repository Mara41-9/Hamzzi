using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum HousingState
{
    SelectRoom,
    Placing,
    Editing
}

public enum HousingViewMode
{
    OverView,
    FocusRoom,
    Garden
}

public class HousingViewModel : ViewModelBase
{
    public List<FurnitureViewModel> GardenFurnitureList { get; private set; } = new List<FurnitureViewModel>();

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

    private FurnitureViewModel _selectedInstallFurniture;
    public FurnitureViewModel SelectedInstallFurniture
    {
        get => _selectedInstallFurniture;
        set
        {
            if (_selectedInstallFurniture != value)
            {
                _selectedInstallFurniture = value;
                OnPropertyChanged(nameof(SelectedInstallFurniture));
            }
        }
    }

    private FurnitureViewModel _destroyFurniture;
    public FurnitureViewModel DestroyFurniture
    {
        get => _destroyFurniture;
        set
        {
            if (_destroyFurniture != value)
            {
                _destroyFurniture = value;
                OnPropertyChanged(nameof(DestroyFurniture));
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

        if (CurrentViewMode == HousingViewMode.Garden)
        {
            _currentState = HousingState.Placing;
        }
        else
        {
            _currentState = HousingState.SelectRoom;
        }

        InvokeOnceOnInit();
    }

    public void SelectInstallFurniture(FurnitureViewModel furnitureVM)
    {
        SelectedInstallFurniture = furnitureVM;
        FurnitureVM = furnitureVM;

        TargetRoom.RemoveFurniture(furnitureVM);
        CurrentState = HousingState.Editing;

        CheckCurrentPos();
    }

    public bool RemoveSelectedFurniture()
    {
        DestroyFurniture = FurnitureVM;

        if (CurrentViewMode == HousingViewMode.Garden)
        {
            GardenFurnitureList.Remove(FurnitureVM);
            OnPropertyChanged(nameof(GardenFurnitureList));
        }
        else if (TargetRoom != null)
        {
            TargetRoom.RemoveFurniture(FurnitureVM);
        }

        string furnitureID = FurnitureVM.FurnitureID;
        // 여기에 인벤토리로 돌아가는 로직

        FurnitureVM = null;
        SelectedInstallFurniture = null;
        CurrentState = HousingState.Placing;

        return true;
    }

    public void SelectFurniture(string furnitureID, Vector2Int subSize, RoomViewModel targetRoom)
    {
        TargetRoom = targetRoom;

        Vector2Int initialPos = new Vector2Int(TargetRoom.SubGridSize.x / 2 - subSize.x / 2, TargetRoom.SubGridSize.y / 2 - subSize.y / 2);

        FurnitureVM = new FurnitureViewModel(furnitureID, initialPos, subSize);
        CheckCurrentPos();
    }

    public void SelectGardenFurniture(string furnitureID, Vector2Int subSize)
    {
        Vector2Int initialPos = new Vector2Int(10, 10);
        
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

        if (CurrentViewMode == HousingViewMode.Garden)
        {
            GardenFurnitureList.Add(FurnitureVM);
            OnPropertyChanged(nameof(GardenFurnitureList));

            FurnitureVM = null;
            SelectedInstallFurniture = null;
            CurrentState = HousingState.Placing;

            return true;
        }
        else if (TargetRoom != null)
        {
            FurnitureVM.RoomInstanceID = TargetRoom.InstanceID;

            if (TargetRoom.AddFurniture(FurnitureVM))
            {
                FurnitureVM = null;
                SelectedInstallFurniture = null;
                CurrentState = HousingState.Placing;

                return true;
            }
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
        if (CurrentViewMode == HousingViewMode.Garden)
        {
            bool inBounds = FurnitureVM.LocalPos.x >= -20 && FurnitureVM.LocalPos.x <= 20 && FurnitureVM.LocalPos.y >= -20 && FurnitureVM.LocalPos.y <= 20;

            FurnitureVM.IsValid = inBounds;
        }
        else if (TargetRoom != null)
        {
            FurnitureVM.IsValid = TargetRoom.IsValidPlace(FurnitureVM.LocalPos, FurnitureVM.Size);
        }

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