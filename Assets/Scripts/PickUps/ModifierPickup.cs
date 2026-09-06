using UnityEngine;

public class ModifierPickup : MonoBehaviour
{
    private ModifierData modifierData;

    public void SetModifier(ModifierData data)
    {
        modifierData = data;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        SpellController spellController = other.GetComponent<SpellController>();

        if (spellController != null && modifierData != null)
        {
            spellController.ApplyModifier(modifierData);
        }

        Destroy(gameObject);
    }
}