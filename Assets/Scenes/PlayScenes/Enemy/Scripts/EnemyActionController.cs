using UnityEngine;

namespace Scenes.PlayScenes.Enemy.Scripts
{
    public class EnemyActionController : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// speed 기반 Locomotion 실행
        /// speed 0: idle, speed >= 0.5: walk_guard, speed >= 1: run,
        /// speed <= 0: back_walk_guard  
        /// </summary>
        /// <param name="speed"></param>
        public void SetBasicLocomotionParam(float speed)
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (animator == null) return;
            animator.SetFloat("Speed", speed);
        }

        public void Attack()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (animator == null) return;
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Attack");
        }

        public void PowerAttack()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (animator == null) return;
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("PowerAttack");
        }

        public void Death()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (animator == null) return;
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Death");
        }

        public void SetPlaySpeed(float speed)
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (animator == null) return;
            animator.speed = speed;
        }
    }
}
