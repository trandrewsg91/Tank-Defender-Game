using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffectController : MonoBehaviour
{
    [SerializeField] private Text damageText = null;
    [SerializeField] private CanvasGroup canvasGroup;



    /// <summary>
    /// Show the damage text effect.
    /// </summary>
    /// <param name="damage"></param>
    public void ShowDamageText(float damage)
    {
        damageText.text = "-" + System.Math.Round(damage, 2).ToString();
        StartCoroutine(CRMoveUp());

    }


    /// <summary>
    /// Coroutine move this object up and fade out.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRMoveUp()
    {
        float t = 0;
        float moveTime = 1f;
        Vector3 startVector3 = transform.position;
        Vector3 endVector3 = transform.position + Vector3.up * Time.deltaTime * 100;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            Vector3 newPos = Vector3.Lerp(startVector3, endVector3, factor);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, factor);
            transform.position = newPos;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        gameObject.SetActive(false);
    }
}
