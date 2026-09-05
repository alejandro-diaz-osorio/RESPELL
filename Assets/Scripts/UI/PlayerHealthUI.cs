using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("Fill Image (arrastra el objeto 'Fill' del Slider)")]
    [SerializeField] private Image fillImage;

    [Header("Colores según porcentaje de vida")]
    [SerializeField] private Color highHealthColor = Color.green;
    [SerializeField] private Color midHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;

    [Header("Umbrales (0 a 1)")]
    [SerializeField] private float midThreshold = 0.6f;
    [SerializeField] private float lowThreshold = 0.3f;

    private void Start()
    {
        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
            return;

        if (healthSlider != null)
        {
            healthSlider.maxValue = playerHealth.MaxHealth;
            healthSlider.value = playerHealth.CurrentHealth;
        }

        float healthPercent = playerHealth.MaxHealth > 0
            ? playerHealth.CurrentHealth / playerHealth.MaxHealth
            : 0f;

        UpdateHealthColor(healthPercent);

        if (healthText != null)
        {
            healthText.text =
                $"{playerHealth.CurrentHealth:0} / {playerHealth.MaxHealth:0}";
        }
    }

    private void UpdateHealthColor(float healthPercent)
    {
        if (fillImage == null)
            return;

        if (healthPercent > midThreshold)
        {
            fillImage.color = highHealthColor;
        }
        else if (healthPercent > lowThreshold)
        {
            fillImage.color = midHealthColor;
        }
        else
        {
            fillImage.color = lowHealthColor;
        }
    }
}