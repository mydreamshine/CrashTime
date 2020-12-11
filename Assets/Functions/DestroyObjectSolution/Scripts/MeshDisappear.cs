using UnityEngine;

namespace Functions.DestroyObjectSolution.Scripts
{
    public class MeshDisappear : MonoBehaviour
    {
        [Range(1.0f, 5.0f)] public float lifeTime = 3.0f;
        
        // lifeTime에 비례하여 lifeTime동안 rigidBody에 의한 Transforming을
        // 얼마만큼 수용할 지에 대한 ScaleFactor
        // 0: 전혀 움직이지 않음, 1: lifeTime동안 rigidBody가 적용됨 (lifeTime이 지난 후에 사라지지 않음)
        // 0.5: lifeTime동안 rigidBody에 의한 Transforming이 절반만 적용됨
        [Range(0.0f, 1.0f)] public float rigidBodyFullActionScale = 1.0f;
        
        [HideInInspector] public MeshRenderer renderer;
        [HideInInspector] public Rigidbody rigidbody;

        private float maxSpeed = 0.0f;
        private float timeStack;

        private void OnEnable()
        {
            timeStack = 0.0f;
        }

        private void Update()
        {
            if (timeStack >= lifeTime)
            {
                if (Mathf.Abs(rigidBodyFullActionScale - 1.0f) < Mathf.Epsilon)
                    gameObject.SetActive(false);
                return;
            }

            timeStack = Mathf.Clamp(timeStack + Time.deltaTime, 0.0f, lifeTime);
            var lerpAlpha = 1.0f - (timeStack / lifeTime);
            
            // update rigid body
            {
                if (rigidbody == null) return;

                if ((timeStack / lifeTime) >= rigidBodyFullActionScale)
                {
                    if (!rigidbody.isKinematic)
                    {
                        rigidbody.useGravity = false;
                        rigidbody.isKinematic = true;
                    }

                    lerpAlpha = 1.0f - rigidBodyFullActionScale;
                }
            }
            
            // update alpha blending
            {
                if (renderer == null) return;

                foreach (var mat in renderer.materials)
                {
                    var alphaColor = mat.color;
                    alphaColor.a = lerpAlpha;
                    mat.color = alphaColor;
                }
            }
        }
    }
}
