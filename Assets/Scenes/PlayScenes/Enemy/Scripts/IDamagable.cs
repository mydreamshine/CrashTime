namespace Scenes.PlayScenes.Enemy.Scripts
{
    public interface IDamagable
    {
        float Hp { get; }
        float MaxHp { get; }
        void Damage(float damageAmount);
    }
}