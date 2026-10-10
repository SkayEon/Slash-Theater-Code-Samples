
public class CacEnemyAttackState : EnemyState
{
    private CacEnemyAI _enemyAI;

    public CacEnemyAttackState(CacEnemyAI enemy_AI)
    {
        _enemyAI = enemy_AI;
    }

    public override void Enter()
    {
        _enemyAI.AttackPlayer();
    }
}
