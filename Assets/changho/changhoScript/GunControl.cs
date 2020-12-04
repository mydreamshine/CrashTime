using System.Collections;
using UnityEngine;
using changhoScript;
using KPU;
using KPU.Manager;

public class GunControl : MonoBehaviour
{
    [SerializeField]
    private BulletObjectPool bulletPool;

    [SerializeField]
    private int BulletSpeed;

   

    bool check = true;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)&&check)
        {
            check = false;
            if (GameManager.Instance.State == State.Playing)
            {
                Manager.instance.MuzzlePaticleOn();
                Fire();
            }

            StartCoroutine(Wait());
        }
    }

    private void Fire()
    {
        var clone = bulletPool.GetObject();
        clone.GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed);

    }


    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        check = true;

    }

}
