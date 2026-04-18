using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSetting")]
public class GameSetting : ScriptableObject
{
    [Header("倒计时")] public int CountDownTime = 100;

    [Header("猫出现持续时间")] public int CatAppearanceDuration = 5;
}


public static class GameSettings
{
    private static GameSetting _gameSetting;

    public static GameSetting GameSetting =>
        _gameSetting ??= Resources.Load<GameSetting>("Setting");
}