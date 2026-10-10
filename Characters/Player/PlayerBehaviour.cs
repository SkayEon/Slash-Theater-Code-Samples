using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerBehaviour : CharacterBehaviour
{
    [Header("Needed References Player Behaviour")]
    public PlayerMovement _playerMovement;
    public PlayerAttackSystem _playerAttackSystem;
    public PassiveManager _passiveManager;

    [HideInInspector] public CameraScript _cameraScript;
    [HideInInspector] public PlayerDamageVignette _playerDamageVignette;

    [HideInInspector] public bool _isAttacking;

    [HideInInspector] public UnityEvent _onPlayerDeath;
    private bool _isPlayerDead = false;

    public override void Start()
    {
        base.Start();

        InitializeHealthUI();

        _cameraScript = FindFirstObjectByType<CameraScript>();
        _playerDamageVignette = FindFirstObjectByType<PlayerDamageVignette>();
    }

    public void SendPlayerDataToGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance._upBSpell = _playerAttackSystem.GetUpB();
            GameManager.Instance._downBSpell = _playerAttackSystem.GetDownB();
            GameManager.Instance._sideBSpell = _playerAttackSystem.GetSideB();
            GameManager.Instance._neutralBSpell = _playerAttackSystem.GetNeutralB();

            GameManager.Instance._playerPassives = _passiveManager._passives;

            GameManager.Instance._playerHealth = _healthPoint;
            GameManager.Instance._playerMaxHealth = _maxHealthPoint;
        }
    }

    public void GetPlayerDataFromGameManager()
    {
        if (GameManager.Instance != null)
        {
            _playerAttackSystem.SetUpB(GameManager.Instance._upBSpell);
            _playerAttackSystem.SetDownB(GameManager.Instance._downBSpell);
            _playerAttackSystem.SetSideB(GameManager.Instance._sideBSpell);
            _playerAttackSystem.SetNeutralB(GameManager.Instance._neutralBSpell);

            _passiveManager._passives = GameManager.Instance._playerPassives;
            _passiveManager.ReloadPassives();

            _healthPoint = GameManager.Instance._playerHealth;
            _maxHealthPoint = GameManager.Instance._playerMaxHealth;

            InitializeHealthUI();
        }
    }

    public void LeftStickInput(InputAction.CallbackContext context)
    {
        _playerMovement.InputMove(context);
        _playerAttackSystem.InputDirection(context);
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        _playerMovement.InputJump(context);
    }

    public void DashInput(InputAction.CallbackContext context)
    {
        _playerMovement.InputDash(context);
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        _playerAttackSystem.InputJab(context);
    }

    public void SpecialAttackInput(InputAction.CallbackContext context)
    {
        _playerAttackSystem.InputSpecialAttack(context);
    }

    public void ShieldInput(InputAction.CallbackContext context)
    {
        _playerAttackSystem.InputShield(context);
    }

    public override void TakeDamage(int damage)
    {
        if (CanBeHit()) 
        {
            base.TakeDamage(damage);

            FightSceneManager.Instance._currentDamageReceived += damage;

            if (_cameraScript != null) 
            {
                _cameraScript.OnDamage();   
            }
            if(_playerDamageVignette != null )
            {
                _playerDamageVignette.OnHit();
            }

            FightSceneManager.Instance._playerHealthBarScript.SetHealth(_healthPoint);

            if (!_isDead)
            {
                PlayAnimation("GetHit");
                _playerAttackSystem.CancelAttack();
            }
        }
        else
        {
            _playerAttackSystem._shieldScript.HitShield(damage);
        }
    }

    public override void TakeEffectDamage(int damage)
    {
        if (!_isShielding && !_isDead && !FightSceneManager.Instance._isFightEnded)
        {
            ReceiveDamage(damage);
            FightSceneManager.Instance._currentDamageReceived += damage;
            FightSceneManager.Instance._playerHealthBarScript.SetHealth(_healthPoint);

            if (_cameraScript != null)
            {
                _cameraScript.OnDamage();
            }
            if (_playerDamageVignette != null)
            {
                _playerDamageVignette.OnHit();
            }
        }
    }

    public override void HealHp(int hp_to_heal)
    {
        base.HealHp(hp_to_heal);

        FightSceneManager.Instance._playerHealthBarScript.SetHealth(_healthPoint);
    }

    public void GetShieldBreak(float shield_break_time)
    {
        GetHitStun(shield_break_time);
    }

    public override void Die()
    {
        if (!_isPlayerDead)
        {
            base.Die();
            StartCoroutine(PlayerDeath());
        }
    }

    private IEnumerator PlayerDeath()
    {
        _isPlayerDead = true;
        _onPlayerDeath.Invoke();

        GameManager.Instance._shouldResetPlayerData = true;
        PlayAnimation("Death");

        yield return new WaitForSeconds(3f);

        GameManager.Instance.ResetData();
        GameManager.Instance.LoadStartingMenuScene();
    }

    private void InitializeHealthUI()
    {
        FightSceneManager.Instance._playerHealthBarScript.SetMaxHealth(_maxHealthPoint);
        FightSceneManager.Instance._playerHealthBarScript.SetHealth(_healthPoint);
    }

    public void AddMaxHealthPoints(int bonus_hp)
    {
        _maxHealthPoint += bonus_hp;
        _healthPoint += bonus_hp;
    }
}
