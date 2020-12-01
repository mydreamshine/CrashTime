using UnityEngine;

namespace Scenes.SharedDataEachScenes.Prefabs.Scripts
{
    public class AudioEffectSliderUI : MonoBehaviour
    {
        public MixLevels mixLevels;

        void Start()
        {
            mixLevels = GameObject.Find("Audio Mixer Control").GetComponent<MixLevels>();
        }
        public void OnValueChanged(float value)
        {
            mixLevels.masterMixer.SetFloat("sfxVol", value);
        }
    }
}
