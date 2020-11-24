using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    Transform target;
    public float dist = 10.0f;
    public float height = 5.0f;
    public float smoothRotate = 5.0f;

    private Transform tr;

    private void Start()
    {


        tr = GetComponent<Transform>();
    }


    private void LateUpdate()
    {


        float currentYAngle = Mathf.LerpAngle(tr.eulerAngles.y,
            target.eulerAngles.y, smoothRotate * Time.deltaTime
            );

        Quaternion rot = Quaternion.Euler(0, currentYAngle, 0);

        tr.position = target.position - (rot * Vector3.forward * dist) 
            + (Vector3.up * height);

        tr.LookAt(target);
    }

}
