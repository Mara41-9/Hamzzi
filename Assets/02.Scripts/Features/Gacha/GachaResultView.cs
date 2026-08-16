using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultView : MonoBehaviour
{
    [SerializeField] private Button ExitButton;

    [SerializeField] private GameObject ResultSlotPrefab;
    [SerializeField] private Transform SlotContent;

    private List<GachaResultSlot> _createdSlotList = new List<GachaResultSlot>();
    private const int _slotCount = 10;

    private void OnEnable()
    {
        ExitButton.onClick.AddListener(ExitResultUI);
    }

    private void OnDisable()
    {
        ExitButton.onClick.RemoveListener(ExitResultUI);
    }

    private void ExitResultUI()
    {
        gameObject.SetActive(false);
    }

    public void ShowGachaResult(List<string> hamsterIdList)
    {
        if (_createdSlotList.Count <= 0)
        {
            CreateResultSlot();
        }
        InitSlot();

        int resultCount = hamsterIdList.Count;
        for(int i = 0; i < resultCount; i++)
        {
            string hamsterId = hamsterIdList[i];
            GachaResultSlot slot = _createdSlotList[i];

            slot.gameObject.SetActive(true);
            slot.UpdateSlot(hamsterId);
        }
    }

    private void InitSlot()
    {
        foreach(var slot in _createdSlotList)
        {
            slot.gameObject.SetActive(false);
        }
    }

    private void CreateResultSlot()
    {
        for(int i = 0; i < _slotCount; i++)
        {
            GameObject slot = Instantiate<GameObject>(ResultSlotPrefab, SlotContent);
            GachaResultSlot component = slot.GetComponent<GachaResultSlot>();

            _createdSlotList.Add(component);
        }
    }
}