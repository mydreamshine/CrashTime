using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleControl : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Manager.instance.MuzzlePaticleOn();

        }
    }

}
