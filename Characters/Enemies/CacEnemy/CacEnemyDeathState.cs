public class CacEnemyDeathState : EnemyState
{
    private EnemyAI _enemyAI;

    public CacEnemyDeathState(EnemyAI enemy_AI)
    {
        _enemyAI = enemy_AI;
    }

    public override void Enter()
    {
        _enemyAI._enemyBehaviour.PlayAnimation("Death");
    }
}
