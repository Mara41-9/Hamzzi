using UnityEngine;
using UnityEngine.UI;

public class GachaView : ViewBase
{
    [SerializeField] private Button GachaButton;

    private void OnEnable()
    {
        GachaButton.onClick.AddListener(DrawHamster);
    }

    private void OnDisable()
    {
        GachaButton.onClick.RemoveListener(DrawHamster);
    }

    private void DrawHamster()
    {
        int SSCount = 0;
        int SCount = 0;
        int ACount = 0;
        for(int i = 0; i < 1000; i++)
        {
            string hamsterId = NetworkManager_YMH.Instance.GachaService.DrawGacha();
            HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);
            Debug.Log($"햄스터 가챠 결과 : {hamsterData.Name}({hamsterData.HamsterTier})");

            switch (hamsterData.HamsterTier)
            {
                case HamsterTier.SS:
                    SSCount++;
                    break;
                case HamsterTier.S:
                    SCount++;
                    break;
                case HamsterTier.A:
                    ACount++;
                    break;
            }
        }

        Debug.Log($"총 횟수 : 1000,  SS : {SSCount},  S : {SCount},  A : {ACount}");
    }
}
