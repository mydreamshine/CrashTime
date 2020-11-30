using KPU.Manager;
using UnityEngine;

namespace Scenes.StartMenuScene.Scripts.UI
{
    public class ExitButtonUI : MonoBehaviour
    {
        private void Awake()
        {
            EventManager.On("exit_application", ExitApplication);
        }

        private void ExitApplication(object obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // 어플리케이션 종료
#endif
        }

        public void OnExit()
        {
            EventManager.Emit("exit_application");
        }
    }
}
