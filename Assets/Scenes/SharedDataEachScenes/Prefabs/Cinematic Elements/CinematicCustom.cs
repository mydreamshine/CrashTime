using UnityEngine;

namespace Scenes.SharedDataEachScenes.Prefabs.Cinematic_Elements
{
    public class CinematicCustom : MonoBehaviour
    {
        [Range(0.0f, 1.0f)] public float timeScale = 1.0f;

        private void OnEnable()
        {
            SlowMotionManager.Instance.SetSlowSpeed(timeScale);
        }
    }
}
