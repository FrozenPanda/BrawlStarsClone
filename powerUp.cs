using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class powerUp : MonoBehaviour
{
    
    private new Collider collider;
    private new Renderer renderer;

    float x = 2f;
    // Start is called before the first frame update
    private void Awake()
    {
        collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();
        collider.enabled = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (x>0)
        {
            x -= Time.deltaTime;
        }
        else
        {
            collider.enabled = true;
        }
        //power upları döndürmek için sadece
        transform.Rotate(Vector3.up, 70f * Time.deltaTime);  
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "PowerUp" && other.gameObject.tag != "Bullet")
        {
            //aslında ölen kişi kaybolurken bir kere daha çarpıyor poweruplara ölürken toplayıp gidiyordu
            //bu hatayı düzeltmek için false yapıldı aşağıdakiler
            renderer.enabled = false;
            collider.enabled = false;
            //PhotonNetwork.Destroy(gameObject);
        }
        
    }

}
