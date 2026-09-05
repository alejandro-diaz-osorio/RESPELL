using UnityEngine;
using TMPro;

public class EnemyCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text enemyCounterText;

    private void Start()
    {
        UpdateEnemyCounter();
    }

    private void Update()
    {
        UpdateEnemyCounter();
    }

    private void UpdateEnemyCounter()
    {
        if (enemyCounterText == null)
            return;

        RoomCombat currentRoom = RoomCombat.CurrentRoom;

        if (currentRoom == null)
        {
            enemyCounterText.text = "";
            return;
        }

        enemyCounterText.text =
            $"Enemigos: {currentRoom.EnemiesRemaining}";
    }
}