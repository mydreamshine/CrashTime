
using UnityEngine;
using changhoScript;
public class Bullet : MonoBehaviour
{
    private float bulletLife;
    private BulletObjectPool bulletPoolManager = null;
    private TrailRenderer bullet_trail;
    private Rigidbody bullet_rigid;

    private Manager particleManager = null;

    private void OnEnable()
    {
      
        bullet_trail = gameObject.GetComponent<TrailRenderer>();
        bullet_rigid = gameObject.GetComponent<Rigidbody>();
        bulletLife = 2f;
        if (null == particleManager)
        {
            particleManager = GameObject.Find("ParticleManager").GetComponent<Manager>();
        }


        if(null == bulletPoolManager)
        {

            bulletPoolManager = GameObject.Find("BulletObjectPool").GetComponent<BulletObjectPool>();

        }

    }


    private void Update()
    {
        bulletLife -= Time.deltaTime;
     

        if (bulletLife < 0)
        {
            bulletLife = 2f;
            
            bullet_rigid.velocity = new Vector3(0, 0, 0);
            bullet_trail.Clear();
            bulletPoolManager.ReturnObject(gameObject);

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
            particleManager.HitParticleOn(transform);
            bullet_rigid.velocity = new Vector3(0, 0, 0);
            bullet_trail.Clear();
            bulletPoolManager.ReturnObject(gameObject);
        }
    }
}
