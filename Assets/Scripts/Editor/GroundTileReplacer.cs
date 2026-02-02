using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.IO;

public class GroundTileReplacer : EditorWindow
{
    [MenuItem("Tools/Replace Ground Tiles")]
    public static void ShowWindow()
    {
        GetWindow<GroundTileReplacer>("Ground Tile Replacer");
    }

    // Публичные поля для настройки вероятностей
    public float highGrassChance = 0.3f; // 30% шанс высокой травы
    public float lowGrassChance = 0.4f;  // 40% шанс малой травы
    // Оставшиеся 30% - обычная земля

    // Ссылки на тайлы, которые можно перетащить в инспекторе
    public TileBase highGrassTile;
    public TileBase lowGrassTile;

    private void OnGUI()
    {
        GUILayout.Label("Ground Tile Replacement", EditorStyles.boldLabel);

        // Поля для настройки вероятностей
        highGrassChance = EditorGUILayout.Slider("High Grass Chance", highGrassChance, 0f, 1f);
        lowGrassChance = EditorGUILayout.Slider("Low Grass Chance", lowGrassChance, 0f, 1f);

        // Проверка, чтобы сумма вероятностей не превышала 1
        float totalChance = highGrassChance + lowGrassChance;
        if (totalChance > 1f)
        {
            EditorGUILayout.HelpBox($"Total chance exceeds 100%! Current: {totalChance * 100}%", MessageType.Warning);
        }

        GUILayout.Space(10);

        // Поля для перетаскивания тайлов
        highGrassTile = (TileBase)EditorGUILayout.ObjectField("High Grass Tile", highGrassTile, typeof(TileBase), false);
        lowGrassTile = (TileBase)EditorGUILayout.ObjectField("Low Grass Tile", lowGrassTile, typeof(TileBase), false);

        GUILayout.Space(20);

        // Кнопка для запуска замены
        if (GUILayout.Button("Replace Ground Tiles"))
        {
            if (highGrassTile == null || lowGrassTile == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign both grass tiles first!", "OK");
                return;
            }

            if (ReplaceGroundTiles())
            {
                EditorUtility.DisplayDialog("Success", "Ground tiles replaced successfully!", "OK");
            }
        }

        // Информационная панель
        GUILayout.Space(10);
        EditorGUILayout.HelpBox("This tool will find all 'земля' tiles and randomly replace them with grass variants.", MessageType.Info);
    }

    private bool ReplaceGroundTiles()
    {
        // Находим все тайлмапы в сцене
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();
        if (allTilemaps.Length == 0)
        {
            Debug.LogError("No tilemaps found in scene!");
            return false;
        }

        Debug.Log($"Found {allTilemaps.Length} tilemaps in scene");

        int totalReplaced = 0;
        int earthTilesFound = 0;

        // Проходим по всем тайлмапам
        foreach (Tilemap tilemap in allTilemaps)
        {
            if (!tilemap.gameObject.activeInHierarchy) continue;

            // Получаем границы тайлмапа
            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;

            int tilemapReplaced = 0;

            // Проходим по всем ячейкам в границах тайлмапа
            foreach (Vector3Int cellPos in bounds.allPositionsWithin)
            {
                if (tilemap.HasTile(cellPos))
                {
                    TileBase currentTile = tilemap.GetTile(cellPos);

                    // Проверяем, является ли тайл "землей"
                    if (IsEarthTileStrict(currentTile))
                    {
                        earthTilesFound++;

                        // Генерируем случайное значение для определения замены
                        float randomValue = Random.value;

                        // Определяем, какой тайл использовать на основе вероятностей
                        if (randomValue < highGrassChance)
                        {
                            // Заменяем на землю с высокой травой
                            tilemap.SetTile(cellPos, highGrassTile);
                            tilemapReplaced++;
                        }
                        else if (randomValue < highGrassChance + lowGrassChance)
                        {
                            // Заменяем на землю с малой травой
                            tilemap.SetTile(cellPos, lowGrassTile);
                            tilemapReplaced++;
                        }
                        // else - оставляем как есть (обычная земля)
                    }
                }
            }

            if (tilemapReplaced > 0)
            {
                Debug.Log($"Replaced {tilemapReplaced} tiles in tilemap: {tilemap.name}");
                totalReplaced += tilemapReplaced;

                // Обновляем тайлмап (важно для отображения изменений)
                tilemap.RefreshAllTiles();
            }
        }

        Debug.Log($"Process completed! Found {earthTilesFound} earth tiles, replaced {totalReplaced} tiles.");

        if (earthTilesFound == 0)
        {
            Debug.LogWarning("No 'земля' tiles found! Check your tile names.");
            return false;
        }

        return true;
    }

    // Альтернативная версия метода с более строгой проверкой
    private bool IsEarthTileStrict(TileBase tile)
    {
        if (tile == null) return false;

        string tileName = tile.name.ToLower();

        // Только точные совпадения и варианты с "земля"
        return tileName == "земля_0";
    }
}