using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager gameManagerScript;
    

    [SerializeField]
    GameObject[] playerPrefab;

    [SerializeField]
    GameObject playerSelectionCanvas;

    public GameObject content;
    public Text killText;

    public GameObject powerCreater;

    // oyunun bitip bitmediğini kontrol etmek için
    public bool finished;

    public Button backToLobbyButton;

    void Start()
    {
        finished = false;

        gameManagerScript = this;
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.InstantiateSceneObject(powerCreater.name, new Vector3(0f, 0f, 0f), Quaternion.identity);
        }
        //oyun bittiği zaman açılan burası geri dönemyi sağlıyor lobbye powerUpCreater in içinden setActive true olarak açıyoruz..
        backToLobbyButton.gameObject.SetActive(false);
        
    }

    public void createPlayer(int x)
    {
        if (PhotonNetwork.IsConnected)
        {

            if (playerPrefab != null)
            {
                int randomPoint = Random.Range(-10, 10);

                PhotonNetwork.Instantiate(playerPrefab[x].name, new Vector3(randomPoint,3f,randomPoint), Quaternion.identity);
                playerSelectionCanvas.gameObject.SetActive(false);
            }
        }
    }


    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();

    }







}
