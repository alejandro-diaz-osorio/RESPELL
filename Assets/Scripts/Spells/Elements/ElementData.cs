using UnityEngine;

[CreateAssetMenu(fileName = "NewElement", menuName = "Spells/Element Data")]
public class ElementData : ScriptableObject
{
    [Header("Identificación")]
    public string elementName = "Fire";

    [Header("Modificadores de stats (multiplicadores)")]
    [Tooltip("1 = sin cambio. Ej: 1.2 = +20% de daño")]
    public float damageMultiplier = 1f;

    [Tooltip("1 = sin cambio. Ej: 0.8 = -20% de velocidad")]
    public float speedMultiplier = 1f;

    [Header("Efecto sobre el tiempo (status effect)")]
    [Tooltip("Daño aplicado en cada tick del efecto")]
    public float tickDamage = 2f;

    [Tooltip("Tiempo en segundos entre cada tick de daño")]
    public float tickInterval = 0.5f;

    [Tooltip("Duración total del efecto en segundos")]
    public float duration = 3f;
}