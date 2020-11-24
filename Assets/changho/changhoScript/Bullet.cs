using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using changhoScript;
public class Bullet : MonoBehaviour
{
    private float bulletLife;
    private BulletObjectPool bullet;
  

    private void OnEnable()
    {
        bullet = gameObject.GetComponentInParent<BulletObjectPool>();
        bulletLife = 2f;
        
    }


    private void Update()
    {
        bulletLife -= Time.deltaTime;
     

        if (bulletLife < 0)
        {
            bulletLife = 2f;
            
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            gameObject.GetComponent<TrailRenderer>().Clear();
            bullet.ReturnObject(gameObject);

        }


    }

    

    


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "gun")
        {
            return;
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            gameObject.GetComponent<TrailRenderer>().Clear();
            bullet.ReturnObject(gameObject);
        }
    }
}
