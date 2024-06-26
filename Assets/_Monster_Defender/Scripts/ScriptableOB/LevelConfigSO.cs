using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "LevelConfiguration/LevelConfig", order = 1)]
public class LevelConfigSO : ScriptableObject
{
    public AudioClip BackgroundMusic = null;
    public Sprite BackGroundSprite = null;
    public float HealthAmount = 0f;
    public List<TankType> InitTanks = new List<TankType>();
    public List<WaveConfig> ListWaveConfig = new List<WaveConfig>();
    public List<BossType>ListBossType = new List<BossType>();
}


[Serializable]
public class WaveConfig
{
    public int minEnemyAmount = 8;
    public int maxEnemyAmount = 15;
    public float enemyDelayTime = 1f;
    [Range(0f, 1f)] public float coinFrequency = 0.5f;
    public List<EnemyTypeConfig> enemyTypeConfigs = new List<EnemyTypeConfig>();
}


[Serializable]
public class EnemyTypeConfig
{
    public EnemyType enemyType = EnemyType.Monster10;
    [Range(0f, 1f)] public float frequency = 0.5f;
}
