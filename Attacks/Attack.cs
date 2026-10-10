using UnityEngine;
using UnityEngine.Localization;

public enum Element
{
    Neutral,
    Fire,
    Ice,
    Thunder,
    Wind
}

public enum Rarity
{
    Common,
    Rare,
    Legendary
}

public enum KnockbackType
{
    Up,
    UpOut,
    Out,
    In
}

public abstract class Attack : ScriptableObject
{
    [Header("Displayable variables")]
    public LocalizedString _name;
    public LocalizedString _description;
    public Sprite _icon;

    [Header("Attack Stats")]
    public int _damage;
    public float _cooldown;
    public Element _element;

    [Header("Rarity")]
    public Rarity _rarity;

    [Header("Hit Stun")]
    public bool _shouldHitStun = false;
    public float _hitStunDuration = 0.5f;

    [Header("Knockdown")]
    public bool _shouldKnockdown = false;
    public float _knockdownDuration = 0.5f;

    [Header("Knockback")]
    public bool _shouldKnockback = false;
    public KnockbackType _knockbackType;
    public float _knockbackForce = 3.0f;

    [Header("Attack Duration")]
    public float _startingAttackDuration = 0.1f;
    public float _attackDuration = 0.1f;

    [Header("Player Animations Names")]
    public string _startingAttackPlayerAnimation;
    public string _attackPlayerAnimation;

    private CharacterBehaviour _user;

    [HideInInspector] public bool _hasAlreadyTouchedAnOpponent = false;

    public virtual void LaunchAttack(CharacterBehaviour user)
    {
        _user = user;
    }

    protected void ApplyEffects(CharacterBehaviour opponent)
    {
        if (_shouldHitStun)
        {
            opponent.GetHitStun(_hitStunDuration);
        }

        if (_shouldKnockdown)
        {
            opponent.GetKnockdown(_knockdownDuration);
        }

        if (_shouldKnockback) 
        {
            ApplyKnockback(opponent);
        }
    }

    // Applies additional effects associated with the attack's element.
    // Some elemental effects can only be triggered once per attack.
    protected void ApplyElementEffects(CharacterBehaviour opponent)
    {
        if (_element.Equals == Element.Fire)
        {
            opponent._statusReciever.GetBurned(3f, _user._fireDotBonusDamage);
        }
        else if (_element.Equals == Element.Ice)
        {
            opponent._statusReciever.AddIceStack(_user._iceStackBonusDamage);
        }
        else if (_element.Equals == Element.Thunder)
        {
            GameObject thunder_ball = Instantiate(FightSceneManager.Instance._thunderBouncingBallPrefab.gameObject, opponent.transform.position, Quaternion.identity);
            thunder_ball.GetComponent<ThunderBouncingBallScript>().LaunchThunderBall(_user, opponent, _user._thunderBallBonusDamage, _user._thunderBallBonusBounce);
            _hasAlreadyTouchedAnOpponent = true;
        }
        else if (_element.Equals == Element.Wind)
        {
            GameObject tornado_hole;

            if(_user._isTornadoBoosted)
            {
                tornado_hole = Instantiate(FightSceneManager.Instance._tornadoHoleSpikePrefab.gameObject, opponent.transform.position, Quaternion.identity);
                tornado_hole.GetComponent<TornadoHoleScript>().StartTornado(_user);
            }
            else
            {
                tornado_hole = Instantiate(FightSceneManager.Instance._tornadoHolePrefab.gameObject, opponent.transform.position, Quaternion.identity);
                tornado_hole.GetComponent<TornadoHoleScript>().StartTornado(_user);
            }

            _hasAlreadyTouchedAnOpponent = true;
        }
    }

    private void ApplyKnockback(CharacterBehaviour opponent)
    {
        if (_knockbackType.Equals == KnockbackType.Up)
        {
            opponent.GetKnockback(Vector2.up, _knockbackForce);
        }
        else if (_knockbackType.Equals == KnockbackType.UpOut)
        {
            Vector2 hit_direction = GetHitDirectionFromUserFacing(opponent) + Vector2.up * 0.6f;
            opponent.GetKnockback(hit_direction, _knockbackForce);
        }
        else if (_knockbackType.Equals == KnockbackType.Out)
        {
            Vector2 hit_direction = GetHitDirectionFromUserFacing(opponent);
            opponent.GetKnockback(hit_direction, _knockbackForce);
        }
        else if (_knockbackType.Equals == KnockbackType.In)
        {
            Vector2 hit_direction = GetHitDirectionFromUserFacing(opponent);
            opponent.GetKnockback(-hit_direction, _knockbackForce);
        }
    }

    public Vector2 GetHitDirectionFromUserFacing(CharacterBehaviour opponent)
    {
        Vector2 hit_direction = opponent.transform.position - _user.transform.position;

        hit_direction.y = 0;

        if (_user.GetIsFacingRight())
        {
            hit_direction.x = Mathf.Abs(hit_direction.x);
        }
        else
        {
            hit_direction.x = -Mathf.Abs(hit_direction.x);
        }

        return hit_direction.normalized;
    }

    public Vector2 GetHitDirectionFromUserPosition(CharacterBehaviour opponent)
    {
        return (opponent.transform.position - _user.transform.position).normalized;
    }

    public Vector2 GetHitDirectionFromAttackPosition(CharacterBehaviour opponent)
    {
        return (opponent.transform.position - _user._attackHitBox.transform.position).normalized;
    }
}
