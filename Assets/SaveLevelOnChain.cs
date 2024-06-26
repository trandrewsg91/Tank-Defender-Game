using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thirdweb;
using UnityEngine.UI;

public class SaveLevelOnChain : MonoBehaviour
{
    public Button nextButton;
    public Text nextButtonText;
    public Button homeButton;

    private EndgameView endgameView;

    public async void SaveLevelForPlayer()
    {
        nextButton.interactable = false;
        homeButton.interactable = false;
        nextButtonText.text = "Saving...";
        string address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        var contract = ThirdwebManager.Instance.SDK.GetContract(
           "0xa36e09AD391E0e4d6fE6d442C82a6655FDfc21D1",
           "[{\"type\":\"function\",\"name\":\"getLevel\",\"inputs\":[{\"type\":\"address\",\"name\":\"_player\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"levels\",\"inputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"updateLevel\",\"inputs\":[{\"type\":\"uint256\",\"name\":\"_newLevel\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"}]"
       );

        int CurrentLevel = PlayerPrefs.GetInt(PlayerPrefsKey.LEVEL_KEY, 1);
        await contract.Write("updateLevel", CurrentLevel);
        Debug.Log("Level Saved");
        nextButtonText.text = "NEXT LEVEL";
        nextButton.interactable = true;
        homeButton.interactable = true;

        // Find the GameObject with the tag "EndGameView"
        GameObject endgameViewObject = GameObject.FindWithTag("EndGameView");
        if (endgameViewObject != null)
        {
            // Get the EndgameView component from the GameObject
            endgameView = endgameViewObject.GetComponent<EndgameView>();
            if (endgameView != null)
            {
                // Call the OnClickNextLevelButton method
                endgameView.OnClickNextLevelButton();
            }
            else
            {
                Debug.LogError("EndgameView component not found!");
            }
        }
        else
        {
            Debug.LogError("EndgameView GameObject with tag 'EndGameView' not found!");
        }
    }
}
