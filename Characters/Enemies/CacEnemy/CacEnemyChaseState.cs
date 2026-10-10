using UnityEngine;

public class CacEnemyChaseState : EnemyState
{
    private EnemyAI _enemyAI;
    private EnemyBehaviour _enemyBehaviour;

    //move out of player
    private bool _isMovingOut = false;
    private float _moveOutTimer;

    public CacEnemyChaseState(EnemyAI enemy_AI)
    {
        _enemyAI = enemy_AI;
        _enemyBehaviour = enemy_AI._enemyBehaviour;
    }

    public override void Enter()
    {
        _enemyBehaviour.PlayAnimation("Walk");
        MoveRelativeToPlayer();
    }

    public override void StateUpdate()
    {
        _enemyBehaviour.PlayAnimation("Walk");

        if (_enemyAI.CheckIfPlayerIsInRange())
        {
            ChooseNextAction();
        }
        else
        {
            MoveRelativeToPlayer();
        }
    }

    private void ChooseNextAction()
    {
        if (!_enemyAI._enemyAttackSystem.CanAttack())
        {
            StartMoveOutOfPlayer(0.5f, 2f);
        }
        else
        {
            int rand = Random.Range(0, 2);

            if (rand == 0)
            {
                _enemyAI._stateMachine.SetState(new CacEnemyAttackState(_enemyAI));
            }
            else if (rand == 1)
            {
                StartMoveOutOfPlayer(0.5f, 2f);
            }
        }
    }

    public void MoveRelativeToPlayer()
    {
        _moveOutTimer -= Time.deltaTime;

        if (_moveOutTimer <= 0f)
        {
            _isMovingOut = false;
        }

        if (_enemyBehaviour._isGrounded)
        {
            int player_direction;

            if (_enemyAI._player.transform.position.x > _enemyAI.transform.position.x + 0.1)
            {
                player_direction = 1;

                if (_isMovingOut)
                {
                    player_direction = -player_direction;
                }
            }
            else if (_enemyAI._player.transform.position.x < _enemyAI.transform.position.x - 0.1)
            {
                player_direction = -1;

                if (_isMovingOut)
                {
                    player_direction = -player_direction;
                }
            }
            else
            {
                //if the enemy is horizontally aligned with player, choose a random direction to move out of player
                int direction = Random.Range(0, 2);

                if(direction == 0) 
                {
                    player_direction = 1;
                }
                else
                {
                    player_direction = -1;
                }

                StartMoveOutOfPlayer(0.5f, 1f);
            }

            _enemyBehaviour._rb.linearVelocity = new Vector2(player_direction * _enemyAI._speed, _enemyBehaviour._rb.linearVelocity.y);

            _enemyBehaviour.CheckIfCharacterShouldBeFlipped(player_direction);
        }
    }

    public void StartMoveOutOfPlayer(float min_time, float max_time)
    {
        _moveOutTimer = Random.Range(min_time, max_time);
        _isMovingOut = true;
    }
}
