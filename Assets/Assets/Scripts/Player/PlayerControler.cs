using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpellController spellController;

    private Rigidbody2D rb;
    private Vector2 movement;

    private Vector2 lastDirection = Vector2.down;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movement = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            movement.y += 1;

        if (Keyboard.current.sKey.isPressed)
            movement.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            movement.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            movement.x += 1;

        movement = movement.normalized;

        if (movement != Vector2.zero)
        {
            lastDirection = movement;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            spellController.Cast(lastDirection);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}