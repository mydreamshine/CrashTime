
using UnityEngine;
using changhoScript;
public class Bullet : MonoBehaviour
{
    private float bulletLife;
    private BulletObjectPool bullet;
    private TrailRenderer bullet_trail;
    private Rigidbody bullet_rigid;

    private void OnEnable()
    {
        bullet = gameObject.GetComponentInParent<BulletObjectPool>();
        bullet_trail = gameObject.GetComponent<TrailRenderer>();
        bullet_rigid = gameObject.GetComponent<Rigidbody>();
        bulletLife = 2f;

       
        
    }


    private void Update()
    {
        bulletLife -= Time.deltaTime;
     

        if (bulletLife < 0)
        {
            bulletLife = 2f;
            
            bullet_rigid.velocity = new Vector3(0, 0, 0);
            bullet_trail.Clear();
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
            bullet_rigid.velocity = new Vector3(0, 0, 0);
            bullet_trail.Clear();
            bullet.ReturnObject(gameObject);
        }
    }
}
