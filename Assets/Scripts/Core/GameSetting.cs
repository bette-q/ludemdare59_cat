using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CatSpawnIntervalConfig
{
    [Header("剩余时间上限(包含)")] public int MaxRemainingTime = 120;
    [Header("剩余时间下限(不包含)")] public int MinRemainingTime = 100;
    [Header("最小刷猫间隔")] public float MinInterval = 5f;
    [Header("最大刷猫间隔")] public float MaxInterval = 6f;
    [Header("猫显示时长")] public float CatAppearanceDuration = 5f;

    public bool IsInRange(int remainingTime)
    {
        return remainingTime <= MaxRemainingTime && remainingTime > MinRemainingTime;
    }

    public float GetRandomInterval()
    {
        var min = Mathf.Max(0.1f, Mathf.Min(MinInterval, MaxInterval));
        var max = Mathf.Max(min, Mathf.Max(MinInterval, MaxInterval));
        return UnityEngine.Random.Range(min, max);
    }
}

[CreateAssetMenu(menuName = "GameSetting")]
public class GameSetting : ScriptableObject
{
    [Header("倒计时")] public int CountDownTime = 100;

    [Header("猫出现持续时间")] public float CatAppearanceDuration = 5f;

    [Header("默认刷猫间隔")] public float DefaultCatSpawnInterval = 5f;

    [Header("刷猫配置")] public List<CatSpawnIntervalConfig> CatSpawnIntervals = new();

    [Header("Cat Request Config")] public List<CatRequestDefinition> NormalCatRequests = new();

    private CatSpawnIntervalConfig GetCatSpawnConfig(int remainingTime)
    {
        if (CatSpawnIntervals == null)
        {
            return null;
        }

        for (var i = 0; i < CatSpawnIntervals.Count; i++)
        {
            var config = CatSpawnIntervals[i];
            if (config != null && config.IsInRange(remainingTime))
            {
                return config;
            }
        }

        return null;
    }

    public float GetCatSpawnInterval(int remainingTime)
    {
        var config = GetCatSpawnConfig(remainingTime);
        if (config != null)
        {
            return config.GetRandomInterval();
        }

        return Mathf.Max(0.1f, DefaultCatSpawnInterval);
    }

    public float GetCatAppearanceDuration(int remainingTime)
    {
        var config = GetCatSpawnConfig(remainingTime);
        if (config != null)
        {
            return Mathf.Max(0.1f, config.CatAppearanceDuration);
        }

        return Mathf.Max(0.1f, CatAppearanceDuration);
    }

    public CatRequestDefinition GetRandomNormalCatRequest()
    {
        if (NormalCatRequests == null || NormalCatRequests.Count == 0)
        {
            return null;
        }

        return NormalCatRequests[UnityEngine.Random.Range(0, NormalCatRequests.Count)];
    }
}


public static class GameSettings
{
    private static GameSetting _gameSetting;

    public static GameSetting GameSetting =>
        _gameSetting ??= Resources.Load<GameSetting>("Setting");
}
