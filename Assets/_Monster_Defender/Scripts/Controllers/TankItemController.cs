using ClawbearGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankItemController : MonoBehaviour
{
    [SerializeField] private Text priceText = null;
    [SerializeField] private Image tankImage = null;
    [SerializeField] private Button buyButton = null;

    private TankItemConfig tankItemConfig = null;


    /// <summary>
    /// Init this tank item with config.
    /// </summary>
    /// <param name="config"></param>
    public void OnInit(TankItemConfig config)
    {
        tankItemConfig = config;
        priceText.text = config.priceToPurchase.ToString();
        tankImage.sprite = config.tankSprite;
        buyButton.interactable = CoinManager.Instance.Coins >= config.priceToPurchase;
    }


    /// <summary>
    /// Update the buy button.
    /// </summary>
    public void UpdateBuyButton()
    {
        buyButton.interactable = CoinManager.Instance.Coins >= tankItemConfig.priceToPurchase;
    }



    public void OnClickBuyButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.Button);
        if (!IngameManager.Instance.IsFullOfTanks())
        {
            CoinManager.Instance.UpdateCoins(-tankItemConfig.priceToPurchase);
            IngameManager.Instance.SpawnTank(tankItemConfig.tankType);
            ViewManager.Instance.IngameView.UpdateBuyTankButtons();
        }
    }
}
