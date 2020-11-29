using System.Collections;
using Functions.DestroyObjectSolution.Scripts;
using Scenes.SharedDataEachScenes;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Scenes.PlayScenes.Enemy.Scripts
{
    public class Enemy : MonoBehaviour, IDamagable
    {
        private NavMeshAgent agent;
        private EnemyActionController enemyActionController;
        private MeshDestroy meshDestroy;

        private Coroutine lifeRoutine;
        private EnemyState state;

        [SerializeField] private Transform target;
        [SerializeField] private Stat stat;
        [SerializeField] private float sightAngle = 0.2f;
        [SerializeField] private float sightLength = 2f;
        [SerializeField] private LayerMask layerToCast;
        [SerializeField] private float hitPlayerDistanceOffset = 2;

        private Vector3 thisPosition;
        private Vector3 thisForward;
        private Vector3 directionToTarget;
        private float distanceToTarget;
        private bool notMoveEventsDone = true;
        
        public float Hp => stat.Hp;
        public float MaxHp => stat.MaxHp;

        public bool NotMoveEventsDone
        {
            set => notMoveEventsDone = value;
        }


        #region Unity Life Cycle

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            enemyActionController = GetComponentInChildren<EnemyActionController>();
            meshDestroy = GetComponent<MeshDestroy>();
        }

        private void OnEnable()
        {
            state = EnemyState.Idle;
            stat.AddHp(stat.MaxHp);

            if (target == null) target = FindObjectOfType<targetTest>().transform;
            notMoveEventsDone = true;

            lifeRoutine = StartCoroutine(LifeRoutine());
        }

        private void OnDisable()
        {
            if (lifeRoutine != null) StopCoroutine(lifeRoutine);

            lifeRoutine = null;
        }

        #endregion

        private void UpdateToTargetRelData()
        {
            var thisTransform = transform;
            thisPosition = thisTransform.position;
            thisForward = thisTransform.forward;
            var targetPosition = target.position;
            directionToTarget = (targetPosition - thisPosition).normalized;
            distanceToTarget = (targetPosition - thisPosition).magnitude;
        }

        private bool CheckExistTargetInView()
        {
            var angle = Vector3.Dot(directionToTarget, thisForward);

            return (angle > sightAngle
                    && Physics.Raycast(thisPosition, directionToTarget, out var raycastHit, sightLength, layerToCast)
                    && raycastHit.collider.CompareTag("Player"));
        }
        
        /// <summary>
        /// animator의 Attack 애니메이션과 PowerAttack 애니메이션에서의 TimeLineEvents에 의해 호출됨.
        /// </summary>
        public void HitDistCheckAndRealDamage()
        {
            if (distanceToTarget <= hitPlayerDistanceOffset)
            {
                // target.Damage(stat.AttackPower);
                Debug.Log("Attack!");
                GameStateManager.Instance.AddHealth(-(int)stat.AttackPower);
            }
        }
        
        /// <summary>
        /// animator의 Death 애니메이션의 TimeLineEvents에 의해 호출됨.
        /// </summary>
        public void DestroyObject()
        {
            meshDestroy.DestroyMesh(Vector3.zero, Vector3.zero);
            //gameObject.SetActive(false);
        }

        private void Update()
        {
            // destroy test
            if (Input.GetKeyDown(KeyCode.B))
            {
                state = EnemyState.Dead;
            }
        }

        #region FSM

        private IEnumerator LifeRoutine()
        {
            while (state != EnemyState.Dead)
            {
                UpdateToTargetRelData();
                
                if (state == EnemyState.Idle) Idle();
                else if (state == EnemyState.Finding) Find();
                else if (state == EnemyState.Chasing) Chasing();
                else if (state == EnemyState.Attacking) Attack();
                
                Debug.DrawLine(thisPosition, thisPosition + thisForward * sightLength, Color.red);
                
                yield return null;
            }

            Death();
        }

        private void Death()
        {
            GameStateManager.Instance.gameData.huntingCount += 1;
            
            enemyActionController.Death();
        }

        private void Attack()
        {
            if (CheckExistTargetInView())
            {
                //transform.rotation = Quaternion.LookRotation(directionToTarget);
                agent.SetDestination(target.position);
                agent.velocity = Vector3.zero;

                if (notMoveEventsDone)
                {
                    notMoveEventsDone = false;

                    if (Random.Range(0f, 1f) >= 0.5f)
                        enemyActionController.Attack();
                    else
                        enemyActionController.PowerAttack();
                }
            }
            else
                state = EnemyState.Idle;
        }

        private void Chasing()
        {
            if (notMoveEventsDone)
                agent.isStopped = false;
            
            if (CheckExistTargetInView())
            {
                if (distanceToTarget <= hitPlayerDistanceOffset) state = EnemyState.Attacking;
                else if (distanceToTarget <= sightLength && notMoveEventsDone) agent.SetDestination(target.position);
            }
            else state = EnemyState.Idle;

            enemyActionController.SetBasicLocomotionParam(agent.velocity.magnitude);
        }

        private void Find()
        {
            enemyActionController.SetBasicLocomotionParam(agent.velocity.magnitude);
            
            if (CheckExistTargetInView())
                state = EnemyState.Chasing;
        }

        private void Idle()
        {
            agent.isStopped = true;
            state = EnemyState.Finding;
        }

        #endregion

        #region API

        public void Damage(float damageAmount)
        {
            stat.AddHp(-damageAmount);
            if (stat.Hp <= 0) state = EnemyState.Dead;
        }

        #endregion
    }
}