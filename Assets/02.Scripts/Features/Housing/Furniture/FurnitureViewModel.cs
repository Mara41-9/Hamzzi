using System;
using UnityEngine;

public class FurnitureViewModel : ViewModelBase
{
    public string InstanceID { get; private set; }
    public string FurnitureID { get; private set; }
    public string PrefabPath { get; private set; }

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

    private string _assignHamsterID;
    public string AssignHamsterID
    {
        get => _assignHamsterID;
        set
        {
            if (_assignHamsterID != value)
            {
                _assignHamsterID = value;
                OnPropertyChanged(nameof(AssignHamsterID));
            }
        }
    }

    public bool CanAssignHamster
    {
        get
        {
            return !string.IsNullOrEmpty(FurnitureID) && FurnitureID.Contains("Wheel");
        }
    }

    public FurnitureViewModel (string furnitureID, string prefabPath, Vector2Int localPos, Vector2Int size)
    {
        InstanceID = Guid.NewGuid().ToString();
        PrefabPath = prefabPath;
        FurnitureID = furnitureID;
        LocalPos = localPos;
        Size = size;
        RotationAngle = 0;
    }

    public void Rotate()
    {
        float currentCenterX = LocalPos.x + Size.x * 0.5f;
        float currentCenterY = LocalPos.y + Size.y * 0.5f;

        Size = new Vector2Int(Size.y, Size.x);
        RotationAngle = (RotationAngle + 90) % 360;

        LocalPos = new Vector2Int(Mathf.RoundToInt(currentCenterX - Size.x * 0.5f), Mathf.RoundToInt(currentCenterY - Size.y * 0.5f));
    }
}
