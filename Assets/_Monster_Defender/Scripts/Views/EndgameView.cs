using ClawbearGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndgameView : BaseView
{
    [SerializeField] private Text levelFailedText = null;
    [SerializeField] private Text levelCompletedText = null;
    [SerializeField] private Text deadEnemyText = null;
    [SerializeField] private Text deadBossText = null;
    [SerializeField] private Text collectedCoinsText = null;
    [SerializeField] private RectTransform nextButtonTrans = null;
    [SerializeField] private RectTransform restartButtonTrans = null;


    /// <summary>
    /// ////////////////////////////////////////////// Private Functions
    /// </summary>






    /// <summary>
    /// ////////////////////////////////////////////// Public Functions
    /// </summary>

    public override void OnShow()
    {
        if (IngameManager.Instance.GameState == GameState.LevelFailed)
        {
            //Handle UI for level failed.
            levelFailedText.gameObject.SetActive(true);
            levelCompletedText.gameObject.SetActive(false);
            nextButtonTrans.gameObject.SetActive(false);
            restartButtonTrans.gameObject.SetActive(true);
        }
        else 
        {
            //Handle UI for level completed.
            levelFailedText.gameObject.SetActive(false);
            levelCompletedText.gameObject.SetActive(true);
            nextButtonTrans.gameObject.SetActive(true);
            restartButtonTrans.gameObject.SetActive(false);
        }
    }

    public override void OnHide() 
    {
        gameObject.SetActive(false);
    }



    /// <summary>
    /// Update the stats.
    /// </summary>
    /// <param name="deadECount"></param>
    /// <param name="totalECount"></param>
    /// <param name="deadBCount"></param>
    /// <param name="totalBCount"></param>
    /// <param name="coins"></param>
    public void UpdateStats(int deadECount, int totalECount, int deadBCount, int totalBCount, int coins)
    {
        deadEnemyText.text = deadECount.ToString() + "/" + totalECount.ToString();
        deadBossText.text = deadBCount.ToString() + "/" + totalBCount.ToString();
        collectedCoinsText.text = "+" + coins.ToString();
    }



    /// <summary>
    /// ////////////////////////////////////////////// UI Functions
    /// </summary>

    public void OnClickNextLevelButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.Button);
        SceneManager.LoadScene("GameScene");
    }


    public void OnClickHomeButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.Button);
        SceneManager.LoadScene("HomeScene");
    }
}
