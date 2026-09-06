using System.Collections;
using UnityEngine;

public class ExplosionVisual : MonoBehaviour
{
    private const float duration = 0.25f;
    private const int textureSize = 64;

    private static Sprite cachedDefaultSprite;

    private SpriteRenderer spriteRenderer;

    public static void Spawn(Vector3 position, float radius, Sprite customSprite)
    {
        GameObject obj = new GameObject("ExplosionVisual");
        obj.transform.position = position;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = customSprite != null ? customSprite : GetDefaultCircleSprite();
        sr.sortingOrder = 10;
        sr.color = new Color(1f, 0.5f, 0f, 0.85f); // naranja semitransparente

        obj.transform.localScale = Vector3.zero;

        ExplosionVisual visual = obj.AddComponent<ExplosionVisual>();
        visual.Play(radius);
    }

    private static Sprite GetDefaultCircleSprite()
    {
        if (cachedDefaultSprite != null)
            return cachedDefaultSprite;

        Texture2D texture = new Texture2D(textureSize, textureSize);
        Vector2 center = new Vector2(textureSize / 2f, textureSize / 2f);
        float radius = textureSize / 2f;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                texture.SetPixel(x, y, distance <= radius ? Color.white : Color.clear);
            }
        }

        texture.Apply();

        cachedDefaultSprite = Sprite.Create(
            texture,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize // pixels per unit = tamaño de textura -> el sprite mide 1x1 unidad
        );

        return cachedDefaultSprite;
    }

    private void Play(float radius)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(PlayRoutine(radius));
    }

    private IEnumerator PlayRoutine(float radius)
    {
        float elapsed = 0f;

        float spriteWorldSize = spriteRenderer.sprite.bounds.size.x;
        Vector3 targetScale = Vector3.one * (radius * 2f / spriteWorldSize);

        Color startColor = spriteRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

            Color fadedColor = startColor;
            fadedColor.a = Mathf.Lerp(startColor.a, 0f, t);
            spriteRenderer.color = fadedColor;

            yield return null;
        }

        Destroy(gameObject);
    }
}