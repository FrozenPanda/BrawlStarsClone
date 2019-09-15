using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class bulletScript : MonoBehaviour
{
    public static bulletScript bulletScrpt;

    public GameObject explosiveEffect;

    // Start is called before the first frame update
    private void Awake()
    {
        bulletScrpt = this;
    }

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }

    private void OnTriggerEnter(Collider other)
    {
        //burada mermiyi ateşleyen kişiye bazen çarpması durumunda instantiate ederken patlıyordu
        //buralar onu engelliyor
        //mermiler ve ateşleyen küpler aynı isime sahip bu sayede mermi birini öldürünce kimin öldürdüğünüde çekiyoruz
        if (other.transform.name != transform.name)
        {
            if (other.gameObject.name != "Plane")
            {
                Debug.Log(other.transform.name);

                //Instantiate(explosiveEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
    }

    public void bulletMovement(Vector3 originalDirection ,Player name)
    {
        //original direction aldığımız zaman, o anki açılan aim'in yönüne göre gitmesini sağlıyor mermilerin
        gameObject.name = name.NickName;

        Rigidbody rb = GetComponent<Rigidbody>();

        transform.forward = originalDirection;

        rb.velocity = originalDirection * 20.0f;
    }
}
