using ClawbearGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Configs")]
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float minDamageAmount = 1f;
    [SerializeField] private float maxDamageAmount = 2f;
    [SerializeField] private float minHealthAmount = 5f;
    [SerializeField] private float maxHealthAmount = 10f;

    [Header("Enemy References")]
    [SerializeField] private EnemyType enemyType = EnemyType.Monster01;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Image healthBar = null;
    [SerializeField] private Material normalMaterial = null;
    [SerializeField] private Material whiteMaterial = null;
    [SerializeField] private Sprite[] animationSprites = null;

    private float enemyDamage;
    private float totalHealth;
    public float currentHealth;
    private bool isTakingDamage = false;
    private bool isOnScreen = false;

    public EnemyType EnemyType { get => enemyType; }




    /// <summary>
    /// Handle init the enemy.
    /// </summary>
    /// <param name="sortingOrder"></param>
    public void OnEnemyInit(int sortingOrder)
    {
        isOnScreen = false;
        isTakingDamage = false;
        healthBar.fillAmount = 1f;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.material = normalMaterial;
        enemyDamage = Random.Range(minDamageAmount, maxDamageAmount);
        totalHealth = Random.Range(minHealthAmount, maxHealthAmount);
        currentHealth = totalHealth;
        StartCoroutine(CRPlayAnimation());
        StartCoroutine(CRMoveDown());
    }



    /// <summary>
    /// Coroutine play the animation by changing the animationSprites.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRPlayAnimation()
    {
        while (gameObject.activeSelf)
        {
            for (int i = 0; i < animationSprites.Length; i++)
            {
                spriteRenderer.sprite = animationSprites[i];
                yield return new WaitForSeconds(0.01f);
            }

            if (animationSprites.Length == 0) { yield return null; }
        }
    }




    /// <summary>
    /// Coroutine move this enemy down toward the tanks.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRMoveDown()
    {
        while (gameObject.activeSelf)
        {
            //Stop moving on Pause game state
            while (IngameManager.Instance.GameState != GameState.GameStart)
            {
                yield return null;
            }

            //CRMoveDown down
            transform.position += Vector3.down * movementSpeed * Time.deltaTime;
            yield return null;

            if (!isOnScreen)
            {
                Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
                if (viewportPos.y <= 0.95)
                {
                    isOnScreen = true;
                }
            }


            //Stop moving at the health bar -> start the attack
            if (transform.position.y <= -4f)
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
        while (gameObject.activeSelf)
        {
            //Stop attacking on Pause game state
            while (IngameManager.Instance.GameState != GameState.GameStart)
            {
                yield return null;
            }


            IngameManager.Instance.OnTakeDamage(enemyDamage);

            //Create damage text effect
            DamageEffectController damageEffect = PoolManager.Instance.GetDamageEffectController();
            damageEffect.transform.position = transform.position + Vector3.down * 0.6f;
            damageEffect.gameObject.SetActive(true);
            damageEffect.ShowDamageText(enemyDamage);

            //Wait attack rate
            yield return new WaitForSeconds(attackRate);
        }
    }
        
    
   


    /// <summary>
    /// Handle this enemy take damage.
    /// </summary>
    /// <param name="damage"></param>
    public void OnTakeDamage(float damage)
    {
        if (!isTakingDamage && isOnScreen)
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
            SoundManager.Instance.PlaySound(SoundManager.Instance.EnemyExplode);

            //Update dead enemy
            IngameManager.Instance.UpdateDeadEnemy();

            //Create the effect
            DeadEffectController deadEffect = PoolManager.Instance.GetDeadEffectController();
            deadEffect.transform.position = transform.position;
            deadEffect.PlayDeadEffect();

            //Create a coin
            if (IngameManager.Instance.IsCreateCoinFromDeadEnemy())
            {
                CoinController coinController = PoolManager.Instance.GetCoinController();
                coinController.transform.position = transform.position;
                coinController.MoveToPos(ViewManager.Instance.IngameView.CoinTextWorldPos(), 1f, true);
            }


            //Disable this enemy
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
