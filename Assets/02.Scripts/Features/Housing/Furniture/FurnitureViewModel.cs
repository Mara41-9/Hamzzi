using System;
using UnityEngine;

public class FurnitureViewModel : ViewModelBase
{
    private string InstanceID {  get; set; }
    public string FurnitureID { get; set; }

    private string _roomInstanceID;
    public string RoomInstanceID
    {
        get => _roomInstanceID;
        set
        {
            if (_roomInstanceID != value)
            {
                _roomInstanceID = value;
                OnPropertyChanged(nameof(RoomInstanceID));
            }
        }
    }

    private Vector2Int _localPos;
    public Vector2Int LocalPos
    {
        get => _localPos;
        set
        {
            if (_localPos != value)
            {
                _localPos = value;
                OnPropertyChanged(nameof(_localPos));
            }
        }
    }

    private Vector2Int _size;
    public Vector2Int Size
    {
        get => _size;
        set
        {
            if (_size != value)
            {
                _size = value;
                OnPropertyChanged(nameof(_size));
            }
        }
    }

    private int _rotationAngle;
    public int RotationAngle
    {
        get => _rotationAngle;
        set
        {
            if (_rotationAngle != value)
            {
                _rotationAngle = value;
                OnPropertyChanged(nameof(_rotationAngle));
            }
        }
    }

    private bool _isValid;
    public bool IsValid
    {
        get => _isValid;
        set
        {
            if (_isValid != value)
            {
                _isValid = value;
                OnPropertyChanged(nameof(_isValid));
            }
        }
    }

    public FurnitureViewModel (string furnitureID, Vector2Int localPos, Vector2Int size)
    {
        InstanceID = Guid.NewGuid().ToString();
        FurnitureID = furnitureID;
        LocalPos = localPos;
        Size = size;
        RotationAngle = 0;
    }

    public void Rotate()
    {
        RotationAngle = (RotationAngle + 90) % 360;
        Size = new Vector2Int(Size.y, Size.x);
    }
}
