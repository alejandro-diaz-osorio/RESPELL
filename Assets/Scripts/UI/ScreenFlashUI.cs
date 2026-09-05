using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlashUI : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.35f);
    [SerializeField] private float flashDuration = 0.25f;

    private Coroutine flashRoutine;

    private void Awake()
    {
        if (flashImage != null)
        {
            Color startColor = flashColor;
            startColor.a = 0f;
            flashImage.color = startColor;
        }
    }

    public void Flash()
    {
        if (flashImage == null)
            return;

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float elapsed = 0f;
        Color color = flashColor;

        // Empieza visible y se desvanece suavemente
        flashImage.color = color;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;

            color.a = Mathf.Lerp(flashColor.a, 0f, t);
            flashImage.color = color;

            yield return null;
        }

        color.a = 0f;
        flashImage.color = color;
        flashRoutine = null;
    }
}