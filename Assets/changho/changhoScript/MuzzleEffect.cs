using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleEffect : MonoBehaviour
{
    private float MuzzleEffectLife;

    private void Update()
    {
        MuzzleEffectLife -= Time.deltaTime;

        if(MuzzleEffectLife < 0)
        {
            MuzzleEffectLife = 0.3f;
            this.gameObject.SetActive(false);

        }


    }

    private void OnEnable()
    {
        MuzzleEffectLife = 0.3f;
        
    }



}
