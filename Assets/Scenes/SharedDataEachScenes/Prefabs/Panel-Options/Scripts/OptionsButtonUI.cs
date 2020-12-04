using KPU;
using KPU.Manager;
using UnityEngine;

namespace Scenes.SharedDataEachScenes.Prefabs.Scripts
{
    public class OptionsButtonUI : MonoBehaviour
    {
        private Canvas parentCanvas;
        [SerializeField] private MixLevels mixLevels;
        private GameObject optionPanel;

        public bool openToggle;
        
        private void Awake()
        {
            EventManager.On("game_pause", OnGamePause);
            EventManager.On("open_option_panel", OpenOptionPanel);

            parentCanvas = FindObjectOfType<Canvas>();
        }

        public void OpenOptionPanel(object obj)
        {
            optionPanel = ObjectPoolManager.Instance.Spawn("option_panel", parent: parentCanvas.transform);
        }

        private void OnGamePause(object obj)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (mixLevels == null)
                mixLevels = FindObjectOfType<MixLevels>();
            if (mixLevels != null)
                mixLevels.paused.TransitionTo(0.01f);
        }

        public void OnOpenOptionPanel()
        {
            if (GameManager.Instance.State == State.Paused) return;
            EventManager.Emit("game_pause");
            EventManager.Emit("open_option_panel");
            GameManager.Instance.SetState(State.Paused);
            openToggle = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !openToggle)
            {
                if (optionPanel == null)
                    OnOpenOptionPanel();
                else if (!optionPanel.activeInHierarchy)
                    OnOpenOptionPanel();
            }
        }
    }
}
