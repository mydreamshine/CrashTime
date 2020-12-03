using Scenes.SharedDataEachScenes;
using UnityEngine;

namespace Scenes.PlayScenes.SlowMotionFunc
{
    public class SlowMotionController : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 1.0f)] private float minSlowMotionFactor = 0.1f;
        [SerializeField] [Range(0.0f, 1.0f)] private float maxSlowMotionFactor = 1.0f;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                SlowMotionManager.Instance.SetSlowSpeed(minSlowMotionFactor);
            else if (Input.GetKeyUp(KeyCode.LeftShift))
                SlowMotionManager.Instance.SetSlowSpeed(maxSlowMotionFactor);
        }
    }
}
