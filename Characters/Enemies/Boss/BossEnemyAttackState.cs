using UnityEngine;

public class BossEnemyAttackState : EnemyState
{
    private BossEnemyAI _enemyAI;

    private float _preparationDuration;

    private bool _isWaitingAfterAttack = false;
    private float _waitAfterAttackTimer = 0f;

    private bool _isLastAttack = false;

    public BossEnemyAttackState(BossEnemyAI enemy_AI)
    {
        _enemyAI = enemy_AI;
    }

    public override void Enter()
    {
        _enemyAI.GetArmor();

        if (_enemyAI._bossPhase == BossPhase.First)
        {
            _preparationDuration = _enemyAI._prepareAttackDurationFirstPhase;
        }
        else if (_enemyAI._bossPhase == BossPhase.Second)
        {
            _preparationDuration = _enemyAI._prepareAttackDurationSecondPhase;
        }
        else if (_enemyAI._bossPhase == BossPhase.Third)
        {
            _preparationDuration = _enemyAI._prepareAttackDurationThirdPhase;
        }

        PrepareAttack();

        _enemyAI._animator.Play("Intro");
    }

    public override void StateUpdate()
    {
        if (_isWaitingAfterAttack)
        {
            _waitAfterAttackTimer += Time.deltaTime;

            if (_isLastAttack)
            {
                if (_waitAfterAttackTimer > _enemyAI._afterLastAttackTime)
                {
                    _enemyAI._stateMachine.SetState(new BossEnemyRecupState(_enemyAI));
                }
            }
            else
            {
                if (_waitAfterAttackTimer > _enemyAI._betweenAttacksTime)
                {
                    _enemyAI._stateMachine.SetState(new BossEnemyAttackState(_enemyAI));
                }
            }
        }
    }

    private void PrepareAttack()
    {
        _enemyAI._nextBossAttacks[0]._onAttackPreparationEnd.AddListener(OnAttackPreparationEnd);
        _enemyAI._nextBossAttacks[0].PrepareAttack(_preparationDuration);
    }

    public void OnAttackPreparationEnd()
    {
        _enemyAI._nextBossAttacks[0]._onAttackPreparationEnd.RemoveListener(OnAttackPreparationEnd);
        AttackPlayer();
    }

    private void AttackPlayer()
    {
        _enemyAI._nextBossAttacks[0]._onAttackEnd.AddListener(OnAttackEnd);
        _enemyAI._nextBossAttacks[0].AttackPlayer();
    }

    public void OnAttackEnd()
    {
        _enemyAI._nextBossAttacks[0]._onAttackEnd.RemoveListener(OnAttackEnd);

        ResetAlphaColor();

        _enemyAI._nextBossAttacks.RemoveAt(0);
        _isWaitingAfterAttack = true;

        if(_enemyAI._nextBossAttacks.Count == 0)
        {
            _isLastAttack = true;
        }
    }

    private void ResetAlphaColor()
    {
        Color color = _enemyAI._nextBossAttacks[0].GetComponent<SpriteRenderer>().color;
        color.a = _enemyAI._baseArmsAlphaColor;
        _enemyAI._nextBossAttacks[0].GetComponent<SpriteRenderer>().color = color;
    }
}
