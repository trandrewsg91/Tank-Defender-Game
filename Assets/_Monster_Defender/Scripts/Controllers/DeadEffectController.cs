using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEffectController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite[] enemyDieSprites = null;



    /// <summary>
    /// Play the enemy dead effect.
    /// </summary>
    public void PlayDeadEffect()
    {
        StartCoroutine(DeadEffect());
    }


    /// <summary>
    /// Coroutine change the sprite to create dead effect.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeadEffect()
    {
        for (int i = 0; i < enemyDieSprites.Length; i++)
        {
            spriteRenderer.sprite = enemyDieSprites[i];
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}
