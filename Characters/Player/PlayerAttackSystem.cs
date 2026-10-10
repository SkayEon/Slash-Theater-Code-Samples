using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackSystem : MonoBehaviour
{
    [Header("Needed References")]
    [SerializeField] private PlayerBehaviour _playerBehaviour;
    public ShieldScript _shieldScript;

    [Header("Attacks")]
    [SerializeField] private Attack _actualUpBAttack;
    [SerializeField] private Attack _actualDownBAttack;
    [SerializeField] private Attack _actualSideBAttack;
    [SerializeField] private Attack _actualNeutralBAttack;

    // Spells CD UI
    private UISpell _upBUI;
    private UISpell _downBUI;
    private UISpell _sideBUI;
    private UISpell _neutralBUI;

    private Vector2 _actualDirectionInput;

    // Cooldowns
    private bool _isJabOnCooldown = false;
    private bool _isJabComboOnCooldown = false;

    private float _attackCooldownJab = 0f;

    private float _jabComboCooldownMaxDuration = 1f;
    private float _jabComboCooldown = 0f;

    private bool _isUpBOnCooldown = false;
    private bool _isDownBOnCooldown = false;
    private bool _isSideBOnCooldown = false;
    private bool _isNeutralBOnCooldown = false;

    private float _attackCooldownUpB = 0f;
    private float _attackCooldownDownB = 0f;
    private float _attackCooldownSideB = 0f;
    private float _attackCooldownNeutralB = 0f;

    private const float _deadzone = 0.6f;

    [Header("Jab Combo")]
    public Attack[] _jabCombo;
    public float _comboResetTime = 0.7f;
    private int _comboIndex = 0;
    private float _comboTimer = 0f;

    private GameObject _currentAuraVfx;

    private void Start()
    {
        _upBUI = FightSceneManager.Instance._cdCanvasScript._upBUI;
        _downBUI = FightSceneManager.Instance._cdCanvasScript._downBUI;
        _sideBUI = FightSceneManager.Instance._cdCanvasScript._sideBUI;
        _neutralBUI = FightSceneManager.Instance._cdCanvasScript._neutralBUI;

        _playerBehaviour._onCharacterDeath.AddListener(CancelAttack);
    }

    public void Update()
    {
        AttacksCooldownsUpdate();
        JabComboResetTimerUpdate();
    }

    // Inputs
    public void InputDirection(InputAction.CallbackContext context)
    {
        _actualDirectionInput = context.ReadValue<Vector2>();
    }

    public void InputJab(InputAction.CallbackContext context)
    {
        if(_playerBehaviour.CanAct() && !_playerBehaviour._isShielding && !_playerBehaviour._isAttacking && !_isJabOnCooldown && !_isJabComboOnCooldown)
        {
            DoJabCombo();
        }
    }

    public void InputSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_playerBehaviour.CanAct() && !_playerBehaviour._isShielding && !_playerBehaviour._isAttacking)
            {
                LaunchBMove(_actualDirectionInput);
            }
        }
    }

    public void InputShield(InputAction.CallbackContext context)
    {
        if (context.started && _playerBehaviour.CanAct() && _playerBehaviour._isGrounded && _shieldScript._canShieldBeActivated && !_playerBehaviour._isAttacking)
        {
            _shieldScript.ActivateShield();
        }

        if(context.canceled)
        {
            _shieldScript.DeactivateShield();
        }
    }

    //Attacks logic
    private void DoJabCombo()
    {
        if (_comboIndex < _jabCombo.Length)
        {
            _comboTimer = 0f;

            // Play attack and jab animation corresponding to combo index
            if (_comboIndex == 0)
            {
                StartCoroutine(JabAttackProgress(_jabCombo[_comboIndex], "Jab1"));
            }
            else if (_comboIndex == 1) 
            {
                StartCoroutine(JabAttackProgress(_jabCombo[_comboIndex], "Jab2"));
            }
            else if (_comboIndex == 2)
            {
                StartCoroutine(JabAttackProgress(_jabCombo[_comboIndex], "Jab3"));
            }
            else if (_comboIndex == 3)
            {
                // Dernier coup = projection
                StartCoroutine(JabAttackProgress(_jabCombo[_comboIndex], "Jab4"));
                ResetCombo();
            }

            _isJabOnCooldown = true;
            _attackCooldownJab = _jabCombo[_comboIndex]._cooldown;

            if(_comboIndex != 3)
            {
                _comboIndex++;
            }
        }
    }

    private void ResetCombo()
    {
        _isJabComboOnCooldown = true;
        _jabComboCooldown = _comboTimer;

        _comboIndex = 0; 
        _comboTimer = 0f;
    }

    private void LaunchBMove(Vector2 direction)
    {
        float x = direction.x;
        float y = direction.y;

        // UP B
        if (y > _deadzone)
        {
            if (_actualUpBAttack != null && !_isUpBOnCooldown)
            {
                StartCoroutine(AttackProgress(_actualUpBAttack));
                
                _attackCooldownUpB = _actualUpBAttack._cooldown;
                _isUpBOnCooldown = true;
            }
        }
        // DOWN B
        else if (y < -_deadzone)
        {
            if (_actualDownBAttack != null && !_isDownBOnCooldown)
            {
                StartCoroutine(AttackProgress(_actualDownBAttack));

                _attackCooldownDownB = _actualDownBAttack._cooldown;
                _isDownBOnCooldown = true;
            }
        }
        // SIDE B 
        else if (Mathf.Abs(x) > _deadzone)
        {
            if (_actualSideBAttack != null && !_isSideBOnCooldown)
            {
                StartCoroutine(AttackProgress(_actualSideBAttack));

                _attackCooldownSideB = _actualSideBAttack._cooldown;
                _isSideBOnCooldown = true;
            }
        }
        // NEUTRAL B
        else
        {
            if (_actualNeutralBAttack != null && !_isNeutralBOnCooldown)
            {
                StartCoroutine(AttackProgress(_actualNeutralBAttack));

                _attackCooldownNeutralB = _actualNeutralBAttack._cooldown;
                _isNeutralBOnCooldown = true;
            }
        }
    }

    //attacks getters and setters
    public void SetUpB(Attack attack)
    {
        _actualUpBAttack = attack;

        if(GameManager.Instance != null)
        {
            GameManager.Instance._upBSpell = attack;
        }
    }

    public void SetDownB(Attack attack)
    {
        _actualDownBAttack = attack;

        if (GameManager.Instance != null)
        {
            GameManager.Instance._downBSpell = attack;
        }
    }

    public void SetSideB(Attack attack)
    {
        _actualSideBAttack = attack;

        if (GameManager.Instance != null)
        {
            GameManager.Instance._sideBSpell = attack;
        }
    }

    public void SetNeutralB(Attack attack)
    {
        _actualNeutralBAttack = attack;

        if (GameManager.Instance != null)
        {
            GameManager.Instance._neutralBSpell = attack;
        }
    }

    public Attack GetUpB()
    {
        return _actualUpBAttack;
    }

    public Attack GetDownB()
    {
        return _actualDownBAttack;
    }

    public Attack GetSideB()
    {
        return _actualSideBAttack;
    }

    public Attack GetNeutralB()
    {
        return _actualNeutralBAttack;
    }

    //Cooldowns Updates
    private void AttacksCooldownsUpdate()
    {
        if (_isJabOnCooldown)
        {
            _attackCooldownJab -= Time.deltaTime;
            if (_attackCooldownJab <= 0f)
            {
                _isJabOnCooldown = false;
            }
        }

        if (_isJabComboOnCooldown)
        {
            _jabComboCooldown += Time.deltaTime;
            if (_jabComboCooldown >= _jabComboCooldownMaxDuration)
            {
                _isJabComboOnCooldown = false;
                _jabComboCooldown = 0f;
            }
        }

        if (_isUpBOnCooldown)
        {
            _attackCooldownUpB -= Time.deltaTime;
            _upBUI.UpdateCooldown(_attackCooldownUpB, _actualUpBAttack._cooldown);
            if (_attackCooldownUpB <= 0f)
            {
                _isUpBOnCooldown = false;
            }
        }

        if (_isDownBOnCooldown)
        {
            _attackCooldownDownB -= Time.deltaTime;
            _downBUI.UpdateCooldown(_attackCooldownDownB, _actualDownBAttack._cooldown);
            if (_attackCooldownDownB <= 0f)
            {
                _isDownBOnCooldown = false;
            }
        }

        if (_isSideBOnCooldown)
        {
            _attackCooldownSideB -= Time.deltaTime;
            _sideBUI.UpdateCooldown(_attackCooldownSideB, _actualSideBAttack._cooldown);
            if (_attackCooldownSideB <= 0f)
            {
                _isSideBOnCooldown = false;
            }
        }

        if (_isNeutralBOnCooldown)
        {
            _attackCooldownNeutralB -= Time.deltaTime;
            _neutralBUI.UpdateCooldown(_attackCooldownNeutralB, _actualNeutralBAttack._cooldown);
            if (_attackCooldownNeutralB <= 0f)
            {
                _isNeutralBOnCooldown = false;
            }
        }
    }

    private void JabComboResetTimerUpdate()
    {
        if (_comboIndex > 0) // condition pour reset le combo
        {
            _comboTimer += Time.deltaTime;
            if (_comboTimer > _comboResetTime)
            {
                ResetCombo();
            }
        }
    }

    private IEnumerator AttackProgress(Attack attack)
    {
        _playerBehaviour._isAttacking = true;

        _playerBehaviour.PlayAnimation(attack._startingAttackPlayerAnimation);

        PlayAuraVfx(attack);

        yield return new WaitForSeconds(attack._startingAttackDuration);

        _playerBehaviour.PlayAnimation(attack._attackPlayerAnimation);

        if(_currentAuraVfx != null)
        {
            Destroy(_currentAuraVfx);
        }

        attack.LaunchAttack(_playerBehaviour);

        yield return new WaitForSeconds(attack._attackDuration);

        _playerBehaviour._isAttacking = false;
    }

    private void PlayAuraVfx(Attack attack)
    {
        if (attack._element == Element.Neutral)
        {
            _currentAuraVfx = Instantiate(FightSceneManager.Instance._neutralAuraVfx, transform);
        }
        else if (attack._element == Element.Fire)
        {
            _currentAuraVfx = Instantiate(FightSceneManager.Instance._fireAuraVfx, transform);
        }
        else if (attack._element == Element.Ice)
        {
            _currentAuraVfx = Instantiate(FightSceneManager.Instance._iceAuraVfx, transform);
        }
        else if (attack._element == Element.Thunder)
        {
            _currentAuraVfx = Instantiate(FightSceneManager.Instance._thunderAuraVfx, transform);
        }
        else if (attack._element == Element.Wind)
        {
            _currentAuraVfx = Instantiate(FightSceneManager.Instance._windAuraVfx, transform);
        }
    }

    private IEnumerator JabAttackProgress(Attack attack, string anim_jab_name)
    {
        _playerBehaviour._isAttacking = true;

        _playerBehaviour.PlayAnimation(anim_jab_name);

        yield return new WaitForSeconds(attack._startingAttackDuration);

        attack.LaunchAttack(_playerBehaviour);

        yield return new WaitForSeconds(attack._attackDuration);

        _playerBehaviour._isAttacking = false;
    }

    public void CancelAttack()
    {
        StopAllCoroutines();

        _playerBehaviour._isAttacking = false;

        if(_currentAuraVfx != null)
        {
            Destroy(_currentAuraVfx);
        }
    }
}
