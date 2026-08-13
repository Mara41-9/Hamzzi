using UnityEngine;
using UnityEngine.UI;

public class GachaResultSlot : MonoBehaviour
{
    [SerializeField] private Image HamsterIcon;

    public void UpdateSlotIcon(string hamsterId)
    {
        // TODO : 아이콘이 올라가면 나머지 구현
        HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);
        string iconPath = hamsterData.IconPath;

        //HamsterIcon.sprite
    }
}