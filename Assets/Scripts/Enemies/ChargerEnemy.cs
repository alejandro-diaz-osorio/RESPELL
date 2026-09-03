using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ChargerEnemy : MonoBehaviour
{
    [SerializeField] private float chargeSpeed = 8f;
    [SerializeField] private float chargeDuration = 0.5f;
    [SerializeField] private float preparationTime = 1.5f;
    [SerializeField] private float contactDamage = 20f;
    [SerializeField] private float damageCooldown = 1f;

    private Rigidbody2D rb;
    private Transform player;
    private RoomCombat room;

    private Vector2 chargeDirection;

    private float stateTimer;
    private float nextDamageTime;

    private enum State
    {
        Preparing,
        Charging
    }

    private State currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        room = GetComponentInParent<RoomCombat>();

        currentState = State.Preparing;
        stateTimer = preparationTime;
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

        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            ChangeState();
        }
    }

    private void FixedUpdate()
    {
        if (player == null || room == null || !room.IsPlayerInside)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (currentState == State.Preparing)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (currentState == State.Charging)
        {
            rb.linearVelocity =
                chargeDirection * chargeSpeed;
        }
    }

    private void ChangeState()
    {
        if (currentState == State.Preparing)
        {
            chargeDirection =
                (player.position - transform.position)
                .normalized;

            currentState = State.Charging;
            stateTimer = chargeDuration;
        }
        else
        {
            currentState = State.Preparing;
            stateTimer = preparationTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < nextDamageTime)
            return;

        PlayerHealth playerHealth =
            collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(contactDamage);

            nextDamageTime =
                Time.time + damageCooldown;
        }
    }
}