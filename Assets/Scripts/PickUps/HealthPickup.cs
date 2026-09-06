using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private float healPercentage;

    public void SetHealPercentage(float percent)
    {
        healPercentage = percent;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.HealPercentage(healPercentage);
        }

        Destroy(gameObject);
    }
}