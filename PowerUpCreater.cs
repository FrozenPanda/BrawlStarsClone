using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PowerUpCreater : MonoBehaviour
{
    public static PowerUpCreater powerUpCreaterScrpt;
    PhotonView photonView;

    public GameObject powerUp;

    //power up için
    float cooldown;
    //10 taneden sonra geri sayım için
    public float winCoolDown;
    
    public string winner;

    public Text winnerText;
    
    string winnerName;

    int winScore;

    


    // Start is called before the first frame update
    void Start()
    {
        winnerText = GameObject.Find("WinnerText").GetComponent<Text>();

        cooldown = -1f;

        
        
        powerUpCreaterScrpt = this;

        photonView = GetComponent<PhotonView>();

        cooldown = 5f;

        winCoolDown = -2f;

        winnerText.gameObject.SetActive(false);

        winnerName = "";

        winScore = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        

        //her bir player için bu gameobject çıktığı için ikişer ya da üçer çıkmasını engelliyoruz

        if (cooldown>0)
        {
            cooldown -= Time.deltaTime;
        }
        else if (cooldown <= 0 && PhotonNetwork.IsMasterClient)
        {
            float x = Random.Range(-1, 1f);
            float z = Random.Range(-1, 1f);

            photonView.RPC("createStart", RpcTarget.AllViaServer,x,z);

            cooldown = 5f;
        }

        if (winCoolDown > 0)
        {
            winCoolDown -= Time.deltaTime;

            winnerText.text = winnerName+ " is going to win in " + winCoolDown.ToString("F1");
        }
        else if(winCoolDown <0f && winCoolDown > -0.5f)
        {
            winnerText.text = winnerName + " has won !";

            GameManager.gameManagerScript.finished = true;

            GameManager.gameManagerScript.backToLobbyButton.gameObject.SetActive(true);
        }

        //Debug.Log(winnerName + " " + winScore + " " + GameManager.gameManagerScript.finished + " " + winCoolDown);

    }

    public void createInfo(float x,float y, int count ,float a, float b)
    {
        photonView.RPC("create", RpcTarget.AllViaServer, x, y, count ,a ,b);
    }

    [PunRPC]
    public void create(float x,float z , int count ,float a, float b)
    {
            GameObject power = Instantiate( powerUp , new Vector3(x + a, 1.5f, z + b), Quaternion.identity);
    }

    [PunRPC]
    public void createStart(float x,float z)
    {
        
        GameObject power = Instantiate(powerUp, new Vector3(x, 1.5f,z), Quaternion.identity);
    }

    [PunRPC]
    public void coolDownForWinner(string player, int count)
    {
        //ilk 10 tane toplayan kişi için geri sayım başlıyor
        // başka biri daha fazla toplarsa bu sefer onun için geri sayım yapıyor
        //brawl stars da bir takım 12 diğeri 15 topladığı zaman ikiside 10 toplamış oluyor
        //ama 15 için geri sayım yapıyor
        if (count > winScore)
        {
            if (player != winnerName)
            {
                winnerName = player;
                winScore = count;
                winnerText.gameObject.SetActive(true);
                winCoolDown = 10.5f;
                winnerText.text = winnerName + " is going to win in " + winCoolDown.ToString("F1");
            }
        }

        //10 taneyi taşıyan kişi öldüğü zaman geri sayım bitiyor.
        if (count == 0)
        {
            if (winnerName == player)
            {
                winnerText.gameObject.SetActive(false);
                winCoolDown = -1f;
                winScore = 0;
                winnerName = "";
            }
        }
        
        
        
        
        
        

    }
}
