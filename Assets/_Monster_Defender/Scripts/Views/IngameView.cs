using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using ClawbearGames;
public class IngameView : BaseView
{
    [SerializeField] private Text levelText = null;
    [SerializeField] private Text waveText = null;
    [SerializeField] private Text coinsText = null;
    [SerializeField] private Button closeButton = null;
    [SerializeField] private RectTransform wavePanelTrans = null;
    [SerializeField] private RectTransform contentTrans = null;
    [SerializeField] private Transform wavesPanelTrans = null;
    [SerializeField] private Transform tanksPanelTrans = null;
    [SerializeField] private WaveItemController waveItemControllerPrefab = null;
    [SerializeField] private TankItemController tankItemControllerPrefab = null;


    private List<WaveItemController> listActiveWaveItem = new List<WaveItemController>();
    private List<TankItemController> listActiveTankItem = new List<TankItemController>();
    private List<WaveItemController> listWaveItemController = new List<WaveItemController>();
    private List<TankItemController> listTankItemController = new List<TankItemController>();


    private void Update()
    {
        coinsText.text = CoinManager.Instance.Coins.ToString();
    }




    /// <summary>
    /// ////////////////////////////////////////////// Private Functions
    /// </summary>


    /// <summary>
    /// Get a WaveItemController object.
    /// </summary>
    /// <returns></returns>
    private WaveItemController GetWaveItemController()
    {
        //Find the object in the list
        WaveItemController waveItem = listWaveItemController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

        if (waveItem == null)
        {
            //Instantiate the dead effect
            waveItem = Instantiate(waveItemControllerPrefab, Vector3.zero, Quaternion.identity);
            listWaveItemController.Add(waveItem);
        }

        waveItem.gameObject.SetActive(true);
        return waveItem;
    }



    /// <summary>
    /// Get a TankItemController object.
    /// </summary>
    /// <returns></returns>
    private TankItemController GetTankItemController()
    {
        //Find the object in the list
        TankItemController tankItem = listTankItemController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

        if (tankItem == null)
        {
            //Instantiate the dead effect
            tankItem = Instantiate(tankItemControllerPrefab, Vector3.zero, Quaternion.identity);
            listTankItemController.Add(tankItem);
        }

        tankItem.gameObject.SetActive(true);
        return tankItem;
    }




    /// <summary>
    /// Coroutine handle the given wave is completed. 
    /// </summary>
    /// <param name="waveIndex"></param>
    /// <returns></returns>
    private IEnumerator CROnWaveCompleted(int waveIndex)
    {
        if (waveIndex < listWaveItemController.Count - 1)
        {
            listActiveWaveItem[waveIndex].UpdateSlider();
            yield return new WaitForSeconds(0.5f);
            listActiveWaveItem[waveIndex + 1].OnActive(true);
            StartCoroutine(CRShowWaveText(waveIndex + 2));
        }
    }



