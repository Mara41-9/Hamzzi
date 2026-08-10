using NUnit.Framework.Constraints;
using UnityEngine;

public class HousingViewModel : ViewModelBase
{
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
            }
        }
    }

    public bool CanConfirm()
    {
        if (FurnitureVM.IsValid)
        {
            return true;
        }

        return false;
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
    }

    public bool ConfirmPos()
    {
        FurnitureVM.IsValid = false;

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

    private void CheckCurrentPos()
    {
        FurnitureVM.IsValid = TargetRoom.IsValidPlace(FurnitureVM.LocalPos, FurnitureVM.Size);
        OnPropertyChanged(nameof(CanConfirm));
    }
}