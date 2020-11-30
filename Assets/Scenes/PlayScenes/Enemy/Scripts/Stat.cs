using System;
using UnityEngine;

namespace Scenes.PlayScenes.Enemy.Scripts
{
    [Serializable]
    public struct Stat
    {
        [SerializeField] private float maxHp;
        [SerializeField] private float attackPower;

        private float hp;

        public Stat(float maxHp = 1f, float attackPower = 1f)
        {
            this.maxHp = maxHp;
            hp = maxHp;
            this.attackPower = attackPower;
        }

        public float MaxHp => maxHp;

        public float Hp => hp;

        public float AttackPower => attackPower;

        public void AddHp(float hpAmount)
        {
            hp = Mathf.Clamp(hp + hpAmount, 0, maxHp);
        }
    }
}