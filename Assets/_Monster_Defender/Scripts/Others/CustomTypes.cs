using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    GameInit = 0,
    GameStart = 1,
    GamePause = 2,
    LevelFailed = 3,
    LevelCompleted = 4,
}

public enum ViewType
{
    HomeView = 0,
    IngameView = 1,
    EndgameView = 2,
}


public enum TankType
{
    Tank01 = 0,
    Tank02 = 1,
    Tank03 = 2,
    Tank04 = 3,
    Tank05 = 4,
    Tank06 = 5,
    Tank07 = 6,
    Tank08 = 7,
    Tank09 = 8,
    Tank10 = 9,
    Tank11 = 10,
    Tank12 = 11,
    Tank13 = 12,
    Tank14 = 13,
    Tank15 = 14,
    Tank16 = 15,
    Tank17 = 16,
    Tank18 = 17,
    Tank19 = 18,
    Tank20 = 19,
}

public enum EnemyType
{
    Monster01 = 0,
    Monster02 = 1,
    Monster03 = 2,
    Monster04 = 3,
    Monster05 = 4,
    Monster06 = 5,
    Monster07 = 6,
    Monster08 = 7,
    Monster09 = 8,
    Monster10 = 9,
    Monster11 = 10,
    Monster12 = 11,
    Monster13 = 12,
    Monster14 = 13,
    Monster15 = 14,
}

public enum BulletType
{
    Bullet01 = 0,
    Bullet02 = 1,
    Bullet03 = 2,
    Bullet04 = 3,
    Bullet05 = 4,
    Bullet06 = 5,
    Bullet07 = 6,
    Bullet08 = 7,
    Bullet09 = 8,
    Bullet10 = 9,
}

public enum BossType
{
    Boss01 = 0,
    Boss02 = 1,
    Boss03 = 2,
    Boss04 = 3,
    Boss05 = 4,
}

public class CustomTypes 
{
    
}


[System.Serializable]
public class TankItemConfig
{
    public TankType tankType = TankType.Tank01;
    public Sprite tankSprite = null;
    public int priceToPurchase = 50;
}
