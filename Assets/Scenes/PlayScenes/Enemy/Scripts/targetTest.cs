using KPU;
using KPU.Manager;
using UnityEngine;

namespace Scenes.PlayScenes.Enemy.Scripts
{
    public class targetTest : MonoBehaviour
    {
        private PlayerMoveController moveController;
        private float currMoveSpeed;
        private bool speedIsSaved;

        private void Awake()
        {
            moveController = GetComponent<PlayerMoveController>();
            currMoveSpeed = moveController.characterMoveSpeed;
        }

        private void Update()
        {
            if (GameManager.Instance.State == State.Paused || GameManager.Instance.State == State.GameEnded)
            {
                if (speedIsSaved) return;
                currMoveSpeed = moveController.characterMoveSpeed;
                moveController.characterMoveSpeed = 0f;
                speedIsSaved = true;
            }
            else
            {
                if (!speedIsSaved) return;
                moveController.characterMoveSpeed = currMoveSpeed;
                speedIsSaved = false;
            }
        }
    }
}
