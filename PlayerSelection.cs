using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    PlayerSelection playerSelectionScript;
    int playerSelectionNumber;
    // Start is called before the first frame update
    void Start()
    {
        playerSelectionScript = this;

        playerSelectionNumber = 1;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void nextPlayer()
    {
        if (playerSelectionNumber < 2)
        {
            playerSelectionNumber++;
            showCurrentPlayer(playerSelectionNumber - 1);
        }
        
    }

    public void previousPlayer()
    {
        if (playerSelectionNumber > 1)
        {
            playerSelectionNumber--;
            showCurrentPlayer(playerSelectionNumber - 1);
        }
        
    }

    public void showCurrentPlayer(int x)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(x).gameObject.SetActive(true);
    }

    public void enterGame()
    {
        GameManager.gameManagerScript.createPlayer(playerSelectionNumber - 1);
    }

}
