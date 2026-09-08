using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomNode
{
    public Vector2Int GridPosition;
    public List<Direction> Connections = new();
    public RoomCombat RoomInstance;
}

public class MapGenerator : MonoBehaviour
{
    [Header("Configuración de la cuadrícula")]
    [SerializeField] private int gridSize = 5;
    [SerializeField] private int minRooms = 5;
    [SerializeField] private int maxRooms = 8;
    [SerializeField] private Vector2 cellSize = new Vector2(32f, 18f);

    [Header("Prefabs")]
    [SerializeField] private GameObject initialRoomPrefab;
    [SerializeField] private List<GameObject> roomPool = new();

    [Header("Jugador")]
    [SerializeField] private Transform playerTransform;

    private Dictionary<Vector2Int, RoomNode> rooms;
    private Vector2Int startPos;

    public IReadOnlyDictionary<Vector2Int, RoomNode> Rooms => rooms;
    public Vector2Int StartPos => startPos;
    public Vector2 CellSize => cellSize;

    private void Start()
    {
        GenerateLayout();
        InstantiateRooms();
        MovePlayerToStart();
    }

    private void GenerateLayout()
    {
        rooms = new Dictionary<Vector2Int, RoomNode>();

        int targetRoomCount = Random.Range(minRooms, maxRooms + 1);

        startPos = new Vector2Int(gridSize / 2, gridSize / 2);
        RoomNode startRoom = new RoomNode { GridPosition = startPos };
        rooms.Add(startPos, startRoom);

        Vector2Int currentPos = startPos;

        while (rooms.Count < targetRoomCount)
        {
            Direction randomDirection = GetRandomDirection();
            Vector2Int nextPos = currentPos + DirectionToOffset(randomDirection);

            if (!IsInsideGrid(nextPos))
            {
                continue;
            }

            if (!rooms.ContainsKey(nextPos))
            {
                RoomNode newRoom = new RoomNode { GridPosition = nextPos };
                rooms.Add(nextPos, newRoom);
            }

            RoomNode current = rooms[currentPos];
            RoomNode next = rooms[nextPos];

            if (!current.Connections.Contains(randomDirection))
            {
                current.Connections.Add(randomDirection);
            }

            Direction opposite = GetOppositeDirection(randomDirection);

            if (!next.Connections.Contains(opposite))
            {
                next.Connections.Add(opposite);
            }

            currentPos = nextPos;
        }
    }

    private void InstantiateRooms()
    {
        foreach (var kvp in rooms)
        {
            RoomNode node = kvp.Value;
            Vector3 worldPosition = GridToWorldPosition(node.GridPosition);

            GameObject prefabToSpawn = node.GridPosition == startPos
                ? initialRoomPrefab
                : FindMatchingPrefab(node.Connections);

            if (prefabToSpawn == null)
            {
                Debug.LogError(
                    $"No se encontró un prefab para la sala en {node.GridPosition} " +
                    $"con conexiones: {string.Join(", ", node.Connections)}"
                );
                continue;
            }

            GameObject instance = Instantiate(prefabToSpawn, worldPosition, Quaternion.identity);
            node.RoomInstance = instance.GetComponent<RoomCombat>();
        }
    }

    private void MovePlayerToStart()
    {
        if (playerTransform == null)
            return;

        Vector3 startWorldPosition = GridToWorldPosition(startPos);
        playerTransform.position = startWorldPosition;
    }

    private GameObject FindMatchingPrefab(List<Direction> requiredDirections)
    {
        HashSet<Direction> required = new HashSet<Direction>(requiredDirections);

        foreach (GameObject prefab in roomPool)
        {
            RoomCombat roomCombat = prefab.GetComponent<RoomCombat>();

            if (roomCombat == null)
                continue;

            HashSet<Direction> prefabDoors = new HashSet<Direction>(roomCombat.DoorDirections);

            if (prefabDoors.SetEquals(required))
            {
                return prefab;
            }
        }

        return null;
    }

    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        Vector2Int relativePos = gridPos - startPos;
        return new Vector3(relativePos.x * cellSize.x, relativePos.y * cellSize.y, 0f);
    }

    private Direction GetRandomDirection()
    {
        return (Direction)Random.Range(0, 4);
    }

    private Vector2Int DirectionToOffset(Direction direction)
    {
        return direction switch
        {
            Direction.North => new Vector2Int(0, 1),
            Direction.South => new Vector2Int(0, -1),
            Direction.East => new Vector2Int(1, 0),
            Direction.West => new Vector2Int(-1, 0),
            _ => Vector2Int.zero
        };
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => direction
        };
    }

    private bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize &&
               pos.y >= 0 && pos.y < gridSize;
    }
}