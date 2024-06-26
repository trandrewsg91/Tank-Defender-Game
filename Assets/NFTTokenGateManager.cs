using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thirdweb;
using UnityEngine.UI;

public class NFTTokenGateManager : MonoBehaviour
{
    public Button nftButton;
    public Button startButton;
    public Button goldClaimButton;
    public Button luckyWheelButton;
    public Button buyTokenButton;
    public GameObject settingPanel;

    public Text goldClaimButtonText;
    public Text buyTokenButtonText;
    public Text luckyWheelButtonText;
    public Text tokenBalanceText;

    public Text currentGoldText;
    public string Address { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        nftButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        goldClaimButton.gameObject.SetActive(false);
        luckyWheelButton.gameObject.SetActive(false);
        buyTokenButton.gameObject.SetActive(false);
        currentGoldText.gameObject.SetActive(false);
        tokenBalanceText.gameObject.SetActive(false);

        nftButton.interactable = true;
        startButton.interactable = true;
        goldClaimButton.interactable = true;
        luckyWheelButton.interactable = true;
        buyTokenButton.interactable = true;
        settingPanel.SetActive(false);
    }

    public async void Login()
    {
        GetTokenBalance();
        LoadLevelForPlayer();
        nftButton.interactable = true;
        Address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        Debug.Log(Address);
        Contract contract = ThirdwebManager.Instance.SDK.GetContract("0xCF77B5b84F7821f4cFBD793CD53BCe66147b0254");
        List<NFT> nftList = await contract.ERC721.GetOwned(Address);
        if (nftList.Count == 0)
        {
            nftButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
            goldClaimButton.gameObject.SetActive(false);
            luckyWheelButton.gameObject.SetActive(false);
            buyTokenButton.gameObject.SetActive(false);
            settingPanel.SetActive(false);
        }
        else
        {
            nftButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
            goldClaimButton.gameObject.SetActive(true);
            luckyWheelButton.gameObject.SetActive(true);
            buyTokenButton.gameObject.SetActive(true);
            settingPanel.SetActive(true);
            currentGoldText.gameObject.SetActive(true);
            currentGoldText.text = "Gold: " + CoinManager.Instance.currentCoins.ToString();
            tokenBalanceText.gameObject.SetActive(true);
        }
    }

    public async void ClaimNFTPass()
    {
        nftButton.interactable = false;
        var contract = ThirdwebManager.Instance.SDK.GetContract("0xCF77B5b84F7821f4cFBD793CD53BCe66147b0254");
        var result = await contract.ERC721.ClaimTo(Address, 1);
        nftButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
        goldClaimButton.gameObject.SetActive(true);
        luckyWheelButton.gameObject.SetActive(true);
        buyTokenButton.gameObject.SetActive(true);
        settingPanel.SetActive(true);
        currentGoldText.gameObject.SetActive(true);
        currentGoldText.text = "Gold: " + CoinManager.Instance.currentCoins.ToString();
        tokenBalanceText.gameObject.SetActive(true);
    }

    public async void ClaimGold()
    {
        goldClaimButtonText.text = "Claiming!";
        nftButton.interactable = false;
        startButton.interactable = false;
        goldClaimButton.interactable = false;
        luckyWheelButton.interactable = false;
        buyTokenButton.interactable = false;
        var contract = ThirdwebManager.Instance.SDK.GetContract("0xd14D5153949358401A3CA83358043246cb6EE5E3");
        var result = await contract.ERC20.Claim("1");
        //Add gold here
        CoinManager.Instance.currentCoins += 40;
        Debug.Log("Gold claimed");
        goldClaimButtonText.text = "+40 Gold";
        nftButton.interactable = true;
        startButton.interactable = true;
        goldClaimButton.interactable = true;
        buyTokenButton.interactable = true;
        currentGoldText.text = "Gold: " + CoinManager.Instance.currentCoins.ToString();

        //LuckyWheelButton
        luckyWheelButton.interactable = true;
    }

    public async void GetTokenBalance()
    {
        Address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        var contract = ThirdwebManager.Instance.SDK.GetContract("0x8f6d29995C1Eac5993172EfEe3C41D0353D31ceb");
        var balance = await contract.ERC20.BalanceOf(Address);
        tokenBalanceText.text = balance.displayValue;
    }

