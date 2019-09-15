using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;



public class PlayerMovement : MonoBehaviourPun
{
    Rigidbody rb;
    private PhotonView photonView;
    
    public GameObject render;
    private new Collider collider;
    private new Renderer renderer;
    private new Renderer rendererCube;

    public GameObject playerJoystick;
    public Joystick playerMovementJoystick;

    public Text PlayerNameText;
    public Text killText;
    public Text powerCountText;

    //powerUpcreate
    public GameObject powerUp;
    float powerUpCoolDown;
    bool power;

    //powerup alınan sayısı
    int powerUpCount;


    // contentin altına kim kimi öldürdüğü için çıkan text i altına koymak için
    //çünkü content in altında sıralanıyor bunlar
    GameObject content;

    float maxSpeed;
    float health = 100f;
    float reSpawnTime;

    public Image healthBar;

    bool isDead;


    [SerializeField]
    private Animator DroneWalkingAnimation;
    [SerializeField]
    private GameObject animationGetKill;

    private void OnEnable()
    {
      //  PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        
    }

    private void OnDisable()
    {
      //  PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    /*
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        object[] data = (object[])photonEvent.CustomData;
        if (eventCode == 0)
        {
            
            if ((int)data[3] == photonView.ViewID)
            {
                renderer.material.color = new Color((float)data[0], (float)data[1], (float)data[2]);
            }
        }
        else if (eventCode == 1)
        {
            renderer.material.color = new Color((float)data[0], (float)data[1], (float)data[2]);
        }
        
        
    } 
    */



    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        rb = GetComponent<Rigidbody>();

        transform.name = photonView.Owner.NickName;

        renderer = render.GetComponent<Renderer>();

