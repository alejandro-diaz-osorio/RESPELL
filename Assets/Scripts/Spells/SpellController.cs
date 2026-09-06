using UnityEngine;

public class SpellController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    [Header("Referencia para calcular velocidad relativa (opcional)")]
    [SerializeField] private PlayerController playerController;

    [Header("Stats base del hechizo (editables en el Inspector, incluso en Play)")]
    [SerializeField] private SpellStats baseStats = new SpellStats();

    [Header("Elemento activo (temporal: asignación manual)")]
    [SerializeField] private ElementData element;

    public SpellStats Stats { get; private set; }

    private float nextCastTime;

    private void Awake()
    {
        Stats = baseStats;
    }

    public void Cast(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

        if (Time.time < nextCastTime)
            return;

        float interval = 1f / Mathf.Max(Stats.fireRate, 0.01f);
        nextCastTime = Time.time + interval;

        float referenceSpeed = playerController != null
            ? playerController.MoveSpeed
            : 0f;

        float damageMultiplier = element != null ? element.damageMultiplier : 1f;
        float speedMultiplier = element != null ? element.speedMultiplier : 1f;

        float effectiveDamage = Stats.damage * damageMultiplier;
        float finalSpeed = (referenceSpeed + Stats.speed) * speedMultiplier;

        for (int i = 0; i < Stats.projectileCount; i++)
        {
            Vector3 spawnPosition = transform.position +
                        (Vector3)(direction.normalized * 0.6f);

            GameObject projectile = Instantiate(
                projectilePrefab,
                spawnPosition,
                Quaternion.identity
            );

            Projectile projectileScript =
                projectile.GetComponent<Projectile>();

            projectileScript.Initialize(direction, Stats, finalSpeed, effectiveDamage, element);
        }
    }

    public void ApplyModifier(ModifierData modifier)
    {
        if (modifier == null)
            return;

        Stats.damage += modifier.damageBonus;
        Stats.speed += modifier.speedBonus;
        Stats.fireRate += modifier.fireRateBonus;
        Stats.projectileCount += modifier.projectileCountBonus;

        Stats.piercing |= modifier.enablePiercing;
        Stats.bouncing |= modifier.enableBouncing;
        Stats.explosive |= modifier.enableExplosive;
        Stats.homing |= modifier.enableHoming;

        Debug.Log($"Modificador aplicado: {modifier.modifierName}");
    }
}