using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;

    private float damage;
    private float lifetime;

    private bool piercing;
    private bool bouncing;

    public void Initialize(Vector2 direction, SpellStats stats)
    {
        rb = GetComponent<Rigidbody2D>();

        damage = stats.damage;
        lifetime = stats.lifetime;

        piercing = stats.piercing;
        bouncing = stats.bouncing;

        transform.localScale = Vector3.one * stats.size;

        rb.linearVelocity = direction.normalized * stats.speed;

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