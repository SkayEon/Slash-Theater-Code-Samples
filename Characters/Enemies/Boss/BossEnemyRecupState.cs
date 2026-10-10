using UnityEngine;

public class BossEnemyRecupState : EnemyState
{
    private BossEnemyAI _enemyAI;

    private float _recupDuration;
    private float _recupTimer = 0f;

    public BossEnemyRecupState(BossEnemyAI enemy_AI)
    {
        _enemyAI = enemy_AI;
    }

    public override void Enter()
    {
        _enemyAI.LoseArmor();

        if (_enemyAI._bossPhase.Equals(BossPhase.First))
        {
            _recupDuration = _enemyAI._recupPhaseDurationFirstPhase;
        }
        else if (_enemyAI._bossPhase.Equals(BossPhase.Second))
        {
            _recupDuration = _enemyAI._recupPhaseDurationSecondPhase;
        }
        else if (_enemyAI._bossPhase.Equals(BossPhase.Third))
        {
            _recupDuration = _enemyAI._recupPhaseDurationThirdPhase;
        }

        _enemyAI.GetComponent<SpriteRenderer>().color = _enemyAI._bossShowColor;

        _enemyAI._enemyBehaviour._onCharacterDeath.AddListener(OnBossDeath);

        _enemyAI._animator.Play("Idle");
    }

    public override void StateUpdate()
    {
        CheckBossHp();

        _recupTimer += Time.deltaTime;

        if (_recupTimer > _recupDuration)
        {
            _enemyAI._stateMachine.SetState(new BossEnemyChooseAttackState(_enemyAI));
        }
    }

    public override void Exit()
    {
        _enemyAI._enemyBehaviour._onCharacterDeath.RemoveListener(OnBossDeath);

        _enemyAI.GetArmor();
        _enemyAI.GetComponent<SpriteRenderer>().color = _enemyAI._bossHideColor;
    }

    private void CheckBossHp()
    {
        if (_enemyAI._bossPhase == BossPhase.First) 
        { 
            if(_enemyAI._enemyBehaviour._healthPoint <= _enemyAI._hpToStartSecondPhase)
            {
                _enemyAI._stateMachine.SetState(new BossEnemyChangePhaseState(_enemyAI));
            }
        }
        else if(_enemyAI._bossPhase == BossPhase.Second)
        {
            if (_enemyAI._enemyBehaviour._healthPoint <= _enemyAI._hpToStartThirdPhase)
            {
                _enemyAI._stateMachine.SetState(new BossEnemyChangePhaseState(_enemyAI));
            }
        }
    }

    private void OnBossDeath()
    {
        _enemyAI._stateMachine.SetState(new BossEnemyDeathState(_enemyAI));
    }
}
