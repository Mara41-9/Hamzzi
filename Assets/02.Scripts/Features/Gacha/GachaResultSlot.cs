using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultSlot : MonoBehaviour
{
    [SerializeField] private Image HamsterIcon;

    public void UpdateSlotIcon(string hamsterId)
    {
        LoadHamsterIcon(hamsterId).Forget();
    }

    private async UniTask LoadHamsterIcon(string hamsterId)
    {
        HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);
        string iconPath = hamsterData.IconPath;

        Sprite hamsterIcon = await ResourceManager.Instance.LoadAsset<Sprite>(iconPath);
        HamsterIcon.sprite = hamsterIcon;
    }
}