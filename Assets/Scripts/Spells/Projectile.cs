using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Explosive Visual (opcional)")]
    [Tooltip("Deja vacío para usar un círculo genérico por defecto")]
    [SerializeField] private Sprite explosionSprite;

    private Rigidbody2D rb;
    private Collider2D col;

    private float damage;
    private float lifetime;

    private bool piercing;
    private bool bouncing;
    private bool explosive;
    private float explosionRadius;
    private float explosionDamage;

    private bool homing;
    private float homingTurnSpeed;
    private float homingDetectionRadius;
    private Transform homingTarget;
    private float currentSpeed;

    private ElementData element;
    private Vector2 previousVelocity;

    public void Initialize(
        Vector2 direction,
        SpellStats stats,
        float finalSpeed,
        float effectiveDamage,
        ElementData element)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        damage = effectiveDamage;
        lifetime = stats.lifetime;

        piercing = stats.piercing;
        bouncing = stats.bouncing;
        explosive = stats.explosive;
        explosionRadius = stats.explosionRadius;
        explosionDamage = stats.explosionDamage;

        homing = stats.homing;
        homingTurnSpeed = stats.homingTurnSpeed;
        homingDetectionRadius = stats.homingDetectionRadius;
        currentSpeed = finalSpeed;

        this.element = element;

        transform.localScale = Vector3.one * stats.size;

        rb.linearVelocity = direction.normalized * finalSpeed;
        previousVelocity = rb.linearVelocity;

        if (homing)
        {
            homingTarget = FindClosestEnemy();
        }

        Invoke(nameof(ExpireLifetime), lifetime);
    }

    private Transform FindClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, homingDetectionRadius);

        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = hit.transform;
            }
        }

        return closest;
    }

    private void FixedUpdate()
    {
        if (homing && homingTarget != null)
        {
            Vector2 currentDirection = rb.linearVelocity.normalized;
            Vector2 targetDirection =
                ((Vector2)homingTarget.position - rb.position).normalized;

            Vector2 newDirection = Vector3.RotateTowards(
                currentDirection,
                targetDirection,
                homingTurnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime,
                0f
            );

            rb.linearVelocity = newDirection * currentSpeed;
        }

        previousVelocity = rb.linearVelocity;
    }

    private void ExpireLifetime()
    {
        if (explosive)
        {
            Explode();
        }

        Destroy(gameObject);
    }

    private void Explode()
    {
        ExplosionVisual.Spawn(transform.position, explosionRadius, explosionSprite);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(explosionDamage);

                if (element != null)
                {
                    enemyHealth.ApplyElement(element);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth =
                collision.gameObject.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);

                if (element != null)
                {
                    enemyHealth.ApplyElement(element);
                }
            }

            if (explosive)
            {
                Explode();
            }

            if (piercing)
            {
                rb.linearVelocity = previousVelocity;
                Physics2D.IgnoreCollision(col, collision.collider, true);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            if (bouncing)
            {
                Vector2 normal = collision.GetContact(0).normal;

                Vector2 reflectedVelocity =
                    Vector2.Reflect(previousVelocity, normal);

                rb.linearVelocity = reflectedVelocity;
                previousVelocity = reflectedVelocity;
            }
            else
            {
                if (explosive)
                {
                    Explode();
                }

                Destroy(gameObject);
            }
        }
    }
}