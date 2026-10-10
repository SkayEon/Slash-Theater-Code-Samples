using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/CacAttack")]
public class CacAttack : Attack
{
    [Header("Cac Attack Variables")]
    [SerializeField] private bool _isPlayerJab = false;

    public override void LaunchAttack(CharacterBehaviour user)
    {
        base.LaunchAttack(user);

        Attack(user);
    }

    private void Attack(CharacterBehaviour user)
    {
        CharacterBehaviour[] opponents = user._attackHitBox.GetCharactersInHitbox();

        foreach (CharacterBehaviour opponent in opponents)
        {
            if (opponent != null && opponent.gameObject.layer != user.gameObject.layer)
            {
                if (user.GetComponent<PlayerBehaviour>() != null && opponent.CanBeHit())
                {
                    GameObject hit_vfx = Instantiate(FightSceneManager.Instance._hitVfx, opponent.transform.position, Quaternion.identity);
                    Destroy(hit_vfx, 0.5f);
                }

                //damage opponent
                opponent.TakeDamage(CalculateDamage(user));

                if(_isPlayerJab && user._isJabIced)
                {
                    opponent._statusReciever.AddIceStack(user._iceStackBonusDamage);
                }

                //effects
                ApplyEffects(opponent);
                ApplyElementEffects(opponent);
            }
        }
    }

    public IEnumerator WaitAFrameThenAttack(CharacterBehaviour user)
    {
        yield return new WaitForEndOfFrame();
        Attack(user);
    }

    protected virtual int CalculateDamage(CharacterBehaviour user)
    {
        int damage = _damage;

        if (_isPlayerJab) 
        {
            damage += user._jabDamageBoost;
        }
        else if (_element.Equals == Element.Neutral)
        {
            damage += user._neutralAttacksBonusDamage;
        }

        return damage;
    }
}
