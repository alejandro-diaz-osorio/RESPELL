using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShooterEnemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float preferredDistance = 5f;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float fireRate = 2f;

    private Rigidbody2D rb;
    private Transform player;
    private RoomCombat room;

    private float nextFireTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        room = GetComponentInParent<RoomCombat>();
    }

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null || room == null || !room.IsPlayerInside)
            return;

        if (Time.time >= nextFireTime)
        {
            Shoot();

            nextFireTime =
                Time.time + fireRate;
        }
    }

    private void FixedUpdate()
    {
        if (player == null || room == null || !room.IsPlayerInside)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );

        Vector2 direction =
            (player.position - transform.position).normalized;

        if (distance > preferredDistance)
        {
            rb.linearVelocity =
                direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Shoot()
    {
        Vector2 direction =
            (player.position - transform.position)
            .normalized;

        GameObject projectile =
            Instantiate(
                projectilePrefab,
                transform.position,
                Quaternion.identity
            );

        EnemyProjectile projectileScript =
            projectile.GetComponent<EnemyProjectile>();

        projectileScript.Initialize(
            direction,
            projectileSpeed
        );
    }
}