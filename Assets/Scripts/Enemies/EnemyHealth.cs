using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 30f;

    [Header("Damage Flash")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Death Feedback")]
    [SerializeField] private float deathDuration = 0.25f;

    private float currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;
    private bool isDead;

    public event Action<EnemyHealth> OnEnemyDeath;

    private void Awake()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        Debug.Log(
            $"{gameObject.name} recibió {damage} de daño. " +
            $"HP: {currentHealth}/{maxHealth}"
        );

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Si ya estaba parpadeando (golpe rápido seguido), reiniciamos el parpadeo
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Si estaba parpadeando al morir, detenemos la corrutina
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            spriteRenderer.color = originalColor;
        }

        // Avisamos de inmediato: el contador de enemigos y RoomCombat
        // no deben esperar a que termine la animación de muerte
        OnEnemyDeath?.Invoke(this);

        // Evitamos que el "cadáver" siga bloqueando o recibiendo golpes
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Color startColor = spriteRenderer.color;

        while (elapsed < deathDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / deathDuration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            Color fadedColor = startColor;
            fadedColor.a = Mathf.Lerp(startColor.a, 0f, t);
            spriteRenderer.color = fadedColor;

            yield return null;
        }

        Destroy(gameObject);
    }
}