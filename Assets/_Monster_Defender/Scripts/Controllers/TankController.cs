using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [Header("Tank Configs")]
    [SerializeField] private float attackRate = 0.5f;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float minDamageAmount = 1f;
    [SerializeField] private float maxDamageAmount = 3f;
    [SerializeField] private float bulletMovementSpeed = 35f;

    [Header("Tank References")]
    [SerializeField] private TankType tankType = TankType.Tank01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite tankIdleSprite = null;
    [SerializeField] private Transform[] bulletSpawns;
    [SerializeField] private ShootEffectController[] shootEffects = null;
    [SerializeField] private Sprite[] animationSprites = null;

    public TankType TankType { get => tankType; }
    public int SortingOder { set { spriteRenderer.sortingOrder = value; } }

    public bool IsMoving { private set; get; }

    private float damageAmount = 0f;



    /// <summary>
    /// Init this tank.
    /// </summary>
    public void OnTankInit()
    {
        IsMoving = false;
        damageAmount = UnityEngine.Random.Range(minDamageAmount, maxDamageAmount);
        StartCoroutine(CRAttackEnemy());

        //Disable shoot effects
        foreach(ShootEffectController shootEffect in shootEffects)
        {
            shootEffect.gameObject.SetActive(false);
        }
    }



    /// <summary>
    /// Set selected for this tank.
    /// </summary>
    /// <param name="selected"></param>
    /// <param name="targetPos"></param>
    /// <param name="snapToTarget"></param>
    public void SetSelected(bool selected, Vector2 targetPos, bool snapToTarget)
    {
        if (selected)
        {
            StopAllCoroutines();
            transform.up = Vector2.up;
            spriteRenderer.sprite = tankIdleSprite;
        }
        else
        {
            if (snapToTarget) 
            { 
                transform.position = targetPos;
                StartCoroutine(CRAttackEnemy());
            }
            else 
            {
                IsMoving = true;
                StartCoroutine(CRMoveToTarget(targetPos));
            }
        }
    }


    /// <summary>
    /// Coroutine move this tank to the target position.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private IEnumerator CRMoveToTarget(Vector2 targetPos)
    {
        float t = 0;
        float moveTime = (Vector3.Distance(transform.position, targetPos) / 20f);
        Vector3 startVector3 = transform.position;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            Vector3 newPos = Vector3.Lerp(startVector3, targetPos, factor);
            transform.position = newPos;
            yield return null;
        }
        IsMoving = false;
        StartCoroutine(CRAttackEnemy());
    }



    /// <summary>
    /// Coroutine attack the boss.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRAttackEnemy()
    {
        EnemyController targetEnemy = null;
        BossController targetBoss = null;
        while (gameObject.activeSelf)
        {
            //Stop at Pause state
            while (IngameManager.Instance.GameState == GameState.GamePause)
            {
                yield return null;
            }


            if (!IngameManager.Instance.IsActiveBoss)
            {
                //Find the closest boss
                List<EnemyController> activeEnemies = PoolManager.Instance.FindActiveEnemies();
                foreach (EnemyController enemy in activeEnemies)
                {
                    float distanceToTank = Vector2.Distance(transform.position, enemy.transform.position);
                    if ((distanceToTank <= attackRange) && (Mathf.Abs(enemy.transform.position.x - transform.position.x) <= 4))
                    {
                        targetEnemy = enemy;
                        break;
                    }
                }


                float timeCount = 0;
                while (targetEnemy != null && targetEnemy.gameObject.activeSelf && gameObject.activeSelf)
                {
                    //Stop at Pause state
                    while (IngameManager.Instance.GameState != GameState.GameStart)
                    {
                        yield return null;
                    }


                    //Rotate to the boss
                    transform.up = (targetEnemy.transform.position - transform.position).normalized;

                    timeCount -= Time.deltaTime;
                    if (timeCount <= 0)
                    {
                        //Reset timeCount
                        timeCount = attackRate;

                        //Play shoot effects and spawn bullets
                        StartCoroutine(CRPlayShootEffectsAndSpawnBullets());
                    }

                    yield return null;
                    if (!targetEnemy.gameObject.activeSelf)
                    {
                        targetEnemy = null;

                        //Rotate back to Vector2.up and return idle sprite
                        transform.up = Vector2.up;
                        spriteRenderer.sprite = tankIdleSprite;

                        break;
                    }
                }
            }
            else
            {
                //Find the closest boss
                List<BossController> activeBosses = PoolManager.Instance.FindActiveBosses();
                foreach (BossController boss in activeBosses)
                {
                    float distanceToTank = Vector2.Distance(transform.position, boss.transform.position);
                    if ((distanceToTank <= attackRange) && (Mathf.Abs(boss.transform.position.x - transform.position.x) <= 4))
                    {
                        targetBoss = boss;
                        break;
                    }
                }



                float timeCount = 0;
                while (targetBoss != null && targetBoss.gameObject.activeSelf && gameObject.activeSelf)
                {
                    //Stop at Pause state
                    while (IngameManager.Instance.GameState != GameState.GameStart)
                    {
                        yield return null;
                    }


                    //Rotate to the boss
                    transform.up = (targetBoss.transform.position - transform.position).normalized;

                    timeCount -= Time.deltaTime;
                    if (timeCount <= 0)
                    {
                        //Reset timeCount
                        timeCount = attackRate;

                        //Play shoot effects and spawn bullets
                        StartCoroutine(CRPlayShootEffectsAndSpawnBullets());
                    }

                    yield return null;
                    if (!targetBoss.gameObject.activeSelf)
                    {
                        targetBoss = null;

                        //Rotate back to Vector2.up and return idle sprite
                        transform.up = Vector2.up;
                        spriteRenderer.sprite = tankIdleSprite;

                        break;
                    }
                }
            }

            yield return null;
        }
    }



    /// <summary>
    /// Coroutine play the shoot effects and spawn the bullets.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRPlayShootEffectsAndSpawnBullets()
    {
        //Play the shoot effect.
        foreach (ShootEffectController shootEffect in shootEffects)
        {
            shootEffect.gameObject.SetActive(true);
            shootEffect.PlayShootEffect();
        }

        //play the animation
        for (int i = 0; i < animationSprites.Length; i++)
        {
            spriteRenderer.sprite = animationSprites[i];
            yield return null;
        }

        //Disable shoot effects
        foreach (ShootEffectController shootEffect in shootEffects)
        {
            shootEffect.gameObject.SetActive(true);
        }


        //Spawn the bullet
        foreach (Transform trans in bulletSpawns)
        {
            BulletController bulletspawn = PoolManager.Instance.GetBulletController(tankType);
            bulletspawn.transform.position = trans.position;
            bulletspawn.transform.up = transform.up;
            bulletspawn.gameObject.SetActive(true);
            bulletspawn.OnInitBullet(damageAmount, bulletMovementSpeed);
        }
    }
}
