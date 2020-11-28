
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

    
    void Start()
    {
        muzzleEffect =Instantiate(muzzleEffect_prefab, paticlePos.transform.position, paticlePos.transform.rotation);
    }



    public void MuzzlePaticleOn()
    {
        muzzleEffect.transform.position = paticlePos.transform.position;
        muzzleEffect.transform.rotation = paticlePos.transform.rotation;

        muzzleEffect.gameObject.SetActive(true);
    }

}
