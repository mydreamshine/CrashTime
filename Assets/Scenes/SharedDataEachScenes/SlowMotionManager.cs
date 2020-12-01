using KPU;
using UnityEngine;

namespace Scenes.SharedDataEachScenes
{
    public class SlowMotionManager : SingletonBehaviour<SlowMotionManager>
    {
        private float currentSlowSpeed;
        public float CurrentSlowSpeed => currentSlowSpeed;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Init();
        }

        public void Init()
        {
            currentSlowSpeed = Time.timeScale = 1.0f;
        }
        
        public void SetSlowSpeed(float amount)
        {
            Time.timeScale = Mathf.Clamp(amount, 0.0f, 1.0f);
        }
    }
}
