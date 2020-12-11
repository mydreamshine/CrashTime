using System;

namespace KPU.Time
{
    public class TimeManager : SingletonBehaviour<TimeManager>
    {
        private bool _timerActive;
        private float _time;
        public float Time => _time;
        public bool TimerActive => _timerActive;

        private void Update()
        {
            // Scene Change Loading에 의한 deltaTime Delay 일시적 보완
            if (1 / UnityEngine.Time.unscaledDeltaTime < 10.0f) return;
            
            if (_timerActive)
                _time += UnityEngine.Time.unscaledDeltaTime;
        }

        public void Active()
        {
            DontDestroyOnLoad(gameObject);
            _timerActive = true;
        }

        public void DeActive()
        {
            _timerActive = false;
        }

        public void ResetTime()
        {
            _timerActive = false;
            _time = 0f;
        }
    }
}