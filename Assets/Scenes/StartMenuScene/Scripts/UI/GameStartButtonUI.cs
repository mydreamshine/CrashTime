using KPU.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.StartMenuScene.Scripts.UI
{
    public class GameStartButtonUI : MonoBehaviour
    {
        private void Awake()
        {
            EventManager.On("game_start", OnGameStart);
        }

        private void OnGameStart(object obj)
        {
            SceneManager.LoadScene("Scenes/PlayScenes/Stage1/Stage1");
        }

        public void OnStart()
        {
            EventManager.Emit("game_start");
        }
    }
}
