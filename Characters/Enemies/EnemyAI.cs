using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Needed Referencies Enemy AI")]
    public EnemyBehaviour _enemyBehaviour;
    public EnemyAttackSystem _enemyAttackSystem;

    [HideInInspector] public GameObject _player;

    [HideInInspector] public EnemyStateMachine _stateMachine;

    [Header("Movement Variables")]
    public float _speed = 2f;

    [Header("Player Detection")]
    [SerializeField] private Transform _checkPlayerPoint;
    [SerializeField] private float _checkPlayerRange = 1f;
    [SerializeField] private LayerMask _playerLayer;

    protected virtual void Start()
    {
        _stateMachine = new EnemyStateMachine();
    }

    protected virtual void Update()
    {
        if (_stateMachine != null && _stateMachine._state != null && _enemyBehaviour.CanAct() && _player != null)
        {
            _stateMachine._state.StateUpdate();
        }  
    }

    public void InitializeEnemy()
    {
        _player = FightSceneManager.Instance._playerBehaviour.gameObject;

        _player.GetComponent<PlayerBehaviour>()._onPlayerDeath.AddListener(OnPlayerDeath);
    }

    public virtual void AttackPlayer()
    {
        _enemyAttackSystem.LaunchAttack();
    }

    public virtual void GetHit()
    {
        StopAttack();
    }

    public bool CheckIfPlayerIsInRange()
    {
        Collider2D hit_player = Physics2D.OverlapCircle(_checkPlayerPoint.position, _checkPlayerRange, _playerLayer);

        return hit_player;
    }

    public virtual void OnEnemyDeath()
    {
        StopAttack();
    }

    private void OnPlayerDeath()
    {
        StopAttack();
    }

    public virtual void StopAttack()
    {
        if (_stateMachine._state != null)
        {
            _stateMachine._state.GetCancel();
        }

        if (_enemyAttackSystem != null)
        {
            _enemyAttackSystem.CancelAttackPreparation();
        }
    }
}
