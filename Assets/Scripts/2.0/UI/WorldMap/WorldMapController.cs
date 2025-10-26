using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapRegion
{
    public string regionName;
    public Vector2[] polygonPoints; // Полигон региона в нормализованных координатах [0,1]
    public string description;
}

public class WorldMapController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject worldMapPanel;
    public RawImage mapImage;
    public RectTransform playerArrow;

    [Header("World Settings")]
    public Vector2 worldSize = new Vector2(1900, 900); // Общий размер
    public Vector2 worldMin = new Vector2(0, 0); // Минимальные координаты мира
    public Vector2 worldMax = new Vector2(1900, 900);   // Максимальные координаты мира
    public Transform playerTransform;

    [Header("Map Settings")]
    public Texture2D mapTexture;
    public float mapUpdateInterval = 0.1f; // Как часто обновлять позицию игрока


    [Header("Interaction")]
    public bool pauseGameWhenMapOpen = true;

    private List<MapRegion> regions = new List<MapRegion>();
    private Dictionary<string, GameObject> activeMarkers = new Dictionary<string, GameObject>();

    private RectTransform mapRectTransform;
    private float lastUpdateTime;

    private void Start()
    {
        mapRectTransform = mapImage.GetComponent<RectTransform>();
        LoadMapTexture();
        CloseMap(); // Сначала карта скрыта
    }



    private void Update()
    {
        // Открытие/закрытие карты по M
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }

        // Обновление позиции игрока на карте
        if (worldMapPanel.activeInHierarchy && Time.time - lastUpdateTime > mapUpdateInterval)
        {
            UpdatePlayerPosition();
            lastUpdateTime = Time.time;
        }
    }

    private void LoadMapTexture()
    {
        if (mapTexture == null)
        {
            // Пытаемся загрузить из Resources
            mapTexture = Resources.Load<Texture2D>("worldmap");
        }

        if (mapTexture != null)
        {
            mapImage.texture = mapTexture;
        }
        else
        {
            Debug.LogError("World map texture not found!");
        }
    }

    private void UpdatePlayerPosition()
    {
        if (playerTransform == null || mapRectTransform == null) return;

        // Используем локальные координаты вместо мировых
        Vector3 localPos = playerTransform.localPosition;

        float normalizedX = (localPos.x - worldMin.x) / (worldMax.x - worldMin.x);
        float normalizedY = (localPos.y - worldMin.y) / (worldMax.y - worldMin.y);

        Debug.Log($"=== LOCAL COORDINATES DEBUG ===");
        Debug.Log($"Local Position: ({localPos.x}, {localPos.y})");
        Debug.Log($"Normalized: ({normalizedX}, {normalizedY})");

        normalizedX = Mathf.Clamp01(normalizedX);
        normalizedY = Mathf.Clamp01(normalizedY);

        float mapWidth = mapRectTransform.rect.width;
        float mapHeight = mapRectTransform.rect.height;

        float uiX = (normalizedX - 0.5f) * mapWidth;
        float uiY = (normalizedY - 0.5f) * mapHeight;

        Vector2 playerUIPosition = new Vector2(uiX, uiY);
        playerArrow.anchoredPosition = playerUIPosition;

        // ЗАМЕНА: Поворачиваем стрелку по направлению движения вместо вращения трансформа
        UpdateArrowDirection();
    }

    private void UpdateArrowDirection()
    {
        TopDownCharacterController playerController = playerTransform.GetComponent<TopDownCharacterController>();
        if (playerController == null) return;

        // Получаем текущее направление движения
        TopDownCharacterController.MovementDirection movementDir = playerController.GetCurrentMovementDirection();

        // Устанавливаем угол поворота в зависимости от направления
        float arrowAngle = 0f;

        switch (movementDir)
        {
            case TopDownCharacterController.MovementDirection.Up:
                arrowAngle = -90f;    // Стрелка смотрит вверх
                break;
            case TopDownCharacterController.MovementDirection.Down:
                arrowAngle = 90f;  // Стрелка смотрит вниз
                break;
            case TopDownCharacterController.MovementDirection.Right:
                arrowAngle = 180f;  // Стрелка смотрит вправо
                break;
            case TopDownCharacterController.MovementDirection.Left:
                arrowAngle = 0f;   // Стрелка смотрит влево
                break;
        }

        playerArrow.localEulerAngles = new Vector3(0, 0, arrowAngle);
    }

    private void OpenMap()
    {
        worldMapPanel.SetActive(true);
        UpdatePlayerPosition();

        if (pauseGameWhenMapOpen)
        {
            Time.timeScale = 0;
        }
    }

    private void CloseMap()
    {
        worldMapPanel.SetActive(false);

        if (pauseGameWhenMapOpen)
        {
            Time.timeScale = 1;
        }
    }

    public void ToggleMap()
    {
        if (worldMapPanel.activeInHierarchy)
            CloseMap();
        else
            OpenMap();
    }
}