using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEffectController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite[] effectSprites = null;



    /// <summary>
    /// Play the shoot effect.
    /// </summary>
    public void PlayShootEffect()
    {
        spriteRenderer.enabled = true;
        StartCoroutine(ShootEffect());
    }


    /// <summary>
    /// Coroutine change the sprite to create shoot effect.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootEffect()
    {
        spriteRenderer.sprite = effectSprites[Random.Range(0, effectSprites.Length)];
        yield return null;
        spriteRenderer.enabled = false;
    }
}
