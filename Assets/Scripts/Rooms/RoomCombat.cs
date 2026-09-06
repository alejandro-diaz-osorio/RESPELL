using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomCombat : MonoBehaviour
{
    public enum RoomState
    {
        Waiting,
        Combat,
        Completed
    }

    [Header("Doors")]
    [SerializeField] private List<Door> doors = new();

    [Header("Pickup de recompensa")]
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private List<ModifierData> possibleModifiers = new();

    [Header("Pickup de vida")]
    [SerializeField] private GameObject healthPickupPrefab;
    [SerializeField] private float healthPickupChance = 0.3f;
    private readonly float[] healPercentages = { 0.25f, 0.5f, 0.75f, 1f };

    private readonly List<EnemyHealth> enemies = new();
    private RoomState currentState = RoomState.Waiting;
    private bool playerInside;

    public static RoomCombat CurrentRoom { get; private set; }

    public static event Action OnAnyRoomCompleted;

    public int EnemiesRemaining => enemies.Count;
    public RoomState CurrentState => currentState;
    public bool IsPlayerInside => playerInside;

    private void Start()
    {
        EnemyHealth[] roomEnemies =
            GetComponentsInChildren<EnemyHealth>(true);

        foreach (EnemyHealth enemy in roomEnemies)
        {
            RegisterEnemy(enemy);
        }

        OpenDoors();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
        CurrentRoom = this;

        if (currentState == RoomState.Waiting)
        {
            StartCombat();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (CurrentRoom == this)
        {
            CurrentRoom = null;
        }
    }

    private void StartCombat()
    {
        if (EnemiesRemaining == 0)
        {
            CompleteRoom();
            return;
        }

        currentState = RoomState.Combat;
        CloseDoors();

        Debug.Log("¡Combate iniciado en la habitación: " + gameObject.name + "!");
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
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            enemy.OnEnemyDeath -= HandleEnemyDeath;
        }

        Debug.Log("Enemigo eliminado: " + enemy.gameObject.name + " | Restantes: " + EnemiesRemaining);

        if (currentState == RoomState.Combat && EnemiesRemaining == 0)
        {
            CompleteRoom();
        }
    }

    private void CompleteRoom()
    {
        currentState = RoomState.Completed;
        OpenDoors();

        Debug.Log("¡¡¡ HABITACIÓN COMPLETADA !!!");

        SpawnRewardPickup();

        OnAnyRoomCompleted?.Invoke();
    }
    private void CloseDoors()
    {
        foreach (Door door in doors)
        {
            if (door != null)
                door.Close();
        }
    }

    private void OpenDoors()
    {
        foreach (Door door in doors)
        {
            if (door != null)
                door.Open();
        }
    }
    private void SpawnRewardPickup()
    {
        if (pickupPrefab != null && possibleModifiers.Count > 0)
        {
            ModifierData randomModifier =
                possibleModifiers[UnityEngine.Random.Range(0, possibleModifiers.Count)];

            GameObject pickupInstance = Instantiate(
                pickupPrefab, transform.position, Quaternion.identity);

            ModifierPickup pickup = pickupInstance.GetComponent<ModifierPickup>();

            if (pickup != null)
            {
                pickup.SetModifier(randomModifier);
            }
        }

        TrySpawnHealthPickup();
    }

    private void TrySpawnHealthPickup()
    {
        if (healthPickupPrefab == null)
            return;

        if (UnityEngine.Random.value > healthPickupChance)
            return;

        float randomPercent =
            healPercentages[UnityEngine.Random.Range(0, healPercentages.Length)];

        Vector3 offset = new Vector3(1.5f, 0f, 0f);

        GameObject healthInstance = Instantiate(
            healthPickupPrefab, transform.position + offset, Quaternion.identity);

        HealthPickup healthPickup = healthInstance.GetComponent<HealthPickup>();

        if (healthPickup != null)
        {
            healthPickup.SetHealPercentage(randomPercent);
        }
    }

    private void OnDestroy()
    {
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null)
                enemy.OnEnemyDeath -= HandleEnemyDeath;
        }

        if (CurrentRoom == this)
        {
            CurrentRoom = null;
        }
    }
}