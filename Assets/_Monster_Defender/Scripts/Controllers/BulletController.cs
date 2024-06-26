using ClawbearGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private TankType tankType = TankType.Tank01;
    [SerializeField] private LayerMask enemyLayerMask = new LayerMask();
    [SerializeField] private LayerMask bossLayerMask = new LayerMask();
    public TankType TankType { get => tankType; }
    private float bulletDamage = 0;
    private float bulletSpeed = 0;

    /// <summary>
    /// Init this bullet with parameters.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="speed"></param>
    public void OnInitBullet(float damage, float speed)
    {
        bulletSpeed = speed;
        bulletDamage = damage;
        StartCoroutine(CRMoveBullet());
    }


    /// <summary>
    /// Coroutine move this bullet by it up vector.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRMoveBullet()
    {
        while (gameObject.activeSelf)
        {
            //Stop moving on Pause game state 
            while (IngameManager.Instance.GameState == GameState.GamePause)
            {
                yield return null;
            }

            //Move using transform.up
            transform.position += transform.up * bulletSpeed * Time.deltaTime;


            //Check collide with enemy
            Collider2D enemyCollider2D = Physics2D.OverlapCircle(transform.position + transform.up * 0.1f, 0.15f, enemyLayerMask);
            if (enemyCollider2D != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.BulletExplode);

                //Enemy take damage
                EnemyController enemy = enemyCollider2D.gameObject.GetComponent<EnemyController>();
                enemy.OnTakeDamage(bulletDamage);

                //Play effect and siable this bullet
                BulletEffectController bulletEffect = PoolManager.Instance.GetBulletEffectController();
                bulletEffect.transform.position = transform.position;
                bulletEffect.PlayBulletExplodeEffect();
                gameObject.SetActive(false);
            }

            //Check collide with boss
            Collider2D bossCollider2D = Physics2D.OverlapCircle(transform.position + transform.up * 0.1f, 0.15f, bossLayerMask);
            if (bossCollider2D != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.BulletExplode);

                //Boss take damage
                BossController boss = bossCollider2D.gameObject.GetComponent<BossController>();
                boss.OnTakeDamage(bulletDamage);

                //Play effect and siable this bullet
                BulletEffectController bulletEffect = PoolManager.Instance.GetBulletEffectController();
                bulletEffect.transform.position = transform.position;
                bulletEffect.PlayBulletExplodeEffect();
                gameObject.SetActive(false);
            }


            //Check and disable this bullet
            Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPos.x >= 1.2f || viewportPos.x <= -0.2f || viewportPos.y >= 1.2f)
            {
                gameObject.SetActive(false);
            }
            yield return null;
        }
    }
    
}
