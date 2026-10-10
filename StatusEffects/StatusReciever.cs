using System.Collections;
using UnityEngine;

public class StatusReciever : MonoBehaviour
{
    private CharacterBehaviour _characterBehaviour;

    //Status
    [Header("Status Variables")]
    private float _reflectDamageMultiplicator;
    private GameObject _currentIceReflectObject;

    //Elemental Status - Ice
    [Header("Elemental Status - Ice Variables")]
    public Vector2 _prefabSpawnOffsetPosition = new Vector2(0, 2f);

    private int _singleIceStackDamage = 1;
    private int _maxStackBonusDamage = 2;

    [HideInInspector] public bool _hasIceStack = false;

    private int _maxNbIceStack = 5;
    private int _nbIceStack = 0;

    private float _iceStackDuration = 3f;
    private float _iceStackTimer = 0f;

    private GameObject _iceStackObject;

    private float _antiIceStackDuration = 1f;
    private float _antiIceStackTimer = 0f;
    private bool _canAddIceStack = true;

    private float _slowPercent = 50f;
    private float _slowDuration = 3f;
    private float _originalSpeed;
    private bool _isSlowed = false;
    private float _slowTimer = 0f;


    private int _opponent_passive_ice_stack_bonus_damage = 0;

    //Elemental Status - Fire
    [HideInInspector] public bool _isBurned = false;

    private int _burnDamage = 1;
    private int _bonusFireDotDamage = 0;

    private float _lastHitBurnedTimer;
    private float _burnedTimer;
    private float _timeBetweenBurnHit = 1f;

    private GameObject _burnVfx;

    //hit stun
    protected float _hitStunTimer = 0f;

    //knockdown
    protected float _knockdownTimer = 0f;

    //Sprite Color
    private Color _hitStunColor = Color.yellow;
    private Color _knockdownColor = Color.purple;

    private void Start()
    {
        _characterBehaviour = GetComponent<CharacterBehaviour>();
        _characterBehaviour._onCharacterDeath.AddListener(OnCharacterDeath);
    }

    private void Update()
    {
        HitStunUpdate();
        KnockdownUpdate();

        BurnUpdate();
        IceStackUpdate();
        SlowUpdate();
    }

    //Status
    public virtual void GetHitStun(float hit_stun_duration)
    {
        if (!_characterBehaviour._isShielding && !_characterBehaviour._isDead)
        {
            _characterBehaviour.PlayAnimation("GetHit");

            if (_hitStunTimer < hit_stun_duration)
            {
                _hitStunTimer = hit_stun_duration;
            }

            _characterBehaviour._isHitStunned = true;
            _characterBehaviour.ChangeSpriteColor(_hitStunColor, hit_stun_duration);
        }
    }

    public virtual void GetKnockdown(float knockdown_duration)
    {
        if (!_characterBehaviour._isShielding && !_characterBehaviour._isDead)
        {
            _characterBehaviour.PlayAnimation("GetKnockdown");

            _characterBehaviour._isKnockdown = true;
            _knockdownTimer = knockdown_duration;

            _characterBehaviour.ChangeSpriteColor(_knockdownColor, knockdown_duration);
        }
    }

    public void GetReflect(float damage_multiplicator, float duration)
    {
        _characterBehaviour._hasReflect = true;
        _reflectDamageMultiplicator = damage_multiplicator;

        // Spawn the ice crystal visual effect
        _currentIceReflectObject = Instantiate(FightSceneManager.Instance._iceReflectPrefab, transform);

        StartCoroutine(ReflectDuration(duration));
    }

