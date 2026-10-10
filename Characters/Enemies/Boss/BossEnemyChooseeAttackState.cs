using UnityEngine;

public class BossEnemyChooseAttackState : EnemyState
{
    private BossEnemyAI _enemyAI;

    private int _nbShowed = 0;
    private int _nbToShow;

    private float _showDuration = 1f;
    private float _showTimer = 0f;

    private Color _currentArmColor;

    public BossEnemyChooseAttackState(BossEnemyAI enemy_AI)
    {
        _enemyAI = enemy_AI;
    }

    public override void Enter()
    {
        _enemyAI.GetArmor();
        _enemyAI.GetComponent<SpriteRenderer>().color = _enemyAI._bossHideColor;

        ChooseAttacks();

        _enemyAI._animator.Play("Intro");
    }

    public override void StateUpdate()
    {
        _showTimer += Time.deltaTime;

        ShowNextArm();

        if (_showTimer > _showDuration) 
        {
            _nbShowed++;

            if (_nbShowed >= _nbToShow) 
            {
                _enemyAI._stateMachine.SetState(new BossEnemyAttackState(_enemyAI));
            }
            else
            {
                _showTimer = 0;
                _currentArmColor = _enemyAI._nextBossAttacks[_nbShowed].GetComponent<SpriteRenderer>().color;
            }
        }

    }

    private void ChooseAttacks()
    {
        _enemyAI._nextBossAttacks.Clear();

        int nb_attacks_to_choose;

        if (_enemyAI._bossPhase == BossPhase.First)
        {
            nb_attacks_to_choose = 1;
            _nbToShow = 1;
        }
        else if (_enemyAI._bossPhase == BossPhase.Second)
        {
            nb_attacks_to_choose = 2;
            _nbToShow = 2;
        }
        else
        {
            nb_attacks_to_choose = 3;
            _nbToShow = 3;
        }

        for (int attack_index = 0; attack_index < nb_attacks_to_choose; attack_index++)
        {
            int rand;

            do
            {
                rand = Random.Range(0, 3);
            } 
            while (_enemyAI._nextBossAttacks.Contains(_enemyAI._bossAttacks[rand]));

            _enemyAI._nextBossAttacks.Add(_enemyAI._bossAttacks[rand]);
        }

        _currentArmColor = _enemyAI._nextBossAttacks[0].GetComponent<SpriteRenderer>().color;
    }

    private void ShowNextArm()
    {
        if(_currentArmColor.a < 1)
        {
            _currentArmColor.a += 0.01f;

            if(_currentArmColor.a > 1)
            {
                _currentArmColor.a = 1;
            }

            _enemyAI._nextBossAttacks[_nbShowed].GetComponent<SpriteRenderer>().color = _currentArmColor;
        }
    }
}