        collider = render.GetComponent<Collider>();
        
    }

    void Start()
    {
        render.transform.name = photonView.Owner.NickName;

        isDead = false;

        reSpawnTime = 0f;

        content = GameObject.FindWithTag("Content");

        powerUpCoolDown = 5f;

        power = false;

        powerUpCount = 0;
        // burası joystick in oyuncularda ikişer tane çıkmasını engellemek için
        if (!photonView.IsMine)
        {
            playerJoystick.gameObject.SetActive(false);

            playerMovementJoystick.gameObject.SetActive(false);
        }

        maxSpeed = 10f;
        //ismini görüntülenmesi için
        PlayerNameText.text = photonView.Owner.NickName;
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        //öldükten sonraa respawn timer bitince tekrar oyun alanının içine atıyoruz oyuncumuzu
        if (isDead && reSpawnTime > -0.1f)
        {
            reSpawnTime -= Time.deltaTime;
            if (reSpawnTime <= 0)
            {
                isDead = false;

                respawn();
            }
                       
        }
        
    }

    private void FixedUpdate()
    {

        if (rb.velocity.magnitude > 1f)
        {
            DroneWalkingAnimation.SetBool("isWalking", true);
        }
        else
        {
            DroneWalkingAnimation.SetBool("isWalking", false);
        }

        if (!photonView.IsMine )
        {
            return;
        }

        // maç bitince hareketi kapatıyoruz
        if (GameManager.gameManagerScript.finished == true)
        {
            return;
        }
        

        if (Input.GetKey(KeyCode.W) || playerMovementJoystick.Vertical > 0.2f)
        {
            rb.AddForce(new Vector3(0f, 0f, 50f));
            
        }
        else if (Input.GetKey(KeyCode.S) ||playerMovementJoystick.Vertical < -0.2f)
        {
            rb.AddForce(new Vector3(0f, 0f, -50f));
        }

        if (Input.GetKey(KeyCode.A) || playerMovementJoystick.Horizontal < -0.2f)
        {
            rb.AddForce(new Vector3(-50f, 0f, 0f));
        }
        else if (Input.GetKey(KeyCode.D) || playerMovementJoystick.Horizontal > 0.2f)
        {
            rb.AddForce(new Vector3(50f, 0f, 0f));
        }

        //BURALAR DENEMEK İÇİN OYUN İÇİNDE TEKRAR AKTİF EDİLİP TEST YAPILABİLİNİR..
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PowerUpCreater.powerUpCreaterScrpt.GetComponent<PhotonView>().RPC("coolDownForWinner", RpcTarget.AllViaServer, photonView.name, 11);
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            PowerUpCreater.powerUpCreaterScrpt.GetComponent<PhotonView>().RPC("coolDownForWinner", RpcTarget.AllViaServer, photonView.name, 0);
        }
        */
        if (Input.GetKeyUp(KeyCode.Space))
        {
            
            DroneWalkingAnimation.SetTrigger("Kill");

            newColor();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            newColor2();
        }

    }
    //burası rpc yi kendi kendine çağırabiliyor ama
    //updatein içinden yönettiğimiz input kodu ile rpc çağıralamıyor
    private void OnTriggerEnter(Collider other)
    {
        if (isDead)
        {
            return;
        }
        if (other.gameObject.tag == "Bullet")
        {
            if (other.gameObject.name != transform.name)
            {
                takeDamage(other.gameObject.name , 0f);
            }
        }
        else if (other.gameObject.tag == "PowerUp" && !isDead)
        {
            powerUpCount++;

            showPowerUpCount(powerUpCount);

            Destroy(other.gameObject);
            //burası 3 için ayarlandı ama çoklu oyuncu olunca 10 tane alınca da geri sayım başlatılabilir.
            if (powerUpCount >= 3)
            {
                PowerUpCreater.powerUpCreaterScrpt.GetComponent<PhotonView>().RPC("coolDownForWinner", RpcTarget.AllViaServer, photonView.name, powerUpCount);
            }

            DroneWalkingAnimation.SetTrigger("Kill");
        }
        
    }

    private void dead()
    {
        float x = gameObject.transform.position.x;
        float z = gameObject.transform.position.z;

        isDead = true;

        transform.position = new Vector3(x, -2f, z);

        PowerUpCreater.powerUpCreaterScrpt.GetComponent<PhotonView>().RPC("coolDownForWinner", RpcTarget.AllViaServer, photonView.name, 0);

        for (int i = 0; i < powerUpCount; i++)
        {
            //burası bir oyuncu öldüğü zaman yeniden oyuncunun topladığı powerupları aynı yerde spawnlamak için
            float a = Random.Range(-2f, 2f);
            float b = Random.Range(-2f, 2f);
            PowerUpCreater.powerUpCreaterScrpt.createInfo(x, z, powerUpCount,a,b);
        }
        
        reSpawnTime = 3f;
        powerUpCount = 0;
        
    }

    private void respawn()
    {
        rb.velocity = Vector3.zero;
        //tekrar aynı cana döndürmek için
        photonView.RPC("reloadHealth", RpcTarget.AllViaServer,1);
        //powerup toplama sayısını tekrardan 0 a eşitliyoruz ve diğer oyunculara bildiriyoruz
        photonView.RPC("showPowerUpCount", RpcTarget.AllViaServer, 0);
        //burası da tekrar oyun içinde rastgele bir yere spawn etmek için
        gameObject.transform.position = new Vector3(Random.Range(-5, 5), 5f, Random.Range(-5, 5));
        
    }

    [PunRPC]
    public void takeDamage(string killer , float healthPlus)
    {
        if (isDead)
        {
            return;
        }
        // burada tekrar 100 canla spawn olması için 2. variable health ekledim spawn komutu çalışınca canınıa 100 gönderiyoruz
        health += healthPlus;
        health -= 30f;
        healthBar.fillAmount = health / 100f;
        if (health <= 0 && photonView.IsMine)
        {
            
            health = 0f;
            photonView.RPC("showKillText", RpcTarget.AllViaServer, killer, transform.name);
            dead();
        }
    }

    [PunRPC]
    public void reloadHealth(int a)
    {
        health = 100f;
        healthBar.fillAmount = health / 100f;
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.position = new Vector3(Random.Range(-10, 10), 4f, Random.Range(-10f, 10f));
    }

    



    [PunRPC]
    public void showKillText(string killer,string dead)
    {
        Text killText2 = Instantiate(killText, gameObject.transform.position, Quaternion.identity);
        killText2.text = killer + "==>>" + dead;
        killText2.transform.parent = content.gameObject.transform;

    }

    [PunRPC]
    public void createPowerUps(int a)
    {
        //GameObject powerUp2 = Instantiate(powerUp, new Vector3(0f,1f,0f), Quaternion.identity);
    }

    //öldükten sonra çıkan power uplar için
    /*[PunRPC]
    public void createPowerUpsAfterDead(int a)
    {
        float x = gameObject.transform.position.x;
        float z = gameObject.transform.position.z;
        
        for (int i = 0; i < a; i++)
        {
            GameObject powerUp3 = Instantiate(powerUp, new Vector3( x + Random.Range(-2,2), 1f, z + Random.Range(-2, 2)), Quaternion.identity);
            powerUp3.tag = "PowerUp2";
        }
    }
    */

    //burası karşı taraftan birinin powerUp aldığı zaman üstte çıkan sayının diğerlerini de görebilmesi için
    [PunRPC]
    public void showPowerUpCount(int a)
    {
        powerUpCount = a;
        if (powerUpCount == 0)
        {
            powerCountText.text = "";
        }
        else
        {
            powerCountText.text = powerUpCount.ToString();
        }
    }

    
    private void newColor()
    {
        float a, b, c;
        a = Random.Range(0f, 1f);
        b = Random.Range(0f, 1f);
        c = Random.Range(0f, 1f);

        object[] datas = new object[] { a, b, c , photonView.ViewID};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };
        
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(0, datas, raiseEventOptions, sendOptions);
        
        
        

    }

    private void newColor2()
    {
        float a, b, c;
        a = Random.Range(0f, 1f);
        b = Random.Range(0f, 1f);
        c = Random.Range(0f, 1f);

        object[] datas = new object[] { a, b, c, photonView.ViewID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(1, datas, raiseEventOptions, sendOptions);




    }






}
