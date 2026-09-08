using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    [Header("Damage Flash")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Screen Feedback")]
    [SerializeField] private ScreenFlashUI screenFlash;

    private float currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        currentHealth =
            Mathf.Max(currentHealth, 0);

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

        if (screenFlash != null)
        {
            screenFlash.Flash();
        }
    }
    public void HealPercentage(float percent)
    {
        float healAmount = maxHealth * percent;

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    }

    private IEnumerator FlashRoutine()
    {
        float halfDuration = flashDuration / 2f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            spriteRenderer.color = Color.Lerp(originalColor, flashColor, t);
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            spriteRenderer.color = Color.Lerp(flashColor, originalColor, t);
            yield return null;
        }

        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

    private void Die()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        spriteRenderer.color = originalColor;
        gameObject.SetActive(false);
    }
}