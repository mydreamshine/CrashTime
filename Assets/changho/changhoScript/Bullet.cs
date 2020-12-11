
using UnityEngine;
using changhoScript;
using Scenes.SharedDataEachScenes;

public class Bullet : MonoBehaviour
{
    private float bulletLifeTimeStack;
    private BulletObjectPool bulletPool;
    private TrailRenderer bulletTrail;
    private Rigidbody bulletRigid;

    private float maxSpeed = 0.0f;

    [SerializeField] [Range(1, 3)] public int damagePower = 1; 
    
    [SerializeField] [Range(0.0f, 5.0f)] private float bulletMaxLifeTime = 10.0f; 

    [SerializeField] private Manager particleManager = null;

    private void OnEnable()
    {
        bulletPool = FindObjectOfType<BulletObjectPool>();
        bulletTrail = gameObject.GetComponent<TrailRenderer>();
        bulletRigid = gameObject.GetComponent<Rigidbody>();
        bulletLifeTimeStack = bulletMaxLifeTime;
        if (null == particleManager)
        {
            particleManager = GameObject.Find("ParticleManager").GetComponent<Manager>();
        }
    }

    private void Update()
    {
        if (maxSpeed - 0.0f < Mathf.Epsilon) maxSpeed = bulletRigid.velocity.magnitude;
        
        var slowModeScale = SlowMotionManager.Instance.CurrentSlowSpeed;
        var slowModeDeltaTime = slowModeScale * Time.deltaTime;
        bulletLifeTimeStack -= slowModeDeltaTime;
        
        var velocity = bulletRigid.velocity;
        var dir = velocity.normalized;
        bulletRigid.velocity = dir * (maxSpeed * slowModeScale);

        if (bulletLifeTimeStack < 0)
        {
            bulletLifeTimeStack = bulletMaxLifeTime;
            
            bulletRigid.velocity = new Vector3(0, 0, 0);
            bulletTrail.Clear();
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
            bulletRigid.velocity = new Vector3(0, 0, 0);
            bulletTrail.Clear();
            bulletPool.ReturnObject(gameObject);
        }
    }
}
