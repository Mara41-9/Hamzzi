using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public struct DoorConfig
{
    public Transform Transform;
    public int Index;
}

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject Object_Up;
    [SerializeField] private GameObject Object_Down;
    [SerializeField] private GameObject Object_Left;
    [SerializeField] private GameObject Object_Right;

    [SerializeField] private DoorConfig[] DoorConfig;

    private float _cellSize = 1.0f;
    private RoomViewModel _roomVM;

    public void Bind(RoomViewModel roomVM)
    {
        _roomVM = roomVM;

        SetupDoorData();

        _roomVM.PropertyChanged += OnPropertyChanged_View;
        SetRoomConnection(_roomVM.AisleConnection);
    }

    private void SetupDoorData()
    {
        if (_roomVM.BuildType != BuildType.Room)
        {
            return;
        }

        float halfWidth = _roomVM.Size.x * _cellSize * 0.5f;
        float halfHeight = _roomVM.Size.y * _cellSize * 0.5f;

        List<DoorData> doorData = new List<DoorData>();

        foreach (var door in DoorConfig)
        {
            Vector2Int offset = CalculateDoor(door.Transform.localPosition, halfWidth, halfHeight);
            doorData.Add(new DoorData { Offset = offset, DirectionIndex = door.Index });
        }

        _roomVM.SetDoorData(doorData);
    }

    private Vector2Int CalculateDoor(Vector3 localPos, float halfWidth, float halfHeight)
    {
        int x = Mathf.Clamp(Mathf.RoundToInt((localPos.x + halfWidth - _cellSize * 0.5f) / _cellSize), 0, _roomVM.Size.x - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((localPos.y + halfHeight - _cellSize * 0.5f) / _cellSize), 0, _roomVM.Size.y - 1);

        return new Vector2Int(x, y);
    }

    private void OnDestroy()
    {
        if (_roomVM != null)
        {
            _roomVM.PropertyChanged -= OnPropertyChanged_View;
        }
    }

    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(RoomViewModel.AisleConnection):
                SetRoomConnection(_roomVM.AisleConnection);
                break;
        }
    }

    public void SetRoomConnection(AisleConnection connection)
    {
        Object_Up.SetActive(!connection.Up);
        Object_Down.SetActive(!connection.Down);
        Object_Left.SetActive(!connection.Left);
        Object_Right.SetActive(!connection.Right);
    }
}
