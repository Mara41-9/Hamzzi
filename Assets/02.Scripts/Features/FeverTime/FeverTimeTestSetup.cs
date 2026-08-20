// 피버타임 휠 터치 테스트용 임시 스크립트 (확인 후 삭제)
using UnityEngine;

public class FeverWheelTestSetup : MonoBehaviour
{
    [SerializeField] private string _testHamsterId = "Hamster_01";

    private void Start()
    {
        GameDataManager.Instance.LoadData<HamsterData>();
        GameDataManager.Instance.LoadData<FeverTimeData>();

        HamsterData data = GameDataManager.Instance.GetData<HamsterData>(_testHamsterId);
        GetComponent<FeverTimeWheel>().SetHamster(data);
    }
}