    private IEnumerator ReflectDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        _characterBehaviour._hasReflect = false;
        Destroy(_currentIceReflectObject);
    }

    public void ReflectProjectile(ProjectileSpawnScript projectile)
    {
        projectile.GetReflected(_reflectDamageMultiplicator, Element.Ice, _characterBehaviour);
    }

    //Elemental Status - Fire
    public void GetBurned(float burn_time, int bonus_fire_dot_damage)
    {
        if (!_characterBehaviour._isShielding && !_characterBehaviour._isDead && !_characterBehaviour._hasArmor)
        {
            _isBurned = true;
            _lastHitBurnedTimer = burn_time;
            _burnedTimer = burn_time;
            _bonusFireDotDamage = bonus_fire_dot_damage;

            // Spawn the burn visual effect
            if (_burnVfx == null)
            {
                _burnVfx = Instantiate(FightSceneManager.Instance._burnVfx, transform);
            }
        }
    }

    //Elemental Status - Ice
    public void AddIceStack(int ice_stack_bonus_damage)
    {
        if (!_characterBehaviour._isShielding && !_characterBehaviour._isDead && _canAddIceStack && !_characterBehaviour._hasArmor)
        {
            _hasIceStack = true;
            _nbIceStack++;

            _iceStackTimer = 0f;

            _opponent_passive_ice_stack_bonus_damage = ice_stack_bonus_damage;

            if (_iceStackObject == null)
            {
                _iceStackObject = Instantiate(FightSceneManager.Instance._iceStackPrefab, transform.position, Quaternion.identity);

                Vector3 offset = new Vector3(_prefabSpawnOffsetPosition.x, _prefabSpawnOffsetPosition.y, 0);
                _iceStackObject.transform.position = _characterBehaviour.transform.position + offset;
            }
            else
            {
                _iceStackObject.GetComponent<Animator>().Play("Idle" + _nbIceStack.ToString());
            }

            if (_nbIceStack == _maxNbIceStack)
            {
                ReleaseIceStackAttack();
            }
        }
    }

    private void ReleaseIceStackAttack()
    {
        StartCoroutine(IceAttack());
    }

    private IEnumerator IceAttack()
    {
        _canAddIceStack = false;

        yield return new WaitForSeconds(0.2f);

        if(_iceStackObject != null)
        {
            _iceStackObject.GetComponent<Animator>().Play("Attack" + _nbIceStack.ToString());
        }
        
        yield return new WaitForSeconds(0.2f);

        if (_iceStackObject != null)
        {
            int damage_to_take = _nbIceStack * (_opponent_passive_ice_stack_bonus_damage + _singleIceStackDamage);

            if (_nbIceStack == _maxNbIceStack)
            {
                damage_to_take += _maxStackBonusDamage;
                GetSlowed();
            }

            _characterBehaviour.TakeEffectDamage(damage_to_take);

            Destroy(_iceStackObject.gameObject);
        }

        _hasIceStack = false;
        _nbIceStack = 0;
    }

    private void GetSlowed()
    {
        if (_isSlowed)
        {
            _slowTimer = _slowDuration;
            return;
        }

        if (GetComponent<PlayerBehaviour>())
        {
            _originalSpeed = GetComponent<PlayerBehaviour>()._playerMovement.GetMoveSpeed();
            GetComponent<PlayerBehaviour>()._playerMovement.SetMoveSpeed(_originalSpeed * (_slowPercent / 100f));
        }
        else if (GetComponent<EnemyAI>())
        {
            _originalSpeed = GetComponent<EnemyAI>()._speed;
            GetComponent<EnemyAI>()._speed *= _slowPercent / 100f;
        }
        else
        {
            return;
        }

        _isSlowed = true;
        _slowTimer = _slowDuration;
    }

    private void SlowUpdate()
    {
        if (!_isSlowed)
        {
            return;
        }

        _slowTimer -= Time.deltaTime;

        if (_slowTimer <= 0f)
        {
            _isSlowed = false;

            if (GetComponent<PlayerBehaviour>())
            {
                GetComponent<PlayerBehaviour>()._playerMovement.SetMoveSpeed(_originalSpeed);
            }
            else if (GetComponent<EnemyAI>())
            {
                GetComponent<EnemyAI>()._speed = _originalSpeed;
            }
        }
    }

    //Elemental Status - Wind
    public void SetIsInTornadoHole()
    {
        _characterBehaviour._rb.gravityScale = 0;
    }

    public void SetIsNotInTornadoHole()
    {
        _characterBehaviour._rb.gravityScale = 1;
    }

    //Status Update
    private void HitStunUpdate()
    {
        if (_characterBehaviour._isHitStunned)
        {
            _hitStunTimer -= Time.deltaTime;

            if (_hitStunTimer <= 0f)
            {
                _characterBehaviour._isHitStunned = false;
                _characterBehaviour.ResetSpriteColor();
            }
        }
    }

    private void KnockdownUpdate()
    {
        if (_characterBehaviour._isKnockdown)
        {
            _knockdownTimer -= Time.deltaTime;

            if (_knockdownTimer <= 0f)
            {
                _characterBehaviour._isKnockdown = false;
                _characterBehaviour.ResetSpriteColor();
            }
        }
    }

    //Elemental Status Update
    private void BurnUpdate()
    {
        if (_isBurned)
        {
            _burnedTimer -= Time.deltaTime;

            if (_burnedTimer <= _lastHitBurnedTimer - _timeBetweenBurnHit)
            {
                _characterBehaviour.TakeEffectDamage(_burnDamage + _bonusFireDotDamage);
                _lastHitBurnedTimer -= _timeBetweenBurnHit;
            }

            if (_burnedTimer <= 0)
            {
                _isBurned = false;
                Destroy(_burnVfx);
                _burnVfx = null;
            }
        }
    }

    private void IceStackUpdate()
    {
        if (_hasIceStack && _canAddIceStack)
        {
            _iceStackTimer += Time.deltaTime;

            if (_iceStackTimer >= _iceStackDuration) 
            {
                ReleaseIceStackAttack();
            }
        }

        if(_iceStackObject != null)
        {
            Vector3 offset = new Vector3(_prefabSpawnOffsetPosition.x, _prefabSpawnOffsetPosition.y, 0);
            _iceStackObject.transform.position = _characterBehaviour.transform.position + offset;
        }

        if (!_canAddIceStack)
        {
            _antiIceStackTimer += Time.deltaTime;

            if(_antiIceStackTimer >= _antiIceStackDuration)
            {
                _canAddIceStack = true;
                _antiIceStackTimer = 0;
            }
        }
    }

    private void OnCharacterDeath()
    {
        Destroy(_burnVfx);
        Destroy(_iceStackObject);
        Destroy(_currentIceReflectObject);
    }
}
