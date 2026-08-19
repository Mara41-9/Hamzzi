using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FaceSlot : MonoBehaviour
{
    [SerializeField] private UIButton SlotButton;
    [SerializeField] private Image FaceIcon;
    [SerializeField] private GameObject LockImage;

    private string _faceId;

    public event Action<string> OnSlotClicked;

    private void OnEnable()
    {
        SlotButton.BindOnClickButtonEvent(OnClickSlot);
    }

    private void OnDisable()
    {
        SlotButton.BindOnClickButtonEvent(OnClickSlot);
    }

    public void InitSlot(FaceData faceData, bool isCollected)
    {
        _faceId = faceData.Id;

        LoadFaceIcon(faceData.IconPath).Forget();
        UpdateLockImage(isCollected);
    }

    public void UpdateLockImage(bool isCollected)
    {
        LockImage.SetActive(!isCollected);
    }

    private async UniTask LoadFaceIcon(string path)
    {
        Sprite faceIcon = await ResourceManager.Instance.LoadAsset<Sprite>(path);
        if (faceIcon == null)
            return;

        FaceIcon.sprite = faceIcon;
    }

    private void OnClickSlot()
    {
        OnSlotClicked?.Invoke(_faceId);
    }
}
