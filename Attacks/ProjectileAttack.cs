using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/ProjectileAttack")]
public class ProjectileAttack : Attack
{
    [Header("Projectile Attack Variables")]
    [SerializeField] protected GameObject _projectilePrefab;
    [SerializeField] protected float _angle = 35f;
    [SerializeField] protected float _launchForce = 10f;

    public override void LaunchAttack(CharacterBehaviour user)
    {
        base.LaunchAttack(user);

        GameObject instantiated_projectile = Instantiate(_projectilePrefab, user._attackHitBox.transform.position, Quaternion.identity);

        bool facing_direction = user.GetIsFacingRight();

        int direction;

        if (facing_direction)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        ProjectileSpawnScript projectile = instantiated_projectile.GetComponent<ProjectileSpawnScript>();

        projectile.GetLaunched(direction, _angle, _launchForce);

        projectile.GetStats(user, _damage, _element, _shouldHitStun, _hitStunDuration, _shouldKnockdown, _knockdownDuration, _shouldKnockback, _knockbackType, _knockbackForce);
    }
}
