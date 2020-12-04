using System.Collections;
using UnityEngine;
using changhoScript;
public class GunControl : MonoBehaviour
{
    [SerializeField]
    private BulletObjectPool bulletPool;

    [SerializeField]
    private int BulletSpeed;

   

    
    public void Fire()
    {
        Manager.instance.MuzzlePaticleOn();
        var clone = bulletPool.GetObject();
        clone.GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed);

    }


   
}
