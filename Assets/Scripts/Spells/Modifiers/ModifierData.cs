using UnityEngine;

[CreateAssetMenu(fileName = "NewModifier", menuName = "Spells/Modifier Data")]
public class ModifierData : ScriptableObject
{
    [Header("Identificación")]
    public string modifierName;

    [Header("Valores aditivos (0 = sin cambio)")]
    public float damageBonus = 0f;
    public float speedBonus = 0f;
    public float fireRateBonus = 0f;
    public int projectileCountBonus = 0;

    [Header("Flags a activar")]
    public bool enablePiercing = false;
    public bool enableBouncing = false;
    public bool enableExplosive = false;
    public bool enableHoming = false;
}