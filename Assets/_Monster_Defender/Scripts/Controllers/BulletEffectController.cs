using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffectController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite[] bulletExplodeSprites = null;


    /// <summary>
    /// Play the muzzle flash effect.
    /// </summary>
    public void PlayBulletExplodeEffect()
    {
        spriteRenderer.sprite = bulletExplodeSprites[0];
        StartCoroutine(CRPlaySprites());
    }


    /// <summary>
    /// Coroutine change the animationSprites to create explode effect.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRPlaySprites()
    {
        for (int i = 0; i < bulletExplodeSprites.Length; i++)
        {
            spriteRenderer.sprite = bulletExplodeSprites[i];
            yield return null;
        }
        gameObject.SetActive(false);
    }
}