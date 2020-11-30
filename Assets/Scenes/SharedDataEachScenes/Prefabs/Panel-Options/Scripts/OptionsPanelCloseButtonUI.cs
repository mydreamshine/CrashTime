using System;
using KPU;
using KPU.Manager;
using UnityEngine;

namespace Scenes.SharedDataEachScenes.Prefabs.Scripts
{
    public class OptionsPanelCloseButtonUI : MonoBehaviour
    {
        private void Awake()
        {
            EventManager.On("game_resume", OnGameResume);
            EventManager.On("close_option_panel", CloseOptionPanel);
        }

        private void CloseOptionPanel(object obj)
        {
            transform.parent.gameObject.SetActive(false);
        }

        private void OnGameResume(object obj)
        {
            
        }

        public void OnCloseOptionPanel()
        {
            if (GameManager.Instance.State == State.Playing) return;
            GameManager.Instance.SetState(State.Playing);
            EventManager.Emit("game_resume");
            EventManager.Emit("close_option_panel");
        }
    }
}
