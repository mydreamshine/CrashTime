using KPU;
using KPU.Manager;
using UnityEngine;

namespace Scenes.SharedDataEachScenes.Prefabs.Scripts
{
    public class OptionsButtonUI : MonoBehaviour
    {
        private Canvas parentCanvas;
        private void Awake()
        {
            EventManager.On("game_pause", OnGamePause);
            EventManager.On("open_option_panel", OpenOptionPanel);

            parentCanvas = FindObjectOfType<Canvas>();
        }

        private void OpenOptionPanel(object obj)
        {
            ObjectPoolManager.Instance.Spawn("option_panel", parent: parentCanvas.transform);
        }

        private void OnGamePause(object obj)
        {
            
        }

        public void OnOpenOptionPanel()
        {
            if (GameManager.Instance.State == State.Paused) return;
            GameManager.Instance.SetState(State.Paused);
            EventManager.Emit("game_pause");
            EventManager.Emit("open_option_panel");
        }
    }
}
