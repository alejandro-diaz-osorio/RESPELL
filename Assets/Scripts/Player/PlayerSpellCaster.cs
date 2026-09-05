using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpellCaster : MonoBehaviour
{
    [SerializeField] private SpellController spellController;

    private void Update()
    {
        Vector2 aimDirection = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed)
            aimDirection.y += 1;

        if (Keyboard.current.downArrowKey.isPressed)
            aimDirection.y -= 1;

        if (Keyboard.current.leftArrowKey.isPressed)
            aimDirection.x -= 1;

        if (Keyboard.current.rightArrowKey.isPressed)
            aimDirection.x += 1;

        if (aimDirection == Vector2.zero)
            return;

        spellController.Cast(aimDirection.normalized);
    }
}