    public async void ClaimToken()
    {
        buyTokenButtonText.text = "Buying!";
        nftButton.interactable = false;
        startButton.interactable = false;
        goldClaimButton.interactable = false;
        luckyWheelButton.interactable = false;
        buyTokenButton.interactable = false;

        var contract = ThirdwebManager.Instance.SDK.GetContract("0x8f6d29995C1Eac5993172EfEe3C41D0353D31ceb");
        var result = await contract.ERC20.Claim("10");
        buyTokenButtonText.text = "Buy Token";
        GetTokenBalance();
        nftButton.interactable = true;
        startButton.interactable = true;
        goldClaimButton.interactable = true;
        buyTokenButton.interactable = true;

        //LuckyWheelButton
        luckyWheelButton.interactable = true;
    }

    public async void SpinluckyWheel()
    {
        nftButton.interactable = false;
        startButton.interactable = false;
        goldClaimButton.interactable = false;
        luckyWheelButton.interactable = false;
        buyTokenButton.interactable = false;

        Address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        var contract = ThirdwebManager.Instance.SDK.GetContract("0x8f6d29995C1Eac5993172EfEe3C41D0353D31ceb");
        var balance = await contract.ERC20.BalanceOf(Address);
        float balanceValue = float.Parse(balance.displayValue);
        if (balanceValue >= 1)
        {
            var data = await contract.ERC20.Burn("1");
            GetTokenBalance();

            //Countdown
            StartCoroutine(CountdownCoroutine());
        }
        else {
            luckyWheelButtonText.text = "Get More Token";
            StartCoroutine(ChangeTextAfterDelay());
            nftButton.interactable = true;
            startButton.interactable = true;
            goldClaimButton.interactable = true;
            luckyWheelButton.interactable = true;
            buyTokenButton.interactable = true;
        }
    }

    IEnumerator CountdownCoroutine()
    {
        int countdown = 3;
        while (countdown > 0)
        {
            luckyWheelButtonText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
        }
        luckyWheelButtonText.text = "1";
        yield return new WaitForSeconds(1);
        luckyWheelButtonText.text = "";

        int randomNumber = Random.Range(1, 4);
        switch (randomNumber)
        {
            case 1:
                CoinManager.Instance.currentCoins += 40;
                luckyWheelButtonText.text = "+40 Gold";
                currentGoldText.text = "Gold: " + CoinManager.Instance.currentCoins.ToString();
                break;
            case 2:
                CoinManager.Instance.currentCoins += 100;
                luckyWheelButtonText.text = "+100 Gold";
                currentGoldText.text = "Gold: " + CoinManager.Instance.currentCoins.ToString();
                break;
            case 3:
                CoinManager.Instance.currentCoins += 150;
                luckyWheelButtonText.text = "+150 Gold";
                currentGoldText.text = "Gold: " + CoinManager.Instance.currentCoins.ToString();
                break;
            default:
                CoinManager.Instance.currentCoins += 40;
                luckyWheelButtonText.text = "+40 Gold";
                currentGoldText.text = "Gold: " + CoinManager.Instance.currentCoins.ToString();
                break;
        }

        StartCoroutine(WaitAndShowText());
    }

    IEnumerator WaitAndShowText()
    {
        yield return new WaitForSeconds(2);
        luckyWheelButtonText.text = "Lucky Wheel\nCost: 1 Token";

        nftButton.interactable = true;
        startButton.interactable = true;
        goldClaimButton.interactable = true;
        luckyWheelButton.interactable = true;
        buyTokenButton.interactable = true;
    }

    IEnumerator ChangeTextAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        luckyWheelButtonText.text = "Lucky Wheel\nCost: 1 Token";
    }

    public async void LoadLevelForPlayer()
    {
        string address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        var contract = ThirdwebManager.Instance.SDK.GetContract(
        "0xa36e09AD391E0e4d6fE6d442C82a6655FDfc21D1",
        "[{\"type\":\"function\",\"name\":\"getLevel\",\"inputs\":[{\"type\":\"address\",\"name\":\"_player\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"levels\",\"inputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"updateLevel\",\"inputs\":[{\"type\":\"uint256\",\"name\":\"_newLevel\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"}]"
        );
        int blockchainLevel = await contract.Read<int>("getLevel", address);
        int CurrentLevel = PlayerPrefs.GetInt(PlayerPrefsKey.LEVEL_KEY, 1);
        if (blockchainLevel > 1)
        {
            if (blockchainLevel > CurrentLevel)
            {
                PlayerPrefs.SetInt(PlayerPrefsKey.LEVEL_KEY, blockchainLevel);
            }
        }
        Debug.Log(blockchainLevel);
    }
}
