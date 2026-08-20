// 게임 데이터 가공용 순수 함수 모음
using System;
using UnityEngine;

public static class GameUtil
{
    // 방치 보상 계산: 마지막 활동 시각부터 경과한 시간(초, 상한 적용)에 초당 생산량을 곱해 반환
    public static int CalculateIdleReward(long lastActiveTicks, float productionPerSec, float capSeconds)
    {
        if (lastActiveTicks <= 0)
        {
            return 0;
        }

        long elapsedTicks = DateTime.UtcNow.Ticks - lastActiveTicks;
        float elapsedSeconds = elapsedTicks / (float)TimeSpan.TicksPerSecond;

        if (elapsedSeconds < 0f)
        {
            elapsedSeconds = 0f;
        }

        elapsedSeconds = Mathf.Min(elapsedSeconds, capSeconds);

        return Mathf.FloorToInt(elapsedSeconds * productionPerSec);
    }
}