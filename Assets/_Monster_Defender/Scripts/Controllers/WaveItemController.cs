using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveItemController : MonoBehaviour
{
    [SerializeField] private Image itemImage = null;
    [SerializeField] private Image coverImage = null;
    [SerializeField] private Transform corverTrans = null;
    [SerializeField] private Transform sliderTrans = null;
    [SerializeField] private Sprite bossSprite;
    [SerializeField] private Sprite dotSprite;



    /// <summary>
    /// Setup the sprite for this wave item.
    /// </summary>
    /// <param name="index"></param>
    public void SetupSprite(int index)
    {
        itemImage.sprite = index == 0 ? dotSprite : bossSprite;
        coverImage.sprite = index == 0 ? dotSprite : bossSprite;
    }



    /// <summary>
    /// Handle active this wave item.
    /// </summary>
    /// <param name="active"></param>
    public void OnActive(bool active)
    {
        corverTrans.gameObject.SetActive(!active);
        sliderTrans.localScale = new Vector3(0f, 1f, 1f);
    }



    /// <summary>
    /// Update the slider of this wave item.
    /// </summary>
    public void UpdateSlider()
    {
        StartCoroutine(CRUpdateSlider());
    }



    /// <summary>
    /// Coroutine update the slider image.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRUpdateSlider()
    {
        float t = 0;
        float updateTime = 0.5f;
        while (t < updateTime)
        {
            t += Time.deltaTime;
            float factor = t / updateTime;
            sliderTrans.localScale = Vector3.Lerp(new Vector3(0f, 1f, 1f), Vector3.one, factor);
            yield return null;
        }
    }
}
