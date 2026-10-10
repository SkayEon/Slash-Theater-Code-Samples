using UnityEngine;

public class BossEnemyDeathState : EnemyState
{
    private BossEnemyAI _enemyAI;

    private Vector2 _enemyPos;

    private float _shakeSpeed = 1f;
    private float _shakeAmount = 1f;

    private float _fallSpeed = 3f;

    //constructeur
    public BossEnemyDeathState(BossEnemyAI enemy_AI)
    {
        _enemyAI = enemy_AI;
    }

    //State methods
    public override void Enter()
    {
        _enemyAI.GetArmor();

        if (FightSceneManager.Instance._cameraScript != null)
        {
            FightSceneManager.Instance._cameraScript.TriggerShake(2f, 0.5f);
        }

        _enemyPos = _enemyAI.transform.position;

        _enemyAI._animator.Play("Death");
    }

    public override void StateUpdate()
    {
        _enemyPos.x = Mathf.Sin(Time.time * _shakeSpeed) * _shakeAmount;
        _enemyPos.y -= Time.deltaTime * _fallSpeed;

        _enemyAI.transform.position = _enemyPos;
    }
}
