using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using changhoScript;
public class MuzzleControl : MonoBehaviour
{
    [SerializeField]
    private BulletObjectPool bulletPool;

    [SerializeField]
    private int BulletSpeed;

    public GameObject bulletPos;

    bool check = true;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)&&check)
        {
            check = false;
            Manager.instance.MuzzlePaticleOn();
            var clone = bulletPool.GetObject();
            clone.transform.Rotate(90, 0, 0);
            clone.GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed);

            StartCoroutine(Wait());
        }
    }



    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        check = true;

    }

}
