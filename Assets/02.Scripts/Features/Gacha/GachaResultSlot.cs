using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultSlot : MonoBehaviour
{
    [SerializeField] private Image HamsterIcon;
    [SerializeField] private Image TierImage;

    [Header("Tier Color")]
    [SerializeField] private Color SSTierColor;
    [SerializeField] private Color STierColor;
    [SerializeField] private Color ATierColor;

    public void UpdateSlot(string hamsterId)
    {
        HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);

        LoadHamsterIcon(hamsterData.IconPath).Forget();
        SetTierColor(hamsterData.HamsterTier);
    }

    private async UniTask LoadHamsterIcon(string iconPath)
    {
        Sprite hamsterIcon = await ResourceManager.Instance.LoadAsset<Sprite>(iconPath);
        HamsterIcon.sprite = hamsterIcon;
    }

    private void SetTierColor(HamsterTier tier)
    {
        switch (tier)
        {
            case HamsterTier.SS:
                TierImage.color = SSTierColor;
                break;
            case HamsterTier.S:
                TierImage.color = STierColor;
                break;
            case HamsterTier.A:
                TierImage.color = ATierColor;
                break;
        }
    }
}