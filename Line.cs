using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Line : MonoBehaviour
{
    public Joystick joystickAim;
    public GameObject player;
    public GameObject bullet;

    private PhotonView photonView;
    private Rigidbody rb;
    //sürekli bırakılması halinde ateş etmemesi için bir yere aim alıp bırakıldıktan sonra ateş etmeye yarıyor..
    bool readyForFİre;
    // Start is called before the first frame update
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        readyForFİre = false;

        gameObject.name = photonView.Owner.NickName;
    }
    
    void Update()
    {
        // maç bitince ateşi kapatıyoruz
        if (GameManager.gameManagerScript.finished == true)
        {
            return;
        }

        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z);

        if (joystickAim.Horizontal == 0f && joystickAim.Vertical == 0f )
        {
            if (readyForFİre)
            {
                photonView.RPC("createBullet", RpcTarget.AllViaServer , rb.rotation );
                readyForFİre = false;
            }
            gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
            
        }
        else
        {
            gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
            readyForFİre = true;

            //this method help us for rotating player's aim
            float verticalDirection;
            float horizontalDirection;

            verticalDirection = joystickAim.Vertical;
            horizontalDirection = joystickAim.Horizontal;

            //bu method sağdaki joystick i oynatınca ona göre aim almamızı sağlıyor fakat kamera açısına göre başına - ve sonun + 90 değerleri girildi
            transform.eulerAngles = new Vector3(0, -Mathf.Atan2(verticalDirection, horizontalDirection) * 180  / Mathf.PI +90f, 0);
        }

    }

    [PunRPC]
    public void createBullet(Quaternion quaternion)
    {
        GameObject bullet2 =
        Instantiate(bullet, gameObject.transform.position , Quaternion.identity);
        //Mermilerin ismini ateş edenle aynı yapıyoruz böylece hem kendine zarar vermiyor hemde ölen kişi onu kimin öldürdüğünü biliyor
        bullet2.transform.name = transform.name;

        bulletScript.bulletScrpt.bulletMovement( (quaternion * Vector3.forward) , photonView.Owner );
    }

}
