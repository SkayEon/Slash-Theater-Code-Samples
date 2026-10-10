using UnityEngine;

public abstract class AttackSpawnScript : MonoBehaviour
{
    [Header("Attack Spawn Variables")]
    protected int _damage;
    protected CharacterBehaviour _user;
    protected Element _element;

    protected bool _shouldHitStun;
    protected float _hitStunDuration;

    protected bool _shouldKnockdown;
    protected float _knockdownDuration;

    protected bool _shouldKnockback;
    protected KnockbackType _knockbackType;
    protected float _knockbackForce;

    protected bool _hasAlreadyTouchedAnOpponent = false;

    public void GetStats(CharacterBehaviour user, int damage, Element element, bool should_hitstun, float hitstun_duration, bool should_knockdown, float knockdown_duration, bool should_knockback, KnockbackType knockback_type, float knockback_force)
    {
        _damage = damage;
        _user = user;
        _element = element;

        _shouldHitStun = should_hitstun;
        _hitStunDuration = hitstun_duration;

        _shouldKnockdown = should_knockdown;
        _knockdownDuration = knockdown_duration;

        _shouldKnockback = should_knockback;
        _knockbackType = knockback_type;
        _knockbackForce = knockback_force;
    }

    protected void ApplyEffects(GameObject opponent)
    {
        CharacterBehaviour character = opponent.GetComponent<CharacterBehaviour>();

        if (_shouldHitStun)
        {
            character.GetHitStun(_hitStunDuration);
        }

        if (_shouldKnockdown)
        {
            character.GetKnockdown(_knockdownDuration);
        }

        if (_shouldKnockback)
        {
            ApplyKnockback(character);
        }
    }

    protected void ApplyElementEffects(GameObject opponent)
    {
        if (_element.Equals == Element.Fire)
        {
            opponent.GetComponent<CharacterBehaviour>()._statusReciever.GetBurned(3f, _user._fireDotBonusDamage);
        }
        else if (_element.Equals == Element.Ice)
        {
            opponent.GetComponent<CharacterBehaviour>()._statusReciever.AddIceStack(_user._iceStackBonusDamage);
        }
        else if (_element.Equals == Element.Thunder && !_hasAlreadyTouchedAnOpponent)
        {
            GameObject thunder_ball = Instantiate(FightSceneManager.Instance._thunderBouncingBallPrefab.gameObject, opponent.transform.position, Quaternion.identity);
            thunder_ball.GetComponent<ThunderBouncingBallScript>().LaunchThunderBall(_user, opponent.GetComponent<CharacterBehaviour>(), _user._thunderBallBonusDamage, _user._thunderBallBonusBounce);
            _hasAlreadyTouchedAnOpponent = true;
        }
        else if (_element.Equals == Element.Wind && !_hasAlreadyTouchedAnOpponent)
        {
            GameObject tornado_hole;

            if (_user._isTornadoBoosted)
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

    protected void ApplyWindEffect()
    {
        if (_element.Equals == Element.Wind)
        {
            GameObject tornado_hole = Instantiate(FightSceneManager.Instance._tornadoHolePrefab.gameObject, transform.position, Quaternion.identity);
            tornado_hole.GetComponent<TornadoHoleScript>().StartTornado(_user);
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
            Vector2 hit_direction = GetHitHorizontalDirection(opponent) + Vector2.up * 0.6f;
            opponent.GetKnockback(hit_direction, _knockbackForce);
        }
        else if (_knockbackType.Equals == KnockbackType.Out)
        {
            Vector2 hit_direction = GetHitHorizontalDirection(opponent);
            opponent.GetKnockback(hit_direction, _knockbackForce);
        }
        else if (_knockbackType.Equals == KnockbackType.In)
        {
            Vector2 hit_direction = GetHitHorizontalDirection(opponent);
            opponent.GetKnockback(-hit_direction, _knockbackForce);
        }
    }

    private Vector2 GetHitDirection(CharacterBehaviour opponent)
    {
        Vector2 hit_direction = (opponent.transform.position - transform.position).normalized;

        return hit_direction;
    }

    private Vector2 GetHitHorizontalDirection(CharacterBehaviour opponent)
    {
        Vector2 hit_direction = opponent.transform.position - transform.position;

        hit_direction.y = 0;

        return hit_direction.normalized;
    }

    protected virtual int CalculateDamage()
    {
        int damage = _damage;

        if (_element.Equals == Element.Neutral)
        {
            damage += _user._neutralAttacksBonusDamage;
        }

        return damage;
    }

    public CharacterBehaviour GetUser()
    {
        return _user;
    }
}
