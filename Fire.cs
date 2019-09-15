using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public static Fire fireScript;

    public GameObject bullet;


    // Start is called before the first frame update
    void Start()
    {
        fireScript = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createBullet(Vector3 direction)
    {
        transform.forward = direction;
        Instantiate(bullet , gameObject.transform.position , Quaternion.identity);
    }
}
