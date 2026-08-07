using System;
using UnityEngine;
using UnityEngine.UI;

public class HamsterSlot : MonoBehaviour
{
    [SerializeField] private Button SlotButton;
    [SerializeField] private GameObject LockImage;
    [SerializeField] private Image HamsterIcon;

    private string _hamsterId;

    public event Action<string> OnSlotClicked;

    private void OnDisable()
    {
        SlotButton.onClick.RemoveListener(OnClickSlot);
    }

    public void InitSlot(HamsterData hamsterData)
    {
        _hamsterId = hamsterData.Id;
        //LoadHamsterIcon(hamsterData.IconPath);

        SlotButton.onClick.AddListener(OnClickSlot);
    }

    private void LoadHamsterIcon(string path)
    {

    }

    private void OnClickSlot()
    {
        OnSlotClicked?.Invoke(_hamsterId);
    }
}