using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float lookSensitivity;

    private Rigidbody rigid;


 


    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        Move();
       
        CharaterRotationX();
       
    }


    private void Move()
    {
        float _moveDirX = Input.GetAxis("Horizontal");
        float _moveDirz = Input.GetAxis("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirz;
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        

        rigid.MovePosition(transform.position + _velocity * Time.deltaTime);
       

       
    }

   

    private void CharaterRotationX()
    {
        float _yRotation = Input.GetAxis("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(_characterRotationY));

    }


}
