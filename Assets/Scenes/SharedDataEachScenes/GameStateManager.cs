using KPU;
using KPU.Manager;
using KPU.Time;
using Scenes.PlayScenes.UI.Scripts;
using UnityEngine.SceneManagement;

namespace Scenes.SharedDataEachScenes
{
    public class GameStateManager : SingletonBehaviour<GameStateManager>
    {
        public RankData gameData;
        public int healthPoint;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
        }

        private void SceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
        {
            var currSceneName = SceneManager.GetActiveScene().name;
            if (currSceneName.Contains("Stage"))
            {
                if (currSceneName == "Stage1")
                    Init();
            }
            else
                TimeManager.Instance.DeActive();
            
            GameManager.Instance.SetState(State.Playing);
            
            EventManager.Emit("game_started");
        }

        private void Start()
        {
            TimeManager.Instance.Active();
        }

        private void Update()
        {
            if (TimeManager.Instance.TimerActive)
            {
                var playTime = (int) (TimeManager.Instance.Time * 1000);
                gameData.playMilliSecondTime = playTime;
                
                if (healthPoint == 0)
                {
                    TimeManager.Instance.DeActive();
                    SceneManager.LoadScene("Scenes/GameOverScene/GameOverScene");
                }
            }
        }

        private void Init()
        {
            gameData = new RankData()
            {
                rank = 0,
                userName = "",
                huntingCount = 0,
                playMilliSecondTime = 0
            };

            var playerHpGroupUI = FindObjectOfType<PlayerHpGroupUI>();
            healthPoint = playerHpGroupUI.MaxHeartCount;
            
            TimeManager.Instance.ResetTime();    
            TimeManager.Instance.DeActive();
        }

        public void AddHealth(int amount)
        {
            healthPoint = healthPoint > 0 ? healthPoint + amount : 0;
        }
    }
}