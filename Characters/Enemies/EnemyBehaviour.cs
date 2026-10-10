using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBehaviour : CharacterBehaviour
{
    [Header("Needed References Enemy Behaviour")]
    [SerializeField] private EnemyAI _enemyAI;

    [Header("Boss")]
    public bool _isBoss = false;

    [HideInInspector] public UnityEvent<int ,int> _onEnemyHealthModification;

    public override void Start()
    {
        base.Start();

        _onEnemyHealthModification.AddListener(FightSceneManager.Instance._enemyHealthBarScript.DisplayHealthBar);
    }

    public override void TakeDamage(int damage)
    {
        if (CanBeHit())
        {
            if (!_isBoss)
            {
                PlayAnimation("GetHit");
            }
            ReceiveDamage(damage);
        }

        if (_enemyAI != null && !_isDead)
        {
            _enemyAI.GetHit();
        }

        _onEnemyHealthModification.Invoke(_healthPoint, _maxHealthPoint);
    }

    public override void TakeEffectDamage(int damage)
    {
        if (!_isShielding && !_isDead && !FightSceneManager.Instance._isFightEnded)
        {
            ReceiveDamage(damage);
            _onEnemyHealthModification.Invoke(_healthPoint, _maxHealthPoint);
        }
    }

    public override void GetHitStun(float hit_stun_duration)
    {
        if (!_isBoss)
        {
            base.GetHitStun(hit_stun_duration);
        }
    }

    public override void GetKnockdown(float knockdown_duration)
    {
        if (!_isBoss)
        {
            base.GetKnockdown(knockdown_duration);
        }
    }

    public override void GetKnockback(Vector2 hit_direction, float knockback_force)
    {
        if (!_isBoss)
        {
            base.GetKnockback(hit_direction, knockback_force);
        }
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(EnemyDeath());
    }

    private IEnumerator EnemyDeath()
    {
        FightSceneManager.Instance._onEnemyDeath.Invoke();
        FightSceneManager.Instance.RemoveEnemyFromList(gameObject);

        _enemyAI.OnEnemyDeath();

        yield return new WaitForSeconds(2f);
        if (!_isBoss)
        {
            Destroy(gameObject);
        }
    }
}
