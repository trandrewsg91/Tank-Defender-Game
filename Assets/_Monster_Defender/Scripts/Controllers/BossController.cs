using ClawbearGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("Boss Configs")]
    [SerializeField] private float attackRate = 0.5f;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float minDamageAmount = 2f;
    [SerializeField] private float maxDamageAmount = 6f;
    [SerializeField] private float minHealthAmount = 25f;
    [SerializeField] private float maxHealthAmount = 40f;
    [SerializeField] private int minCoinBonusAmount = 5;
    [SerializeField] private int maxCoinBonusAmount = 5;

    [Header("Boss References")]
    [SerializeField] private BossType bossType = BossType.Boss01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Material normalMaterial = null;
    [SerializeField] private Material whiteMaterial = null;
    [SerializeField] private Image healthBar = null;
    [SerializeField] private Sprite[] animationSprites = null;

    public BossType BossType => bossType;
    public bool IsDead { private set; get; }

    private float bossDamage;
    private float totalHealth;
    private float currentHealth;
    private bool isTakingDamage = false;



    /// <summary>
    /// Handle init the boss.
    /// </summary>
    /// <param name="sortingOrder"></param>
    public void OnBossInit(int sortingOrder)
    {
        IsDead = false;
        isTakingDamage = false;
        bossDamage = Random.Range(minDamageAmount, maxDamageAmount);
        totalHealth = Random.Range(minHealthAmount, maxHealthAmount); ;
        currentHealth = totalHealth;
        healthBar.fillAmount = 1f;
        spriteRenderer.material = normalMaterial;
        spriteRenderer.sortingOrder = sortingOrder;
        StartCoroutine(CRPlayAnimation());
        StartCoroutine(CRMoveDown());
    }



    /// <summary>
    /// Coroutine play the bounce animation.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRPlayAnimation()
    {
        while (gameObject.activeSelf)
        {
            for (int i = 0; i < animationSprites.Length; i++)
            {
                spriteRenderer.sprite = animationSprites[i];
                yield return new WaitForSeconds(0.02f);
            }

            if (animationSprites.Length == 0) { yield return null; }
        }
    }



    /// <summary>
    /// Coroutine move this boss down toward the tanks.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRMoveDown()
    {
        while (gameObject.activeSelf)
        {
            //Stop moving on Pause game state
            while (IngameManager.Instance.GameState == GameState.GamePause)
            {
                yield return null;
            }

            //CRMoveDown down
            transform.position += Vector3.down * movementSpeed * Time.deltaTime;
            yield return null;

            //Stop moving at the health bar -> start the attack
            if (transform.position.y <= -3.15f)
            {
                StartCoroutine(CROnAttack());
                yield break;
            }
        }
    }



    /// <summary>
    /// Coroutine attack the health bar.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CROnAttack()
    {
        while(gameObject.activeSelf)
        {
            //Stop attacking on Pause game state
            while (IngameManager.Instance.GameState == GameState.GamePause)
            {
                yield return null;
            }

            IngameManager.Instance.OnTakeDamage(bossDamage);

            //Create damage effect
            DamageEffectController damageEffect = PoolManager.Instance.GetDamageEffectController();
            damageEffect.transform.position = transform.position + Vector3.up;
            damageEffect.gameObject.SetActive(true);
            damageEffect.ShowDamageText(bossDamage);


            yield return new WaitForSeconds(attackRate);
        }
    }


    /// <summary>
    /// Handle this boss take damage.
    /// </summary>
    /// <param name="damage"></param>
    public void OnTakeDamage(float damage)
    {
        if (!isTakingDamage)
        {
            isTakingDamage = true;
            StartCoroutine(CROnTakeDamage(damage));
        }
    }



    /// <summary>
    /// Coroutine take damage.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    private IEnumerator CROnTakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, totalHealth);
        healthBar.fillAmount = currentHealth / totalHealth;
        if (currentHealth <= 0)
        {
            IsDead = true;

            SoundManager.Instance.PlaySound(SoundManager.Instance.BossExplode);

            //Update dead enemy
            IngameManager.Instance.UpdateDeadBoss();

            //Create the effect
            DeadEffectController deadEffect = PoolManager.Instance.GetDeadEffectController();
            deadEffect.transform.position = transform.position;
            deadEffect.PlayDeadEffect();


            spriteRenderer.gameObject.SetActive(false);

            //Create the coins
            List<CoinController> listCoin = new List<CoinController>();
            int coinAmount = Random.Range(minCoinBonusAmount, maxCoinBonusAmount);
            for(int i = 0; i < coinAmount; i++)
            {
                CoinController coinController = PoolManager.Instance.GetCoinController();
                coinController.transform.position = transform.position;

                Vector2 targetPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;
                coinController.MoveToPos(targetPos, 0.1f, false);
                listCoin.Add(coinController);
                yield return null;
            }

            yield return new WaitForSeconds(0.15f);
            foreach (CoinController coin in listCoin)
            {
                coin.MoveToPos(ViewManager.Instance.IngameView.CoinTextWorldPos(), 0.5f, true);
                yield return null;
            }

            //Disable this boss
            spriteRenderer.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            spriteRenderer.material = whiteMaterial;
            yield return null;
            spriteRenderer.material = normalMaterial;
            isTakingDamage = false;
        }
    }
}
