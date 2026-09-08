using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private RectTransform container;
    [SerializeField] private GameObject roomIconPrefab;

    [Header("Colores")]
    [SerializeField] private Color visitedColor = Color.white;
    [SerializeField] private Color currentRoomColor = Color.yellow;

    [Header("Tamaño en el minimapa")]
    [SerializeField] private float iconSpacing = 24f;

    private readonly Dictionary<RoomNode, Image> iconsByNode = new();

    private void Start()
    {
        BuildIcons();
    }

    private void BuildIcons()
    {
        foreach (var kvp in mapGenerator.Rooms)
        {
            RoomNode node = kvp.Value;

            GameObject iconObj = Instantiate(roomIconPrefab, container);
            RectTransform rect = iconObj.GetComponent<RectTransform>();

            Vector2Int relativePos = node.GridPosition - mapGenerator.StartPos;
            rect.anchoredPosition = new Vector2(
                relativePos.x * iconSpacing,
                relativePos.y * iconSpacing
            );

            Image image = iconObj.GetComponent<Image>();
            iconsByNode[node] = image;

            iconObj.SetActive(false);
        }
    }

    private void Update()
    {
        foreach (var kvp in iconsByNode)
        {
            RoomNode node = kvp.Key;
            Image icon = kvp.Value;

            if (node.RoomInstance == null)
                continue;

            if (!node.RoomInstance.HasBeenVisited)
            {
                icon.gameObject.SetActive(false);
                continue;
            }

            icon.gameObject.SetActive(true);

            bool isCurrentRoom = RoomCombat.CurrentRoom == node.RoomInstance;
            icon.color = isCurrentRoom ? currentRoomColor : visitedColor;
        }
    }
}