    /// <summary>
    /// Coroutine show the wave text.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private IEnumerator CRShowWaveText(int number)
    {
        yield return null;
        Vector2 startPos = new Vector2(1000f, wavePanelTrans.anchoredPosition.y);
        Vector2 midPos = new Vector2(0f, wavePanelTrans.anchoredPosition.y);
        Vector2 endPos = new Vector2(-1000f, wavePanelTrans.anchoredPosition.y);

        wavePanelTrans.anchoredPosition = startPos;
        waveText.text = "WAVE: " + number.ToString();

        float t = 0;
        float moveTime = 0.5f;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            wavePanelTrans.anchoredPosition = Vector2.Lerp(startPos, midPos, factor);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        t = 0;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float factor = t / moveTime;
            wavePanelTrans.anchoredPosition = Vector2.Lerp(midPos, endPos, factor);
            yield return null;
        }
    }



    /// <summary>
    /// Coroutine create tank items.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRCreateTankItems()
    {
        for (int i = 0; i < IngameManager.Instance.ListTankItemConfig.Count; i++)
        {
            TankItemController tankItem = GetTankItemController();
            tankItem.transform.SetParent(contentTrans);
            tankItem.transform.localScale = Vector3.one;
            tankItem.OnInit(IngameManager.Instance.ListTankItemConfig[i]);
            listActiveTankItem.Add(tankItem);
            yield return null;
        }
        closeButton.interactable = true;
    }



    /// <summary>
    /// ////////////////////////////////////////////// Public Functions
    /// </summary>


    public override void OnShow()
    {
        //Show the level
        levelText.text = "LEVEL: " + PlayerPrefs.GetInt(PlayerPrefsKey.LEVEL_KEY, 1).ToString();

        //Hide the wave text
        wavePanelTrans.anchoredPosition = new Vector2(1000f, wavePanelTrans.anchoredPosition.y);

        ///Hide tanks panel
        tanksPanelTrans.gameObject.SetActive(false);
    }

    public override void OnHide() 
    {
        //Disable all wave items
        foreach(WaveItemController waveItem in listActiveWaveItem)
        {
            waveItem.gameObject.SetActive(false);
        }
        listActiveWaveItem.Clear();

        gameObject.SetActive(false);
    }


    /// <summary>
    /// Create the wave items with given wave amount.
    /// </summary>
    /// <param name="enemyWaveAmount"></param>
    /// <param name="bossWaveAmount"></param>
    public void CreateWaveItems(int enemyWaveAmount, int bossWaveAmount)
    {
        //Create the wave items for enemy waves
        for (int i = 0; i < enemyWaveAmount; i++)
        {
            WaveItemController waveItem = GetWaveItemController();
            waveItem.transform.SetParent(wavesPanelTrans);
            waveItem.transform.localScale = Vector3.one;
            listActiveWaveItem.Add(waveItem);
            waveItem.OnActive(i == 0);
            waveItem.SetupSprite(0);
        }

        //Create the wave items for boss waves
        for (int i = 0; i < bossWaveAmount; i++)
        {
            WaveItemController waveItem = GetWaveItemController();
            waveItem.transform.SetParent(wavesPanelTrans);
            waveItem.transform.localScale = Vector3.one;
            listActiveWaveItem.Add(waveItem);
            waveItem.OnActive(false);
            waveItem.SetupSprite(1);
        }
    }


    /// <summary>
    /// Show the wave text of the first wave.
    /// </summary>
    public void ShowTextForFirstWave()
    {
        StartCoroutine(CRShowWaveText(1));
    }


    /// <summary>
    /// Handle UI when the given wave index is completed.
    /// </summary>
    /// <param name="waveIndex"></param>
    public void OnWaveCompleted(int waveIndex)
    {
        StartCoroutine(CROnWaveCompleted(waveIndex));
    }



    /// <summary>
    /// Get the world position for the coin to move to.
    /// </summary>
    /// <returns></returns>
    public Vector2 CoinTextWorldPos()
    {
        Vector3 worldPos = Vector2.zero;
        Vector2 screenPoint = coinsText.rectTransform.position;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(coinsText.rectTransform, screenPoint, Camera.main, out worldPos);
        return new Vector2(worldPos.x, worldPos.y);
    }



    /// <summary>
    /// Update the buy tank buttons of all tank items.
    /// </summary>
    public void UpdateBuyTankButtons()
    {
        foreach (TankItemController tankItem in listActiveTankItem)
        {
            tankItem.UpdateBuyButton();
        }
    }


    /// <summary>
    /// ////////////////////////////////////////////// UI Functions
    /// </summary>
    


    public void OnClickHomeButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.Button);
        SoundManager.Instance.StopMusic(true);
        SceneManager.LoadScene("HomeScene");
    }


    public void OnClickStoreButton() 
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.Button);
        IngameManager.Instance.GamePause();
        tanksPanelTrans.gameObject.SetActive(true);
        if (contentTrans.childCount == 0)
        {
            closeButton.interactable = false;
            StartCoroutine(CRCreateTankItems());
        }
        else
        {
            foreach(TankItemController tankItem in listActiveTankItem)
            {
                tankItem.UpdateBuyButton();
            }
        }
    }


    public void OnClickCloseButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.Button);
        tanksPanelTrans.gameObject.SetActive(false);
        IngameManager.Instance.GameResume();
    }
}
