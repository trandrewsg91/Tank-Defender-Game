using ClawbearGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public static IngameManager Instance { get; private set; }

    [Header("Ingame Configs")]
    [SerializeField] private List<TankItemConfig> listTankItemConfig = new List<TankItemConfig>();


    [Header("Ingame References")]
    [SerializeField] private Transform healthBar = null;
    [SerializeField] private SpriteRenderer backgroundSprite = null;
    [SerializeField] private List<TankSpawnController> ListTankSpawns = new List<TankSpawnController>();


    private LevelConfigSO levelConfig = null;
    private TankController selectedTank = null;
    private TankSpawnController originalTankSpawn = null;

    public GameState GameState { private set; get; }
    public List<TankItemConfig> ListTankItemConfig => listTankItemConfig;
    public int CurrentLevel;
    public bool IsActiveBoss { private set; get; }


    public float totalHealth = 0;
    public float currentHealth = 0f;
    public int enemyAmountInWave = 0;
    private int deadEnemyCount = 0;
    public int totalEnemyCount = 0;
    public int enemyWaveIndex = 0;
    public int bossWaveIndex = 0;
    private int deadBossCount = 0;
    public int currentCoins = 0;
    private int UIWaveIndex = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance = null;
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    private void Start()
    {
        Application.targetFrameRate = 60;
        GameState = GameState.GameInit;
        StartCoroutine(CRShowViewWithDelay(ViewType.IngameView, 0.05f));

        //Load level
        CurrentLevel = PlayerPrefs.GetInt(PlayerPrefsKey.LEVEL_KEY, 1);
        levelConfig = Resources.Load("Levels/" + CurrentLevel.ToString(), typeof(LevelConfigSO)) as LevelConfigSO;
        backgroundSprite.sprite = levelConfig.BackGroundSprite;

        //Setup health bar
        totalHealth = levelConfig.HealthAmount;
        currentHealth = levelConfig.HealthAmount;

        //Spawn the init tanks
        for (int i = 0; i < levelConfig.InitTanks.Count; i++)
        {
            SpawnTank(levelConfig.InitTanks[i]);
        }

        //Create the wave items for UI
        ViewManager.Instance.IngameView.CreateWaveItems(levelConfig.ListWaveConfig.Count, levelConfig.ListBossType.Count);

        //Init GameStart
        Invoke(nameof(GameStart), 0.15f);
    }

    private void Update()
    {
        if(GameState == GameState.GameStart)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && selectedTank == null)
            {
                float minDis = 1000f;
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                foreach(TankSpawnController tankSpawn in ListTankSpawns)
                {
                    if(tankSpawn.TankController != null && tankSpawn.TankController.IsMoving == false)
                    {
                        float distance = Vector2.Distance(tankSpawn.TankController.transform.position, touchPos);
                        if (distance < 0.7f && distance < minDis)
                        {
                            minDis = distance;
                            selectedTank = tankSpawn.TankController;
                            originalTankSpawn = tankSpawn;
                        }
                    }
                }

                if (selectedTank != null)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.SelectTank);
                    selectedTank.SetSelected(true, Vector2.zero, false);
                }
            }
            if (Input.GetMouseButton(0) && selectedTank != null)
            {
                float newY = Mathf.Clamp(mousePos.y, -9.8f, 0f);
                selectedTank.transform.position = new Vector3(mousePos.x, newY, 0f);
            }
            if (Input.GetMouseButtonUp(0) && selectedTank != null)
            {
                TankSpawnController newTankSpawn = GetClosestTankSpawn(selectedTank.transform.position);
                if (newTankSpawn == null)
                {
                    selectedTank.SetSelected(false, originalTankSpawn.transform.position, false);
                }
                else
                {
                    if (newTankSpawn.Equals(originalTankSpawn))
                    {
                        selectedTank.SetSelected(false, originalTankSpawn.transform.position, true);
                    }
                    else
                    {
                        if (newTankSpawn.TankController == null)
                        {
                            originalTankSpawn.TankController = null;
                            newTankSpawn.TankController = selectedTank;
                            selectedTank.SetSelected(false, newTankSpawn.transform.position, true);
                        }
                        else
                        {
                            if ((newTankSpawn.TankController.TankType == selectedTank.TankType) && (selectedTank.TankType != TankType.Tank20))
                            {
                                SoundManager.Instance.PlaySound(SoundManager.Instance.MergeTank);

                                TankType nextTankType = GetNextTankType(selectedTank.TankType);
                                newTankSpawn.TankController.gameObject.SetActive(false);
                                selectedTank.gameObject.SetActive(false);
                                originalTankSpawn.TankController = null;

                                TankController newTank = PoolManager.Instance.GetTankController(nextTankType);
                                newTank.transform.position = newTankSpawn.transform.position;
                                newTankSpawn.TankController = newTank;
                                newTank.OnTankInit();
                            }
                            else
                            {
                                selectedTank.SetSelected(false, originalTankSpawn.transform.position, false);
                            }
                        }
                    }
                }

                selectedTank = null;
                originalTankSpawn = null;
            }
        }
    }





    /// <summary>
    /// Coroutine show the view with delay time and callback.
    /// </summary>
    /// <param name="viewType"></param>
    /// <param name="delayTime"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator CRShowViewWithDelay(ViewType viewType, float delayTime, System.Action callback = null)
    {
        yield return new WaitForSeconds(delayTime);
        ViewManager.Instance.SetActiveView(viewType);
        yield return null;
        callback?.Invoke();
    }


    ////////////////////////////////////////////////// State Functions



    /// <summary>
    /// Init GameStart state.
    /// </summary>
    public void GameStart()
    {
        GameState = GameState.GameStart;
        StartCoroutine(CRSpawnNextEnemyWave(1f));
        ViewManager.Instance.IngameView.ShowTextForFirstWave();
        SoundManager.Instance.PlayMusic(levelConfig.BackgroundMusic);
        IsActiveBoss = false;
    }


    /// <summary>
    /// Init GamePause state
    /// </summary>
    public void GamePause()
    {
        GameState = GameState.GamePause;
    }


    /// <summary>
    /// Resume the game
    /// </summary>
    public void GameResume()
    {
        GameState = GameState.GameStart;
    }



    /// <summary>
    /// Level failed.
    /// </summary>
    public void LevelFailed()
    {
        GameState = GameState.LevelFailed;
        SoundManager.Instance.StopMusic(true);
        SoundManager.Instance.PlaySound(SoundManager.Instance.LevelFailed);
        StartCoroutine(CRShowViewWithDelay(ViewType.EndgameView, 1f, () =>
        {
            totalEnemyCount = deadEnemyCount + 10;
            ViewManager.Instance.EndgameView.UpdateStats(deadEnemyCount, totalEnemyCount, deadBossCount, levelConfig.ListBossType.Count, currentCoins);
        }));
    }



    /// <summary>
    /// Level failed.
    /// </summary>
    public void LevelCompleted()
    {
        GameState = GameState.LevelCompleted;
        SoundManager.Instance.StopMusic(true);
        SoundManager.Instance.PlaySound(SoundManager.Instance.LevelCompleted);
        StartCoroutine(CRShowViewWithDelay(ViewType.EndgameView, 1f, () =>
        {
            ViewManager.Instance.EndgameView.UpdateStats(deadEnemyCount, deadEnemyCount, deadBossCount, levelConfig.ListBossType.Count, currentCoins);
        }));

        CurrentLevel++;
        PlayerPrefs.SetInt(PlayerPrefsKey.LEVEL_KEY, CurrentLevel);
    }


    ////////////////////////////////////////////////// Private Functions



    /// <summary>
    /// Coroutine spawn the next enemy wave.
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private IEnumerator CRSpawnNextEnemyWave(float delayTime)
    {
        enemyAmountInWave = Random.Range(levelConfig.ListWaveConfig[enemyWaveIndex].minEnemyAmount, levelConfig.ListWaveConfig[enemyWaveIndex].maxEnemyAmount);
        int enemyNow = enemyAmountInWave;
        totalEnemyCount += enemyAmountInWave;
        int enemyOrder = 1000;
        WaveConfig waveConfig = levelConfig.ListWaveConfig[enemyWaveIndex];

        //Wait
        float timeCount = delayTime;
        while (timeCount > 0)
        {
            timeCount -= Time.deltaTime;

            //Stop at Pause state
            while (GameState == GameState.GamePause)
            {
                yield return null;
            }
        }

        for (int i = 0; i < enemyNow; i++)
        {
            EnemyController enemyController = PoolManager.Instance.GetEnemyController(GetEnemyType(waveConfig.enemyTypeConfigs));
            Vector3 spawnPos = Vector3.zero;
            spawnPos.y = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1.2f)).y;
            spawnPos.x = Random.Range(-4.5f, 4.5f);
            enemyController.transform.position = spawnPos;
            enemyController.OnEnemyInit(enemyOrder);
            enemyOrder--;
            yield return new WaitForSeconds(waveConfig.enemyDelayTime);
        }
    }


    /// <summary>
    /// Coroutine spawn the next boss wave.
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private IEnumerator CRSpawnNextBossWave(float delayTime)
    {
        BossType bossType = levelConfig.ListBossType[bossWaveIndex];

        //Wait
        float timeCount = delayTime;
        while (timeCount > 0)
        {
            timeCount -= Time.deltaTime;

            //Stop at Pause state
            while (GameState == GameState.GamePause)
            {
                yield return null;
            }
        }

        BossController bossController = PoolManager.Instance.GetBossController(bossType);
        Vector3 spawnPos = Vector3.zero;
        spawnPos.y = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1.2f)).y;
        spawnPos.x = Random.Range(-4.5f, 4.5f);
        bossController.transform.position = spawnPos;
        bossController.OnBossInit(1000);
    }



    /// <summary>
    /// Get the EnemyType based on List<EnemyTypeConfig>.
    /// </summary>
    /// <param name="enemyConfigs"></param>
    /// <returns></returns>
    private EnemyType GetEnemyType(List<EnemyTypeConfig> enemyConfigs)
    {
        //Calculate the total frequency
        float totalFreq = 0;
        foreach (EnemyTypeConfig configuration in enemyConfigs)
        {
            totalFreq += configuration.frequency;
        }

        float randomFreq = Random.Range(0, totalFreq);
        for (int i = 0; i < enemyConfigs.Count; i++)
        {
            if (randomFreq < enemyConfigs[i].frequency)
            {
                return enemyConfigs[i].enemyType;
            }
            else
            {
                randomFreq -= enemyConfigs[i].frequency;
            }
        }

        return enemyConfigs[0].enemyType;
    }


  
  

    /// <summary>
    /// Get the closest TankSpawnController with given position.
    /// </summary>
    /// <param name="tankPos"></param>
    /// <returns></returns>
    private TankSpawnController GetClosestTankSpawn(Vector3 tankPos)
    {
        float minDistance = 0.5f;
        TankSpawnController cur = null;
        for (int i = 0; i < ListTankSpawns.Count; i++)
        {
            float distance = Vector3.Distance(tankPos, ListTankSpawns[i].gameObject.transform.position);
            if ((distance < minDistance))
            {
                minDistance = distance;
                cur = ListTankSpawns[i];
            }
        }
        return cur;
    }



    /// <summary>
    /// Get the next TankType.
    /// </summary>
    /// <param name="oldType"></param>
    /// <returns></returns>
    private TankType GetNextTankType(TankType oldType)
    {
        TankType tankType = oldType;
        switch (oldType)
        {
            case TankType.Tank01:
                tankType = TankType.Tank02;
                break;
            case TankType.Tank02:
                tankType = TankType.Tank03;
                break;
            case TankType.Tank03:
                tankType = TankType.Tank04;
                break;
            case TankType.Tank04:
                tankType = TankType.Tank05;
                break;
            case TankType.Tank05:
                tankType = TankType.Tank06;
                break;
            case TankType.Tank06:
                tankType = TankType.Tank07;
                break;
            case TankType.Tank07:
                tankType = TankType.Tank08;
                break;
            case TankType.Tank08:
                tankType = TankType.Tank09;
                break;
            case TankType.Tank09:
                tankType = TankType.Tank10;
                break;
            case TankType.Tank10:
                tankType = TankType.Tank11;
                break;
            case TankType.Tank11:
                tankType = TankType.Tank12;
                break;
            case TankType.Tank12:
                tankType = TankType.Tank13;
                break;
            case TankType.Tank13:
                tankType = TankType.Tank14;
                break;
            case TankType.Tank14:
                tankType = TankType.Tank15;
                break;
            case TankType.Tank15:
                tankType = TankType.Tank16;
                break;
            case TankType.Tank16:
                tankType = TankType.Tank17;
                break;
            case TankType.Tank17:
                tankType = TankType.Tank18;
                break;
            case TankType.Tank18:
                tankType = TankType.Tank19;
                break;
            case TankType.Tank19:
                tankType = TankType.Tank20;
                break;
        }
        return tankType;
    }



    ////////////////////////////////////////////////// Public Functions





    /// <summary>
    /// Handle the health bar take damage.
    /// </summary>
    /// <param name="damage"></param>
    public void OnTakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, totalHealth);
        healthBar.localScale = new Vector3(currentHealth / totalHealth, 1f, 1f);

        if (currentHealth <= 0 && GameState == GameState.GameStart)
        {
            LevelFailed();
        }
    }
    /// <summary>
    /// Update the dead enemy by enemyAmountInWave--;
    /// </summary>
    public void UpdateDeadEnemy()
    {
        deadEnemyCount++;
        enemyAmountInWave--;
        if (enemyAmountInWave <= 0)
        {
            Debug.Log("Next");
            if (enemyWaveIndex == levelConfig.ListWaveConfig.Count - 1)
            {
                if (levelConfig.ListBossType.Count == 0)
                {
                    LevelCompleted();
                }
                else
                {
                    //Update the wave on UI
                    ViewManager.Instance.IngameView.OnWaveCompleted(UIWaveIndex);
                    UIWaveIndex++;

                    //Spawn the first boss
                    IsActiveBoss = true;
                    StartCoroutine(CRSpawnNextBossWave(1f));
                }
            }
            else
            {
                //Update the wave on UI
                ViewManager.Instance.IngameView.OnWaveCompleted(UIWaveIndex);
                UIWaveIndex++;

                enemyWaveIndex++;
                StartCoroutine(CRSpawnNextEnemyWave(1f));
            }
        }
    }


    /// <summary>
    /// Update dead boss.
    /// </summary>
    public void UpdateDeadBoss()
    {
        deadBossCount++;
        if (bossWaveIndex == levelConfig.ListBossType.Count - 1)
        {
            //Update the wave on UI
            ViewManager.Instance.IngameView.OnWaveCompleted(UIWaveIndex);

            //Kill all the bosses -> complete level
            LevelCompleted();
        }
        else
        {
            //Update the wave on UI
            ViewManager.Instance.IngameView.OnWaveCompleted(UIWaveIndex);
            UIWaveIndex++;

            //Spawn the next boss
            bossWaveIndex++;
            StartCoroutine(CRSpawnNextBossWave(1f));
        }
    }


    /// <summary>
    /// Spawn the tank with given TankType.
    /// </summary>
    /// <param name="tankType"></param>
    public void SpawnTank(TankType tankType)
    {
        for (int i = 0; i < ListTankSpawns.Count; i++)
        {
            TankSpawnController tankSpawn = ListTankSpawns[i];
            if (tankSpawn.TankController == null)
            {
                TankController tank = PoolManager.Instance.GetTankController(tankType);
                tankSpawn.TankController = tank;
                tank.gameObject.transform.position = tankSpawn.gameObject.transform.position;
                tank.OnTankInit();
                break;
            }
        }
    }



    /// <summary>
    /// Update currentCoins by +1.
    /// </summary>
    public void UpdateCoins()
    {
        currentCoins++;
    }


    /// <summary>
    /// Is create the coin from dead enemy.
    /// </summary>
    /// <returns></returns>
    public bool IsCreateCoinFromDeadEnemy()
    {
        return Random.value < levelConfig.ListWaveConfig[enemyWaveIndex].coinFrequency;
    }



    /// <summary>
    /// Chekc the grid is full of tanks.
    /// </summary>
    /// <returns></returns>
    public bool IsFullOfTanks()
    {
        bool isFull = true;
        foreach (TankSpawnController tankSpawn in ListTankSpawns)
        {
            if (tankSpawn.TankController == null) { isFull = false; break; }
        }
        return isFull;
    }
}
