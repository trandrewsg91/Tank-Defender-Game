using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance {  get; private set; }

    [SerializeField] private CoinController coinControllerPrefab = null;
    [SerializeField] private DeadEffectController deadEffectControllerPrefab = null;
    [SerializeField] private DamageEffectController damageEffectControllerPrefab = null;
    [SerializeField] private BulletEffectController bulletEffectControllerPrefab = null;
    [SerializeField] private EnemyController[] enemyControllerPrefabs = null;
    [SerializeField] private TankController[] tankControllerPrefabs = null;
    [SerializeField] private BulletController[] bulletControllerPrefabs = null;
    [SerializeField] private BossController[] bossControllerPrefabs = null;


    private List<CoinController> listCoinController = new List<CoinController>();
    private List<TankController> listTankController = new List<TankController>();
    private List<BulletController> listBulletController = new List<BulletController>();
    private List<EnemyController> listEnemyController = new List<EnemyController>();
    private List<BossController> listBossController = new List<BossController>();
    private List<DeadEffectController> listDeadEffectController = new List<DeadEffectController>();
    private List<DamageEffectController> listDamageEffectController = new List<DamageEffectController>();
    private List<BulletEffectController> listBulletEffectController = new List<BulletEffectController>();

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



    /// <summary>
    /// Find all active enemies.
    /// </summary>
    /// <returns></returns>
    public List<EnemyController> FindActiveEnemies()
    {
        List<EnemyController> listResult = new List<EnemyController>();
        foreach(EnemyController enemy in listEnemyController)
        {
            if (enemy.gameObject.activeSelf) { listResult.Add(enemy); }
        }
        return listResult;
    }



    /// <summary>
    /// Find all active bosses.
    /// </summary>
    /// <returns></returns>
    public List<BossController> FindActiveBosses()
    {
        List<BossController> listResult = new List<BossController>();
        foreach (BossController boss in listBossController)
        {
            if (boss.gameObject.activeSelf && !boss.IsDead) { listResult.Add(boss); }
        }
        return listResult;
    }



    /// <summary>
    /// Get a CoinController object.
    /// </summary>
    /// <returns></returns>
    public CoinController GetCoinController()
    {
        //Find the object in the list
        CoinController coinController = listCoinController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

        if (coinController == null)
        {
            //Instantiate the dead effect
            coinController = Instantiate(coinControllerPrefab, Vector3.zero, Quaternion.identity);
            listCoinController.Add(coinController);
        }

        coinController.gameObject.SetActive(true);
        return coinController;
    }



    /// <summary>
    /// Get an BossController object.
    /// </summary>
    /// <param name="bossType"></param>
    /// <returns></returns>
    public BossController GetBossController(BossType bossType)
    {
        //Find object in the list
        BossController resultBoss = listBossController.Where(a => !a.gameObject.activeSelf && a.BossType == bossType).FirstOrDefault();

        if (resultBoss == null)
        {
            //Instantiate the boss
            BossController prefab = bossControllerPrefabs.Where(a => a.BossType.Equals(bossType)).FirstOrDefault();
            resultBoss = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            listBossController.Add(resultBoss);
        }

        resultBoss.gameObject.SetActive(true);
        return resultBoss;
    }



    /// <summary>
    /// Get an EnemyController object.
    /// </summary>
    /// <param name="enemyType"></param>
    /// <returns></returns>
    public EnemyController GetEnemyController(EnemyType enemyType)
    {
        //Find object in the list
        EnemyController resultEnemy = listEnemyController.Where(a => a.EnemyType.Equals(enemyType) && !a.gameObject.activeSelf).FirstOrDefault();

        if (resultEnemy == null)
        {
            //Instantiate the boss
            EnemyController prefab = enemyControllerPrefabs.Where(a => a.EnemyType.Equals(enemyType)).FirstOrDefault();
            resultEnemy = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            listEnemyController.Add(resultEnemy);
        }
        resultEnemy.gameObject.SetActive(true);
        return resultEnemy;
    }


    /// <summary>
    /// Get a TankController object.
    /// </summary>
    /// <param name="tankType"></param>
    /// <returns></returns>
    public TankController GetTankController(TankType tankType)
    {
        //Find object in the list
        TankController resultTank = listTankController.Where(a => a.TankType.Equals(tankType) && !a.gameObject.activeSelf).FirstOrDefault(); ;

        if (resultTank == null)
        {
            //Instantiate the tank
            TankController prefab = tankControllerPrefabs.Where(a => a.TankType.Equals(tankType)).FirstOrDefault();
            resultTank = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            listTankController.Add(resultTank);
        }
        resultTank.gameObject.SetActive(true);
        return resultTank;
    }


    /// <summary>
    /// Get a BulletController object.
    /// </summary>
    /// <param name="tankType"></param>
    /// <returns></returns>
    public BulletController GetBulletController(TankType tankType)
    {
        //Find object in the list
        BulletController resultBullet = listBulletController.Where(a => a.TankType.Equals(tankType) && !a.gameObject.activeSelf).FirstOrDefault();

        if(resultBullet == null)
        {
            //Instantiate the bullet
            BulletController prefab = bulletControllerPrefabs.Where(a => a.TankType.Equals(tankType)).FirstOrDefault();
            resultBullet = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            listBulletController.Add(resultBullet);
        }
        resultBullet.gameObject.SetActive(true);
        return resultBullet;
    }


    /// <summary>
    /// Get a DeadEffectController object.
    /// </summary>
    /// <returns></returns>
    public DeadEffectController GetDeadEffectController()
    {
        //Find the object in the list
        DeadEffectController deadEffect = listDeadEffectController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

        if (deadEffect == null)
        {
            //Instantiate the dead effect
            deadEffect = Instantiate(deadEffectControllerPrefab, Vector3.zero, Quaternion.identity);
            listDeadEffectController.Add(deadEffect);
        }

        deadEffect.gameObject.SetActive(true);
        return deadEffect;
    }


    /// <summary>
    /// Get a DamageEffectController object.
    /// </summary>
    /// <returns></returns>
    public DamageEffectController GetDamageEffectController()
    {
        //Find the object in the list
        DamageEffectController damageEffect = listDamageEffectController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

        if(damageEffect == null)
        {
            //Instantiate the damage effect
            damageEffect = Instantiate(damageEffectControllerPrefab, Vector3.zero, Quaternion.identity);
            listDamageEffectController.Add(damageEffect);
        }

        damageEffect.gameObject.SetActive(true);
        return damageEffect;
    }


    /// <summary>
    /// Get a BulletEffectController object.
    /// </summary>
    /// <returns></returns>
    public BulletEffectController GetBulletEffectController()
    {
        //Find the object in the list
        BulletEffectController bulletEffect = listBulletEffectController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

        if (bulletEffect == null)
        {
            //Instantiate the damage effect
            bulletEffect = Instantiate(bulletEffectControllerPrefab, Vector3.zero, Quaternion.identity);
            listBulletEffectController.Add(bulletEffect);
        }

        bulletEffect.gameObject.SetActive(true);
        return bulletEffect;
    }
}
