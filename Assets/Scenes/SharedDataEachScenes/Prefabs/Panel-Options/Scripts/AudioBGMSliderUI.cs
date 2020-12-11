using UnityEngine;
using UnityEngine.UI;

namespace Scenes.SharedDataEachScenes.Prefabs.Scripts
{
    public class AudioBGMSliderUI : MonoBehaviour
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
            slider.value = mixLevels.GetBgmLvl();
        }

        public void OnValueChanged(float value)
        {
            mixLevels.SetBgmLvl(value);
        }
    }
}
