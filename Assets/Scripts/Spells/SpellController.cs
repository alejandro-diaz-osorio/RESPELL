using UnityEngine;

public class SpellController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    public SpellStats Stats { get; private set; }

    private void Awake()
    {
        Stats = new SpellStats();
    }

    public void Cast(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

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

            projectileScript.Initialize(direction, Stats);
        }
    }
}