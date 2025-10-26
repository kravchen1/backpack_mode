using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.IO;

public class MapGenerator : EditorWindow
{
    [MenuItem("Tools/Generate World Map")]
    public static void ShowWindow()
    {
        GetWindow<MapGenerator>("World Map Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("World Map Generation", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Map Texture"))
        {
            GenerateMapTexture();
        }
    }
    //FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
    private void GenerateMapTexture()
    {
        // 1. Находим все тайлмапы в сцене
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();
        if (allTilemaps.Length == 0)
        {
            Debug.LogError("No tilemaps found in scene!");
            return;
        }

        Debug.Log($"Found {allTilemaps.Length} tilemaps in scene");

        // 2. Определяем границы мира
        Bounds worldBounds = CalculateWorldBounds(allTilemaps);
        int width = Mathf.CeilToInt(worldBounds.size.x);
        int height = Mathf.CeilToInt(worldBounds.size.y);

        Debug.Log($"World bounds: {worldBounds.min} to {worldBounds.max}");
        Debug.Log($"World size: {width}x{height} tiles, Total tiles: {width * height}");

        // 3. Создаем текстуру
        Texture2D mapTexture = new Texture2D(width, height);
        mapTexture.filterMode = FilterMode.Point;

        // 4. Заполняем текстуру прозрачным цветом
        Color[] transparentPixels = new Color[width * height];
        for (int i = 0; i < transparentPixels.Length; i++)
            transparentPixels[i] = Color.clear;

        mapTexture.SetPixels(transparentPixels);

        // 5. Проходим по всем тайлам и устанавливаем цвета
        int tilesProcessed = 0;

        // ДЕБАГ: Проверим несколько позиций
        Debug.Log("Checking tile positions...");
        for (int i = 0; i < Mathf.Min(10, allTilemaps.Length); i++)
        {
            Tilemap tm = allTilemaps[i];
            tm.CompressBounds();
            Debug.Log($"Tilemap {i}: {tm.name} at {tm.transform.position}, bounds: {tm.localBounds}");
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Используем точные мировые координаты
                Vector3 worldPosition = new Vector3(
                    worldBounds.min.x + x + 0.5f, // +0.5f чтобы попасть в центр тайла
                    worldBounds.min.y + y + 0.5f,
                    0
                );

                bool tileFound = false;

                // Ищем тайл во всех тайлмапах
                foreach (Tilemap tilemap in allTilemaps)
                {
                    // Преобразуем мировую позицию в позицию ячейки тайлмапа
                    Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

                    if (tilemap.HasTile(cellPosition))
                    {
                        TileBase tile = tilemap.GetTile(cellPosition);
                        Color tileColor = GetColorForTile(tile);
                        mapTexture.SetPixel(x, y, tileColor);
                        tilesProcessed++;
                        tileFound = true;
                        break;
                    }
                }

                if (!tileFound)
                {
                    mapTexture.SetPixel(x, y, Color.clear);
                }
            }

            // Прогресс для больших миров
            if (width > 100 && x % 100 == 0)
            {
                float progress = (float)x / width;
                EditorUtility.DisplayProgressBar("Generating World Map",
                    $"Processing tiles... {tilesProcessed} tiles found", progress);
            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log($"Total tiles processed: {tilesProcessed}");

        // Если все еще 0 тайлов, попробуем альтернативный метод
        if (tilesProcessed == 0)
        {
            Debug.LogWarning("No tiles found with standard method. Trying alternative approach...");
            tilesProcessed = GenerateMapAlternative(allTilemaps, mapTexture, worldBounds);
        }

        // 6. Сохраняем текстуру
        mapTexture.Apply();
        SaveTextureAsPNG(mapTexture, "worldmap");

        Debug.Log($"World map generated successfully! Tiles found: {tilesProcessed}");
    }

    // Альтернативный метод поиска тайлов
    private int GenerateMapAlternative(Tilemap[] tilemaps, Texture2D texture, Bounds worldBounds)
    {
        int tilesProcessed = 0;
        int width = texture.width;
        int height = texture.height;

        // Проходим по каждому тайлмапу и его тайлам
        foreach (Tilemap tilemap in tilemaps)
        {
            if (!tilemap.gameObject.activeInHierarchy) continue;

            // Получаем все позиции с тайлами в этом тайлмапе
            BoundsInt bounds = tilemap.cellBounds;

            foreach (Vector3Int cellPos in bounds.allPositionsWithin)
            {
                if (tilemap.HasTile(cellPos))
                {
                    // Преобразуем позицию ячейки в мировую позицию
                    Vector3 worldPos = tilemap.CellToWorld(cellPos);

                    // Проверяем, находится ли позиция в пределах наших границ
                    if (worldBounds.Contains(worldPos))
                    {
                        // Конвертируем мировую позицию в координаты текстуры
                        int texX = Mathf.FloorToInt(worldPos.x - worldBounds.min.x);
                        int texY = Mathf.FloorToInt(worldPos.y - worldBounds.min.y);

                        if (texX >= 0 && texX < width && texY >= 0 && texY < height)
                        {
                            TileBase tile = tilemap.GetTile(cellPos);
                            Color tileColor = GetColorForTile(tile);
                            texture.SetPixel(texX, texY, tileColor);
                            tilesProcessed++;
                        }
                    }
                }
            }
        }

        return tilesProcessed;
    }

    private Bounds CalculateWorldBounds(Tilemap[] tilemaps)
    {
        if (tilemaps == null || tilemaps.Length == 0)
            return new Bounds(Vector3.zero, Vector3.one);

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        bool foundAnyTile = false;

        foreach (Tilemap tilemap in tilemaps)
        {
            if (!tilemap.gameObject.activeInHierarchy) continue;

            // Получаем все границы тайлов в этом тайлмапе
            tilemap.CompressBounds();
            Bounds localBounds = tilemap.localBounds;

            if (localBounds.size.magnitude > 0)
            {
                foundAnyTile = true;

                // Преобразуем локальные границы в мировые
                Vector3 worldMin = tilemap.transform.TransformPoint(localBounds.min);
                Vector3 worldMax = tilemap.transform.TransformPoint(localBounds.max);

                minX = Mathf.Min(minX, worldMin.x);
                minY = Mathf.Min(minY, worldMin.y);
                maxX = Mathf.Max(maxX, worldMax.x);
                maxY = Mathf.Max(maxY, worldMax.y);
            }
        }

        if (!foundAnyTile)
        {
            Debug.LogWarning("No active tiles found in any tilemap!");
            // Возвращаем дефолтные границы основываясь на позициях тайлмапов
            return CalculateBoundsFromTilemapPositions(tilemaps);
        }

        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);

        Debug.Log($"Calculated world bounds: Min({minX}, {minY}) Max({maxX}, {maxY}) Size({size.x}, {size.y})");

        return new Bounds(center, size);
    }

    private Bounds CalculateBoundsFromTilemapPositions(Tilemap[] tilemaps)
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (Tilemap tilemap in tilemaps)
        {
            Vector3 pos = tilemap.transform.position;
            minX = Mathf.Min(minX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxX = Mathf.Max(maxX, pos.x + 100); // предполагаем размер чанка 100
            maxY = Mathf.Max(maxY, pos.y + 100);
        }

        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);

        return new Bounds(center, size);
    }

    private Color GetColorForTile(TileBase tile)
    {
        if (tile == null) return Color.clear;

        string tileName = tile.name.ToLower();

        // Приоритет по списку с русскими названиями
        if (tileName.Contains("камен")) return new Color(0.4f, 0.4f, 0.4f); // Темно-серый для камня
        else if (tileName.Contains("боло")) return new Color(0.2f, 0.3f, 0.1f); // Болотный зеленый
        else if (tileName.Contains("вод")) return new Color(0.1f, 0.3f, 0.8f); // Синий для воды
        else if (tileName.Contains("газон")) return new Color(0.3f, 0.7f, 0.2f); // Ярко-зеленый для газона
        else if (tileName.Contains("грязь")) return new Color(0.4f, 0.3f, 0.2f); // Коричневатый для грязи
        else if (tileName.Contains("деревянный пол")) return new Color(0.7f, 0.5f, 0.3f); // Светло-коричневый для деревянного пола
        else if (tileName.Contains("кирпичный пол")) return new Color(0.6f, 0.4f, 0.3f); // Терракотовый для кирпичного пола
        else if (tileName.Contains("земл")) return new Color(0.5f, 0.4f, 0.2f); // Земляной коричневый
        else if (tileName.Contains("белый кирпич")) return new Color(0.9f, 0.9f, 0.9f); // Почти белый
        else if (tileName.Contains("красный кирпич")) return new Color(0.7f, 0.2f, 0.1f); // Красный для кирпича
        else if (tileName.Contains("деревянные")) return new Color(0.6f, 0.4f, 0.2f); // Коричневый для деревянной стены
        else return new Color(0.5f, 0.5f, 0.5f); // Серый по умолчанию (лучше чем белый)
    }

    private void SaveTextureAsPNG(Texture2D texture, string filename)
    {
        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, "Resources", filename + ".png");

        // Создаем папку Resources если ее нет
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        Debug.Log($"Map saved to: {path}");
    }
}