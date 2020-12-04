
using System;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private ParticleSystem muzzleEffect_prefab;
    private ParticleSystem muzzleEffect;
    [SerializeField] private GameObject paticlePos;

    private ParticleSystem hitEffect;
    [SerializeField] private ParticleSystem hitEffect_prefab;

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

    
    void Start()
    {
        muzzleEffect =Instantiate(muzzleEffect_prefab);
        muzzleEffect.transform.SetParent(paticlePos.transform);
        hitEffect = Instantiate(hitEffect_prefab);
    }



    public void MuzzlePaticleOn()
    {
        muzzleEffect.transform.position = paticlePos.transform.position;
        muzzleEffect.transform.rotation = paticlePos.transform.rotation;

        muzzleEffect.gameObject.SetActive(true);
    }

    public void HitParticleOn(Transform target)
    {
        print(target);
        hitEffect.transform.position = target.position;
        hitEffect.transform.rotation = target.rotation;
        
        hitEffect.Play();
    }
}
