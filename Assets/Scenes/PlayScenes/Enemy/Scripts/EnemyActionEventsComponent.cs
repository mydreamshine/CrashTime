using System;
using UnityEngine;

namespace Scenes.PlayScenes.Enemy.Scripts
{
    public class EnemyActionEventsComponent : MonoBehaviour
    {
        private Enemy enemyComponent;

        private void Awake() => enemyComponent = GetComponentInParent<Enemy>();

        /// <summary>
        /// animator의 Attack 애니메이션과 PowerAttack 애니메이션에서의 TimeLineEvents에 의해 호출됨.
        /// </summary>
        public void HitDistCheckAndRealDamage() => enemyComponent.HitDistCheckAndRealDamage();
        
        /// <summary>
        /// animator의 Death 애니메이션의 TimeLineEvents에 의해 호출됨.
        /// </summary>
        public void DestroyObject() => enemyComponent.DestroyObject();
        
        /// <summary>
        /// animator의 Attack 애니메이션과 PowerAttack 애니메이션에서의 TimeLineEvents에 의해 호출됨.
        /// </summary>
        public void EventDone() => enemyComponent.NotMoveEventsDone = true;
    }
}
