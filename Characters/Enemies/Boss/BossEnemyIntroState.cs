using UnityEngine;

public class BossEnemyIntroState : EnemyState
{
    private BossEnemyAI _enemyAI;

    private float _introTimer = 0f;

    private bool _hasScreamed = false;

    public BossEnemyIntroState(BossEnemyAI enemy_AI)
    {
        _enemyAI = enemy_AI;
    }

    public override void Enter()
    {
        _enemyAI.GetArmor();

        _enemyAI._animator.Play("Intro");
    }

    public override void StateUpdate()
    {
        _introTimer += Time.deltaTime;

        if (!_hasScreamed && _introTimer > _enemyAI._startIntroPhaseDuration) 
        {
            //screen shake during the boss scream
            if (FightSceneManager.Instance._cameraScript != null)
            {
                FightSceneManager.Instance._cameraScript.TriggerShake(2f, 0.5f);
            }

            _enemyAI.SpawnScreamVfx((_enemyAI._introPhaseDuration - _enemyAI._startIntroPhaseDuration)/2f);

            _hasScreamed = true;
        }

        if (_introTimer > _enemyAI._introPhaseDuration) 
        {
            _enemyAI._stateMachine.SetState(new BossEnemyChooseAttackState(_enemyAI));
        }
    }
}
