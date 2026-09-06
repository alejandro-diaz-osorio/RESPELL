using UnityEngine;
using UnityEngine.InputSystem;
public class DebugModifierTester : MonoBehaviour
{
    [SerializeField] private SpellController spellController;
    [SerializeField] private ModifierData modifierToTest;

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            spellController.ApplyModifier(modifierToTest);
        }
    }
}