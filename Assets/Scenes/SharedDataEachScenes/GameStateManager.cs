using KPU;
using KPU.Manager;
using KPU.Time;
using Scenes.PlayScenes.Enemy.Scripts;
using Scenes.PlayScenes.UI.Scripts;
using Scenes.SharedDataEachScenes.Prefabs.Cinematic_Elements;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.SharedDataEachScenes
{
    public class GameStateManager : SingletonBehaviour<GameStateManager>
    {
        public RankData gameData;
        public int healthPoint;
        public int enemyCount;
        private string currSceneName;
        public float sceneChangeDelay = 3f;
        private float sceneChangeDelayTimeStack;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
        }

        private void SceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
        {
            InitSceneProperty();
        }

        private void Start()
        {
            InitSceneProperty();
        }

        private void Update()
        {
            if (!currSceneName.Contains("Stage")) return;

            if (GameManager.Instance.State == State.Paused)
            {
                SlowMotionManager.Instance.SetSlowSpeed(0f);
                TimeManager.Instance.DeActive();
            }
            else
            {
                SlowMotionManager.Instance.SetSlowSpeed(SlowMotionManager.Instance.CurrentSlowSpeed);
                TimeManager.Instance.Active();
            }
            
            var playTime = (int) (TimeManager.Instance.Time * 1000);
            gameData.playMilliSecondTime = playTime;
            
            var enemys = FindObjectsOfType<Enemy>(false);
            enemyCount = (enemys != null) ? enemys.Length : 0;

            if (enemyCount == 0 || healthPoint == 0)
            {
                GameManager.Instance.SetState(State.GameEnded);
                
                TimeManager.Instance.DeActive();
                
                sceneChangeDelayTimeStack += Time.unscaledDeltaTime;
            }

            if (!(sceneChangeDelayTimeStack >= sceneChangeDelay)) return;
            switch (currSceneName)
            {
                case "Stage1":
                    SceneManager.LoadScene(healthPoint == 0
                        ? "Scenes/GameOverScene/GameOverScene"
                        : "Scenes/PlayScenes/Stage2/Stage2");
                    break;
                case "Stage2": SceneManager.LoadScene("Scenes/GameOverScene/GameOverScene"); break;
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

        private void InitSceneProperty()
        {
            currSceneName = SceneManager.GetActiveScene().name;
            if (currSceneName.Contains("Stage"))
            {
                if (currSceneName == "Stage1")
                    Init();

                var enemys = FindObjectsOfType<Enemy>();
                enemyCount = (enemys != null) ? enemys.Length : 0;
                
                TimeManager.Instance.Active();
            }
            else
            {
                TimeManager.Instance.DeActive();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            sceneChangeDelayTimeStack = 0.0f;
            
            SlowMotionManager.Instance.Init();
            var cinematicCustom = FindObjectOfType<CinematicCustom>();
            if (cinematicCustom != null) SlowMotionManager.Instance.SetSlowSpeed(cinematicCustom.timeScale);

            GameManager.Instance.SetState(State.Playing);
            
            EventManager.Emit("game_started");
        }

        public void AddHealth(int amount)
        {
            healthPoint = healthPoint > 0 ? healthPoint + amount : 0;
        }
    }
}