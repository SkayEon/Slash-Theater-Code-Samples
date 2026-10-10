using UnityEngine;

public class BossEnemyChangePhaseState : EnemyState
{
    private BossEnemyAI _enemyAI;

    private float _changePhaseTimer = 0f;

    public BossEnemyChangePhaseState(BossEnemyAI enemy_AI)
    {
        _enemyAI = enemy_AI;
    }

    public override void Enter()
    {
        _enemyAI.GetArmor();

        if (_enemyAI._bossPhase == BossPhase.First)
        {
            _enemyAI._bossPhase = BossPhase.Second;
        }
        else if (_enemyAI._bossPhase == BossPhase.Second)
        {
            _enemyAI._bossPhase = BossPhase.Third;
        }

        if (FightSceneManager.Instance._cameraScript != null)
        {
            FightSceneManager.Instance._cameraScript.TriggerShake(1f, 0.5f);
        }

        _enemyAI.SpawnScreamVfx(_enemyAI._changePhaseDuration/2f);

        //knockback player
        Vector2 knockback_direction = Vector2.zero;
        if (FightSceneManager.Instance._playerBehaviour.transform.position.x > _enemyAI.transform.position.x)
        {
            knockback_direction.x = 1;
        }
        else
        {
            knockback_direction.x = -1;
        }
        knockback_direction.y = 0.5f;

        knockback_direction = knockback_direction.normalized;

        FightSceneManager.Instance._playerBehaviour.GetKnockback(knockback_direction, 40f);

        _enemyAI._animator.Play("Angry");
    }

    public override void StateUpdate()
    {
        _changePhaseTimer += Time.deltaTime;

        if(_changePhaseTimer >= _enemyAI._changePhaseDuration)
        {
            _enemyAI._stateMachine.SetState(new BossEnemyChooseAttackState(_enemyAI));
        }
    }
}