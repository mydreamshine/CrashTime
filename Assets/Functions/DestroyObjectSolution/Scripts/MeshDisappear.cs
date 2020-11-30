using System;
using UnityEngine;

namespace Functions.DestroyObjectSolution.Scripts
{
    public class MeshDisappear : MonoBehaviour
    {
        [Range(1.0f, 5.0f)] public float lifeTime = 3.0f;
        
        [HideInInspector] public MeshRenderer renderer;
        private float timeStack;

        private void OnEnable()
        {
            timeStack = 0.0f;
        }

        private void Update()
        {
            if (Mathf.Abs(timeStack - lifeTime) < Mathf.Epsilon)
            {
                gameObject.SetActive(false);
                return;
            }

            timeStack = Mathf.Clamp(timeStack + Time.deltaTime, 0.0f, lifeTime);
            
            if (renderer == null) return;
            
            var lerpAlpha = 1.0f - (timeStack / lifeTime);

            foreach (var mat in renderer.materials)
            {
                var alphaColor = mat.color;
                alphaColor.a = lerpAlpha;
                mat.color = alphaColor;
            }
        }
    }
}
