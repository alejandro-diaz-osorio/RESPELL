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
    private Coroutine statusEffectRoutine;
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

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    public void ApplyElement(ElementData element)
    {
        if (isDead || element == null)
            return;

        // Si el elemento no define daño sobre el tiempo, no hacemos nada
        if (element.tickDamage <= 0f)
            return;

        // Si ya había un efecto activo, lo reiniciamos con el nuevo
        if (statusEffectRoutine != null)
        {
            StopCoroutine(statusEffectRoutine);
        }

        statusEffectRoutine = StartCoroutine(StatusEffectRoutine(element));
    }

    private IEnumerator StatusEffectRoutine(ElementData element)
    {
        float elapsed = 0f;

        while (elapsed < element.duration)
        {
            yield return new WaitForSeconds(element.tickInterval);

            elapsed += element.tickInterval;

            if (isDead)
                yield break;

            TakeDamage(element.tickDamage);
        }

        statusEffectRoutine = null;
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

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            spriteRenderer.color = originalColor;
        }

        if (statusEffectRoutine != null)
        {
            StopCoroutine(statusEffectRoutine);
        }

        OnEnemyDeath?.Invoke(this);

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