// 쳇바퀴 피버타임 밸런싱 데이터(등급별 씨앗 획득량, 발생 주기, 제한시간)
using UnityEngine;

[System.Serializable]
public class FeverTimeData : GameDataBase
{
    public int SeedPerTap;
    public int TriggerIntervalSec;
    public int TapDurationSec;
}