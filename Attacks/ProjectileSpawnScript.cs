using UnityEngine;

public class ProjectileSpawnScript : AttackSpawnScript
{
    [Header("Projectile Spawn Variables")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private bool _shouldDestroyOnWalls = true;
    [SerializeField] private bool _shouldDestroyOnGround = true;
    [SerializeField] private bool _shouldFaceMovementDirection = true;

    protected virtual void Update()
    {
        if (_shouldFaceMovementDirection)
        {
            float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public virtual void GetLaunched(int direction, float angle, float launch_force)
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        Vector2 launch_direction = new Vector2(Mathf.Cos(angleInRadians) * direction, Mathf.Sin(angleInRadians));
        _rb.AddForce(launch_direction * launch_force, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterBehaviour character = collision.gameObject.GetComponent<CharacterBehaviour>();

        if (character != null && character != _user)
        {
            if (!character._hasReflect)
            {
                TriggerCollision(collision);
            }
            else
            {
                character._statusReciever.ReflectProjectile(this);
            }
        }

        if (collision.gameObject.CompareTag("Ground") && _shouldDestroyOnGround)
        {
            ApplyWindEffect();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall") && _shouldDestroyOnWalls)
        {
            ApplyWindEffect();
            Destroy(gameObject);
        }
    }

    protected virtual void TriggerCollision(Collider2D collision)
    {
        if (collision.gameObject.layer != _user.gameObject.layer)
        {
            CharacterBehaviour character = collision.gameObject.GetComponent<CharacterBehaviour>();

            character.TakeDamage(CalculateDamage());

            ApplyEffects(collision.gameObject);
            ApplyElementEffects(collision.gameObject);

            Destroy(gameObject);
        }
    }

    public void GetReflected(float damage_multiplicator, Element new_element, CharacterBehaviour new_user)
    {
        _user = new_user;
        _damage = (int)(_damage * damage_multiplicator);
        _element = new_element;
        _rb.linearVelocity = new Vector2(-_rb.linearVelocity.x, _rb.linearVelocity.y);
    }
}
