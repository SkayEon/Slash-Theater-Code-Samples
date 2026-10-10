using System.Collections;
using UnityEngine;

public class CacEnemyAI : EnemyAI
{
    protected override void Start()
    {
        base.Start();
        
        _stateMachine.SetState(new CacEnemyChaseState(this));
    }

    public override void AttackPlayer()
    {
        base.AttackPlayer();
        StartCoroutine("AttackProgress");
    }

    public IEnumerator AttackProgress()
    {
        _enemyBehaviour.PlayAnimation("ChargeAttack");

        yield return new WaitForSeconds(_enemyAttackSystem._attack._startingAttackDuration);

        _enemyBehaviour.PlayAnimation("Attack");

        yield return new WaitForSeconds(_enemyAttackSystem._attack._attackDuration);

        _stateMachine.SetState(new CacEnemyChaseState(this));
    }

    public override void GetHit()
    {
        base.GetHit();
        CancelAttack();
    }

    public override void OnEnemyDeath()
    {
        base.OnEnemyDeath();
        StopCoroutine("AttackProgress");
        _stateMachine.SetState(new CacEnemyDeathState(this));
    }

    private void CancelAttack() 
    {
        StopCoroutine("AttackProgress");
        _stateMachine.SetState(new CacEnemyChaseState(this));
    }
}
