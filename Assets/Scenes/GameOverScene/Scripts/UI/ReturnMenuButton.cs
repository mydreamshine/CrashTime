using KPU.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.GameOverScene.Scripts.UI
{
    public class ReturnMenuButton : MonoBehaviour
    {
        private void Awake()
        {
            EventManager.On("return_menu_scene", ReturnMenuScene);
        }

        private void ReturnMenuScene(object obj)
        {
            SceneManager.LoadScene("Scenes/StartMenuScene/StartMenuScene");
        }

        public void OnReturnMenu()
        {
            EventManager.Emit("return_menu_scene");
        }
    }
}
