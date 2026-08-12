using System.ComponentModel;
using UnityEngine;

public class Aisle : MonoBehaviour
{
    [SerializeField] private GameObject Object_Up;
    [SerializeField] private GameObject Object_Down;
    [SerializeField] private GameObject Object_Left;
    [SerializeField] private GameObject Object_Right;

    private RoomViewModel _roomVM;

    public void Bind(RoomViewModel roomVM)
    {
        _roomVM = roomVM;
        _roomVM.PropertyChanged += OnPropertyChanged_View;

        ApplyConnection(_roomVM.AisleConnection);
    }

    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_roomVM.AisleConnection):
                ApplyConnection(_roomVM.AisleConnection);
                break;
        }
    }

    private void OnDestroy()
    {
        if (_roomVM != null)
        {
            _roomVM.PropertyChanged -= OnPropertyChanged_View;
        }
    }

    public void ApplyConnection(AisleConnection connection)
    {
        Object_Up.SetActive(!connection.Up);
        Object_Down.SetActive(!connection.Down);
        Object_Left.SetActive(!connection.Left);
        Object_Right.SetActive(!connection.Right);
    }
}