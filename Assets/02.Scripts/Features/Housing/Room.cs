using System.ComponentModel;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject Object_Left;
    [SerializeField] private GameObject Object_Right;

    private RoomViewModel _roomVM;

    public void Bind(RoomViewModel roomVM)
    {
        _roomVM = roomVM;

        _roomVM.PropertyChanged += OnPropertyChanged_View;
        SetRoomConnection(_roomVM.AisleConnection);
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
        Object_Left.SetActive(!connection.Left);
        Object_Right.SetActive(!connection.Right);
    }
}
