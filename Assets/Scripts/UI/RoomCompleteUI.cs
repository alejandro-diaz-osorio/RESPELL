using System.Collections;
using UnityEngine;
using TMPro;

public class RoomCompleteUI : MonoBehaviour
{
    [SerializeField] private TMP_Text RoomCompleteText;
    [SerializeField] private float displayDuration = 5f;

    private Coroutine hideRoutine;

    private void OnEnable()
    {
        RoomCombat.OnAnyRoomCompleted += HandleRoomCompleted;
    }

    private void OnDisable()
    {
        RoomCombat.OnAnyRoomCompleted -= HandleRoomCompleted;
    }

    private void Start()
    {
        if (RoomCompleteText != null)
            RoomCompleteText.text = "";
    }

    private void HandleRoomCompleted()
    {
        if (RoomCompleteText == null)
            return;

        RoomCompleteText.text = "HABITACIÓN COMPLETADA";

        // Si ya había un temporizador corriendo (ej. completas otra sala rápido), lo reiniciamos
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        RoomCompleteText.text = "";
        hideRoutine = null;
    }
}