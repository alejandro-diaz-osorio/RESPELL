using UnityEngine;

public class Door : MonoBehaviour
{
    private Collider2D doorCollider;
    private SpriteRenderer spriteRenderer;

    private bool isOpen;

    private void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Open()
    {
        isOpen = true;

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        Debug.Log(gameObject.name + " abierta.");
    }

    public void Close()
    {
        isOpen = false;

        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        Debug.Log(gameObject.name + " cerrada.");
    }

    public bool IsOpen => isOpen;
}