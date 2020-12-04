using KPU;
using KPU.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (SceneManager.GetActiveScene().name.Contains("Stage"))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            var mixLevels = FindObjectOfType<MixLevels>();
            if (mixLevels != null)
                mixLevels.unpaused.TransitionTo(0.1f);

            var optionButton = FindObjectOfType<OptionsButtonUI>();
            optionButton.openToggle = false;
            
            if (SceneManager.GetActiveScene().name.Contains("Stage"))
                SlowMotionManager.Instance.SetSlowSpeed(1.0f);
        }

        public void OnCloseOptionPanel()
        {
            if (GameManager.Instance.State == State.Playing) return;
            EventManager.Emit("game_resume");
            EventManager.Emit("close_option_panel");
            GameManager.Instance.SetState(State.Playing);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (transform.parent.gameObject.activeInHierarchy)
                    OnCloseOptionPanel();
            }
        }
    }
}
