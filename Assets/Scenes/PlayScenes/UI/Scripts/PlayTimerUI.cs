using System;
using KPU.Time;
using TMPro;
using UnityEngine;

namespace Scenes.PlayScenes.UI.Scripts
{
    public class PlayTimerUI : MonoBehaviour
    {
        private float timeStack;
        private TMP_Text timerText;

        private void Awake()
        {
            timerText = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            TimeManager.Instance.ResetTime();
            TimeManager.Instance.Active();
        }

        private void Update()
        {
            var time = new TimeSpan(0, 0, 0, 0, (int) (TimeManager.Instance.Time * 1000));
            
            timerText.text = $"{time.Minutes:00}:{time.Seconds:00}:{time.Milliseconds:000}";
        }
    }
}
