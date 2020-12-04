
using UnityEngine;
using changhoScript;
using KPU.Time;

public class Bullet : MonoBehaviour
{
    private float bulletLifeTimeStack;
    private BulletObjectPool bulletPool;
    private TrailRenderer bullet_trail;
    private Rigidbody bullet_rigid;

    [SerializeField] [Range(1, 3)] public int damagePower = 1; 
    
    [SerializeField] [Range(0.0f, 5.0f)] private float bulletMaxLifeTime = 10.0f; 

    [SerializeField] private Manager particleManager = null;

    private void OnEnable()
    {
        bulletPool = FindObjectOfType<BulletObjectPool>();
        bullet_trail = gameObject.GetComponent<TrailRenderer>();
        bullet_rigid = gameObject.GetComponent<Rigidbody>();
        bulletLifeTimeStack = bulletMaxLifeTime;
        if (null == particleManager)
        {
            particleManager = GameObject.Find("ParticleManager").GetComponent<Manager>();
        }

    }


    private void Update()
    {
        bulletLifeTimeStack -= Time.deltaTime;

        if (bulletLifeTimeStack < 0)
        {
            bulletLifeTimeStack = bulletMaxLifeTime;
            
            bullet_rigid.velocity = new Vector3(0, 0, 0);
            bullet_trail.Clear();
            bulletPool.ReturnObject(gameObject);

        }
    }

    

    


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("gun"))
        {
            return;
        }
        else
        {
            particleManager.HitParticleOn(transform);
            bullet_rigid.velocity = new Vector3(0, 0, 0);
            bullet_trail.Clear();
            bulletPool.ReturnObject(gameObject);
        }
    }
}
