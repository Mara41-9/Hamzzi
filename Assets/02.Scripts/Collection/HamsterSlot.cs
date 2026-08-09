using Cysharp.Threading.Tasks;
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

    private void OnEnable()
    {
        SlotButton.onClick.AddListener(OnClickSlot);
    }

    private void OnDisable()
    {
        SlotButton.onClick.RemoveListener(OnClickSlot);
    }

    public void InitSlot(HamsterData hamsterData, bool isCollected)
    {
        _hamsterId = hamsterData.Id;
        // 나중에 이미지 넣으면 주석 해체
        //LoadHamsterIcon(hamsterData.IconPath).Forget();

        LockImage.SetActive(!isCollected);
    }

    private async UniTask LoadHamsterIcon(string path)
    {
        Sprite hamsterIcon = await ResourceManager.Instance.LoadAsset<Sprite>(path);
        if (hamsterIcon == null)
            return;

        HamsterIcon.sprite = hamsterIcon;
    }

    private void OnClickSlot()
    {
        OnSlotClicked?.Invoke(_hamsterId);

    }
}