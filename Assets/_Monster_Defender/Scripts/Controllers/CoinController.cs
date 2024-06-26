using ClawbearGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{


    /// <summary>
    /// Move this coin object to the target pos and add coin to CoinManager.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="time"></param>
    /// <param name="isUpdateCoin"></param>
    public void MoveToPos(Vector2 targetPos, float time, bool isUpdateCoin)
    {
        StartCoroutine(CRMoveToPos(targetPos, time, isUpdateCoin));
    }



    /// <summary>
    /// Coroutine move this coin object to the target pos and add coin to CoinManager.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="time"></param>
    /// <param name="isUpdateCoin"></param>
    /// <returns></returns>
    private IEnumerator CRMoveToPos(Vector2 targetPos, float time, bool isUpdateCoin)
    {
        float t = 0;
        float moveTime = time;
        Vector3 startPos = transform.position;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            transform.position = Vector3.Lerp(startPos, targetPos, factor);
            yield return null;
        }

        if (isUpdateCoin)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.CoinItem);
            IngameManager.Instance.UpdateCoins();
            CoinManager.Instance.UpdateCoins(1);
            gameObject.SetActive(false);
        }
    }
}
