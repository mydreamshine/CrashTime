using Scenes.SharedDataEachScenes;
using UnityEngine;

namespace Scenes.PlayScenes.SlowMotionFunc
{
    public class SlowMotionController : MonoBehaviour
    {
        private MixLevels mixLevels;
        
        [SerializeField] [Range(0.0f, 1.0f)] private float minSlowMotionFactor = 0.1f;
        [SerializeField] [Range(0.0f, 1.0f)] private float maxSlowMotionFactor = 1.0f;
        [SerializeField] [Range(0.1f, 3.0f)] private float gradualTime = 1.0f;

        private float slowScale;
        private float targetSlowScale;

        private void Awake()
        {
            mixLevels = FindObjectOfType<MixLevels>();
            targetSlowScale = slowScale = maxSlowMotionFactor;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                targetSlowScale = minSlowMotionFactor;
                //SlowMotionManager.Instance.SetSlowSpeed(minSlowMotionFactor);
                //if (mixLevels != null) mixLevels.SetTimeScale(Mathf.Clamp(minSlowMotionFactor, 0.6f, 1.0f));
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                targetSlowScale = maxSlowMotionFactor;
                //SlowMotionManager.Instance.SetSlowSpeed(maxSlowMotionFactor);
                //if (mixLevels != null) mixLevels.SetTimeScale(maxSlowMotionFactor);
            }

            slowScale += (targetSlowScale - slowScale) * (Time.unscaledDeltaTime * gradualTime);
            slowScale = Mathf.Clamp(slowScale, 0f, 1f);
            SlowMotionManager.Instance.SetSlowSpeed(slowScale);
            if (mixLevels != null) mixLevels.SetTimeScale(Mathf.Clamp(slowScale, 0.6f, 1.0f));
            
        }
    }
}
