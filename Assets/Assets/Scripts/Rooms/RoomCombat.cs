using System.Collections.Generic;
using UnityEngine;

public class RoomCombat : MonoBehaviour
{
    private readonly List<EnemyHealth> enemies = new();

    public int EnemiesRemaining => enemies.Count;

    private void Start()
    {
        Debug.Log("========== ROOM COMBAT ==========");
        Debug.Log("RoomCombat está funcionando.");
        Debug.Log("Objeto: " + gameObject.name);

        EnemyHealth[] roomEnemies =
            GetComponentsInChildren<EnemyHealth>(true);

        Debug.Log(
            "EnemyHealth encontrados: " +
            roomEnemies.Length
        );

        foreach (EnemyHealth enemy in roomEnemies)
        {
            Debug.Log(
                "Enemigo encontrado: " +
                enemy.gameObject.name
            );

            RegisterEnemy(enemy);
        }

        Debug.Log(
            "Enemigos registrados: " +
            EnemiesRemaining
        );

        Debug.Log("=================================");
    }

    private void RegisterEnemy(EnemyHealth enemy)
    {
        if (enemy == null)
            return;

        if (enemies.Contains(enemy))
            return;

        enemies.Add(enemy);

        enemy.OnEnemyDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath(EnemyHealth enemy)
    {
        enemies.Remove(enemy);

        Debug.Log(
            "Enemigo eliminado: " +
            enemy.gameObject.name
        );

        Debug.Log(
            "Enemigos restantes: " +
            EnemiesRemaining
        );

        if (EnemiesRemaining == 0)
        {
            CompleteRoom();
        }
    }

    private void CompleteRoom()
    {
        Debug.Log("¡¡¡ HABITACIÓN COMPLETADA !!!");
    }

    private void OnDestroy()
    {
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDeath -= HandleEnemyDeath;
            }
        }
    }
}