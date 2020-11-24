using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private ParticleSystem muzzleEffect_prefab;
    private ParticleSystem muzzleEffect;
    [SerializeField] private GameObject paticlePos;

    public static Manager instance = null;

    private void Awake()
    {
        if (instance == null)
        {

            instance = this;
        }
        else if(instance != null)
        {
            Destroy(this.gameObject);

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        muzzleEffect =Instantiate(muzzleEffect_prefab, paticlePos.transform.position, paticlePos.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void MuzzlePaticleOn()
    {
        muzzleEffect.transform.position = paticlePos.transform.position;
        muzzleEffect.transform.rotation = paticlePos.transform.rotation;

        muzzleEffect.gameObject.SetActive(true);
    }

}
