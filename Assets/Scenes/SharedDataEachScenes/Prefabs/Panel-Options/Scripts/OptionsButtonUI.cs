using KPU;
using KPU.Manager;
using UnityEngine;

namespace Scenes.SharedDataEachScenes.Prefabs.Scripts
{
    public class OptionsButtonUI : MonoBehaviour
    {
        private Canvas parentCanvas;
        [SerializeField] private MixLevels mixLevels;
        private void Awake()
        {
            EventManager.On("game_pause", OnGamePause);
            EventManager.On("open_option_panel", OpenOptionPanel);

            parentCanvas = FindObjectOfType<Canvas>();
        }

        public void OpenOptionPanel(object obj)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            ObjectPoolManager.Instance.Spawn("option_panel", parent: parentCanvas.transform);
        }

        private void OnGamePause(object obj)
        {
            mixLevels.paused.TransitionTo(0.01f);
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
