using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterBehaviour : MonoBehaviour
{
    [Header("Needed References")]
    [HideInInspector] public Rigidbody2D _rb;
    [HideInInspector] public StatusReciever _statusReciever;
    private Animator _animator;
    public AttackHitboxScript _attackHitBox;

    [Header("Character Stats")]
    [SerializeField] protected int _maxHealthPoint = 10;
    [HideInInspector] public int _healthPoint;
    [HideInInspector] public bool _isDead = false;

    private SpriteRenderer _spriteRenderer;
    private Color _baseColor;
    private Color _hitColor = Color.red;
    private float _currentSpriteResetCoroutineDuration;

    [HideInInspector] public bool _isDashing = false;
    [HideInInspector] public bool _isShielding = false;

    [Header("Ground Check")]
    [SerializeField] private float _groundDistance = 1.23f;
    [SerializeField] private LayerMask _groundLayer;
    [HideInInspector] public bool _isGrounded;
    [HideInInspector] public bool _isFalling = false;
    [HideInInspector] public bool _isLanding = false;
    [HideInInspector] public float _groundedTime = 0f;

    [HideInInspector] public bool _isHitStunned = false;
    [HideInInspector] public bool _isKnockdown = false;

    [HideInInspector] public bool _hasArmor = false;
    [HideInInspector] public bool _hasReflect = false;

    [Header("Opponent Layer")]
    public LayerMask _opponentLayers;

    private bool _isFacingRight = true;

    [HideInInspector] public int _jabDamageBoost = 0;
    [HideInInspector] public bool _isJabIced = false;

    [HideInInspector] public int _kunaiBonusDamage = 0;

    [HideInInspector] public int _bombBonusDamage = 0;
    [HideInInspector] public float _bombBonusExplodeRange = 0;

    [HideInInspector] public int _neutralAttacksBonusDamage = 0;
    [HideInInspector] public int _iceStackBonusDamage = 0;
    [HideInInspector] public int _fireDotBonusDamage = 0;
    [HideInInspector] public bool _isTornadoBoosted = false;

    [HideInInspector] public int _thunderBallBonusDamage = 0;
    [HideInInspector] public int _thunderBallBonusBounce = 0;

    [Header("Feedbacks")]
    [SerializeField] private bool _shouldScreenShakeOnGetHit = true;
    [SerializeField] private float _onGetHitScreenShakeMultiplicator = 1f;

    [HideInInspector] public UnityEvent _onCharacterDeath;

    [HideInInspector] public bool _isFightFinished = false;

    private void Awake()
    {
        // Initialize health before GameManager restores player data
        _healthPoint = _maxHealthPoint;

        GetReferences();
        _baseColor = _spriteRenderer.color;
    }

    public virtual void Start()
    {
        FightSceneManager.Instance._onEndFight.AddListener(OnEndFight);
    }

    public virtual void Update()
    {
        GroundedUpdate();
    }

    private void GetReferences()
    {
        _rb = GetComponent<Rigidbody2D>();
        _statusReciever = GetComponent<StatusReciever>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damage)
    {
        if (CanBeHit())
        {
            PlayAnimation("GetHit");
            ReceiveDamage(damage);
        }
    }

    public virtual void TakeEffectDamage(int damage)
    {
        if(!_isShielding && !_isDead && !FightSceneManager.Instance._isFightEnded)
        {
            ReceiveDamage(damage);
        }
    }

    protected void ReceiveDamage(int damage)
    {
        ChangeSpriteColor(_hitColor, 0.1f);

        DamageDisplayScript damage_display = Instantiate(FightSceneManager.Instance._damageDisplayCanvas, new Vector2(transform.position.x + _statusReciever._prefabSpawnOffsetPosition.x, transform.position.y + _statusReciever._prefabSpawnOffsetPosition.y), Quaternion.identity);
        damage_display.GetDamage(damage);

        if (GetComponent<EnemyBehaviour>() != null)
        {
            FightSceneManager.Instance._currentDamageDealt += damage;
        }

        if (FightSceneManager.Instance._cameraScript != null && _shouldScreenShakeOnGetHit)
        {
            if (damage >= 6)
            {
                FightSceneManager.Instance._cameraScript.TriggerShake(0.1f, damage * _onGetHitScreenShakeMultiplicator);
            }
        }

        if (damage > _healthPoint)
        {
            _healthPoint = 0;
        }
        else
        {
            _healthPoint -= damage;
        }

        if (_healthPoint == 0)
        {
            _isDead = true;
            Die();
        }
    }

    public virtual void HealHp(int hp_to_heal)
    {
        _healthPoint += hp_to_heal;

        if (_healthPoint > _maxHealthPoint) 
        { 
            _healthPoint = _maxHealthPoint;
        }
    }

    public virtual void GetKnockback(Vector2 hit_direction, float knockback_force)
    {
        if (!_isShielding && !_isDead && !_hasArmor)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.totalForce = Vector2.zero;
            _rb.AddForce(hit_direction * knockback_force, ForceMode2D.Impulse);
        }
    }

    public virtual void GetHitStun(float hit_stun_duration)
    {
        if (!_isShielding && !_isDead && !_hasArmor)
        {
            _statusReciever.GetHitStun(hit_stun_duration);
        }
    }

    public virtual void GetKnockdown(float knockdown_duration)
    {
        if (!_isShielding && !_isDead && !_hasArmor)
        {
            _statusReciever.GetKnockdown(knockdown_duration);
        }
    }
    
    public virtual void Die()
    {
        _isDead = true;
        _onCharacterDeath.Invoke();
    }

    public bool CanAct()
    {
        return !_isHitStunned && !_isKnockdown && !_isDashing && !_isDead && !_isFightFinished && !GameManager.Instance._isGamePaused;
    }

    public virtual bool CanBeHit()
    {
        return !_isDead && !_hasArmor && !_isShielding && !_isKnockdown && !FightSceneManager.Instance._isFightEnded;
    }

    private void GroundedUpdate()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _groundDistance, _groundLayer);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * _groundDistance, Color.red);

        if (_isGrounded) 
        {
            _groundedTime += Time.deltaTime;
        }
        else 
        {
            _groundedTime = 0;
        }

        if (!_isGrounded && _rb.linearVelocityY < 0)
        {
            _isFalling = true;
        }
        else
        {
            _isFalling = false;
        }
    }

    // Flip character and attack point
    public void CheckIfCharacterShouldBeFlipped(float horizontal_direction)
    {
        if (horizontal_direction > 0 && !_isFacingRight)
        {
            FlipCharacter();
        }
        else if (horizontal_direction < 0 && _isFacingRight)
        {
            FlipCharacter();
        }
    }

    private void FlipCharacter()
    {
        _isFacingRight = !_isFacingRight;

        // Flip the character sprite
        Vector3 local_scale = transform.localScale;
        local_scale.x *= -1;
        transform.localScale = local_scale;

        // Flip the attack hitbox
        Vector3 attack_point_local_scale = _attackHitBox.gameObject.transform.localScale;
        attack_point_local_scale.x *= -1;
        _attackHitBox.gameObject.transform.localScale = attack_point_local_scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            _isFalling = false;
        }
    }

    public void PlayAnimation(string animation_name)
    {
        _animator.Play(animation_name);
    }

    public void ChangeSpriteColor(Color color)
    {
        _spriteRenderer.color = color;
        StopCoroutine("ResetSpriteColorAfterTime");
    }

    public void ChangeSpriteColor(Color color, float duration)
    {
        _spriteRenderer.color = color;
        _currentSpriteResetCoroutineDuration = duration;

        // Stop the existing parameterless coroutine before restarting it
        StopCoroutine("ResetSpriteColorAfterTime");
        StartCoroutine("ResetSpriteColorAfterTime");
    }

    private IEnumerator ResetSpriteColorAfterTime()
    {
        yield return new WaitForSeconds(_currentSpriteResetCoroutineDuration);
        ResetSpriteColor();
    }

    public void ResetSpriteColor()
    {
        _spriteRenderer.color = _baseColor;
    }

    public void StartCoroutineCacAttack(CharacterBehaviour user, CacAttack cac_attack)
    {
        StartCoroutine(cac_attack.WaitAFrameThenAttack(user));
    }

    public virtual void OnEndFight()
    {
        _isFightFinished = true;
    }

    public bool GetIsFacingRight()
    {
        return _isFacingRight;
    }

    public int GetMaxHealthPoint() 
    { 
        return _maxHealthPoint;
    }
}
