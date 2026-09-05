using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;

    private float damage;
    private float lifetime;

    private bool piercing;
    private bool bouncing;

    private ElementData element;

    public void Initialize(
        Vector2 direction,
        SpellStats stats,
        float finalSpeed,
        float effectiveDamage,
        ElementData element)
    {
        rb = GetComponent<Rigidbody2D>();

        damage = effectiveDamage;
        lifetime = stats.lifetime;

        piercing = stats.piercing;
        bouncing = stats.bouncing;

        this.element = element;

        transform.localScale = Vector3.one * stats.size;

        rb.linearVelocity = direction.normalized * finalSpeed;

        Destroy(gameObject, lifetime);
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

            if (!piercing)
            {
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            if (!bouncing)
            {
                Destroy(gameObject);
            }
        }
    }
}