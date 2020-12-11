using UnityEngine;
using UnityEngine.UI;

namespace Scenes.SharedDataEachScenes.Prefabs.Scripts
{
    public class AudioEffectSliderUI : MonoBehaviour
    {
        public MixLevels mixLevels;
        private Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            mixLevels = GameObject.Find("Audio Mixer Control").GetComponent<MixLevels>();
        }

        private void OnEnable()
        {
            slider.value = mixLevels.GetSfxLvl();
        }

        public void OnValueChanged(float value)
        {
            mixLevels.SetSfxLvl(value);
        }
    }
}
