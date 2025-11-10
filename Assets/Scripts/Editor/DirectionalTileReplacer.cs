using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

public class DirectionalTileReplacer : EditorWindow
{
    [MenuItem("Tools/Directional Tile Replacer")]
    public static void ShowWindow()
    {
        GetWindow<DirectionalTileReplacer>("Directional Tile Replacer");
    }

    [System.Serializable]
    public class TileReplacement
    {
        public TileBase sourceTile;
        public TileBase targetTile;
        public TileBase conditionTile;
        public int checkRadius = 1;
        public bool includeDiagonals = true;
        public bool includeCardinalDirections = true;
        public bool requireUniqueCondition = true;
    }

    public List<TileReplacement> tileReplacements = new List<TileReplacement>();

    private Vector2 scrollPosition;

    private void OnGUI()
    {
        GUILayout.Label("Directional Tile Replacement", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Replace tiles and rotate based on ConditionTile position", MessageType.Info);

        GUILayout.Space(10);

        // Прокручиваемая область для списка замен
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Отображаем список замен
        for (int i = 0; i < tileReplacements.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"Replacement {i + 1}", EditorStyles.boldLabel);

            tileReplacements[i].sourceTile = (TileBase)EditorGUILayout.ObjectField(
                "Source Tile",
                tileReplacements[i].sourceTile,
                typeof(TileBase),
                false);

            tileReplacements[i].targetTile = (TileBase)EditorGUILayout.ObjectField(
                "Target Tile",
                tileReplacements[i].targetTile,
                typeof(TileBase),
                false);

            tileReplacements[i].conditionTile = (TileBase)EditorGUILayout.ObjectField(
                "Condition Tile",
                tileReplacements[i].conditionTile,
                typeof(TileBase),
                false);

            tileReplacements[i].checkRadius = EditorGUILayout.IntField("Check Radius", tileReplacements[i].checkRadius);
            tileReplacements[i].checkRadius = Mathf.Clamp(tileReplacements[i].checkRadius, 1, 3);

            // Настройки направлений проверки
            EditorGUILayout.LabelField("Check Directions:", EditorStyles.miniBoldLabel);

            tileReplacements[i].includeCardinalDirections = EditorGUILayout.Toggle("Cardinal Directions (↑↓←→)", tileReplacements[i].includeCardinalDirections);
            tileReplacements[i].includeDiagonals = EditorGUILayout.Toggle("Diagonal Directions (↖↗↙↘)", tileReplacements[i].includeDiagonals);

            // Новая опция - требовать уникальность condition tile
            tileReplacements[i].requireUniqueCondition = EditorGUILayout.Toggle("Require Unique Condition", tileReplacements[i].requireUniqueCondition);
            if (tileReplacements[i].requireUniqueCondition)
            {
                EditorGUILayout.HelpBox("Will only replace if exactly ONE condition tile is found nearby", MessageType.Info);
            }

            // Валидация - хотя бы одно направление должно быть включено
            if (!tileReplacements[i].includeCardinalDirections && !tileReplacements[i].includeDiagonals)
            {
                EditorGUILayout.HelpBox("At least one direction type must be enabled!", MessageType.Error);
            }

            // Кнопка удаления этой замены
            if (GUILayout.Button("Remove This Replacement"))
            {
                tileReplacements.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        // Кнопки управления списком
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add New Replacement"))
        {
            tileReplacements.Add(new TileReplacement()
            {
                checkRadius = 1,
                includeCardinalDirections = true,
                includeDiagonals = true,
                requireUniqueCondition = true
            });
        }

        if (GUILayout.Button("Clear All"))
        {
            if (EditorUtility.DisplayDialog("Clear All",
                "Are you sure you want to clear all replacements?", "Yes", "No"))
            {
                tileReplacements.Clear();
            }
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // Кнопка для запуска замены
        EditorGUI.BeginDisabledGroup(tileReplacements.Count == 0 || !IsAnyDirectionEnabled());
        if (GUILayout.Button("Replace Tiles", GUILayout.Height(30)))
        {
            if (ValidateReplacements())
            {
                if (ReplaceTiles())
                {
                    EditorUtility.DisplayDialog("Success", "Tiles replaced and rotated successfully!", "OK");
                }
            }
        }
        EditorGUI.EndDisabledGroup();

        if (tileReplacements.Count == 0)
        {
            EditorGUILayout.HelpBox("Add at least one tile replacement to continue", MessageType.Warning);
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Replaces source tiles with target tiles rotated based on ConditionTile position.", MessageType.Info);
        EditorGUILayout.HelpBox("Rotation rules:\n- Right/Right-Up: 90°\n- Up/Left-Up: 180°\n- Down/Left-Down: 270°\n- Left: 0°", MessageType.Info);
    }

    private bool IsAnyDirectionEnabled()
    {
        foreach (var replacement in tileReplacements)
        {
            if (replacement.includeCardinalDirections || replacement.includeDiagonals)
                return true;
        }
        return false;
    }

    private bool ValidateReplacements()
    {
        foreach (var replacement in tileReplacements)
        {
            if (replacement.sourceTile == null)
            {
                EditorUtility.DisplayDialog("Error", "One or more source tiles are not assigned!", "OK");
                return false;
            }

            if (replacement.targetTile == null)
            {
                EditorUtility.DisplayDialog("Error", "One or more target tiles are not assigned!", "OK");
                return false;
            }

            if (replacement.conditionTile == null)
            {
                EditorUtility.DisplayDialog("Error", "One or more condition tiles are not assigned!", "OK");
                return false;
            }

            if (!replacement.includeCardinalDirections && !replacement.includeDiagonals)
            {
                EditorUtility.DisplayDialog("Error", "At least one direction type must be enabled for all replacements!", "OK");
                return false;
            }
        }
        return true;
    }

    private bool ReplaceTiles()
    {
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();
        if (allTilemaps.Length == 0)
        {
            Debug.LogError("No tilemaps found in scene!");
            return false;
        }

        Debug.Log($"Found {allTilemaps.Length} tilemaps in scene");

        int totalReplaced = 0;

        foreach (Tilemap tilemap in allTilemaps)
        {
            if (!tilemap.gameObject.activeInHierarchy) continue;

            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;

            int tilemapReplaced = 0;

            foreach (Vector3Int cellPos in bounds.allPositionsWithin)
            {
                if (tilemap.HasTile(cellPos))
                {
                    TileBase currentTile = tilemap.GetTile(cellPos);

                    // Проверяем все возможные замены
                    foreach (var replacement in tileReplacements)
                    {
                        if (AreTilesEqual(currentTile, replacement.sourceTile))
                        {
                            // Находим все позиции condition tiles
                            List<Vector3Int> conditionPositions = FindAllConditionTilePositions(tilemap, cellPos, replacement);

                            // Проверяем условие уникальности
                            if (conditionPositions.Count > 0 && ShouldReplaceBasedOnUniqueness(conditionPositions.Count, replacement))
                            {
                                // Берем первую найденную позицию для определения поворота
                                Vector3Int conditionPos = conditionPositions[0];
                                float rotation = CalculateRotation(cellPos, conditionPos);
                                SetTileWithRotation(tilemap, cellPos, replacement.targetTile, rotation);
                                tilemapReplaced++;
                                break; // Прерываем после первой найденной замены
                            }
                        }
                    }
                }
            }

            if (tilemapReplaced > 0)
            {
                Debug.Log($"Replaced {tilemapReplaced} tiles in tilemap: {tilemap.name}");
                totalReplaced += tilemapReplaced;
                tilemap.RefreshAllTiles();
            }
        }

        Debug.Log($"Process completed! Total tiles replaced: {totalReplaced}");

        if (totalReplaced == 0)
        {
            Debug.LogWarning("No matching source tiles found with condition tiles nearby!");
            return false;
        }

        return true;
    }

    private List<Vector3Int> FindAllConditionTilePositions(Tilemap tilemap, Vector3Int centerPos, TileReplacement replacement)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        int radius = replacement.checkRadius;

        // Проверяем все ячейки в указанном радиусе
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                // Пропускаем центральную ячейку (исходную позицию)
                if (x == 0 && y == 0) continue;

                // Проверяем, нужно ли проверять эту позицию based on direction settings
                if (!ShouldCheckPosition(x, y, replacement))
                    continue;

                Vector3Int checkPos = new Vector3Int(centerPos.x + x, centerPos.y + y, centerPos.z);

                if (tilemap.HasTile(checkPos))
                {
                    TileBase neighborTile = tilemap.GetTile(checkPos);
                    if (AreTilesEqual(neighborTile, replacement.conditionTile))
                    {
                        positions.Add(checkPos);
                    }
                }
            }
        }

        return positions;
    }

    private bool ShouldReplaceBasedOnUniqueness(int conditionTileCount, TileReplacement replacement)
    {
        if (replacement.requireUniqueCondition)
        {
            // Заменяем только если найден ровно один condition tile
            return conditionTileCount == 1;
        }
        else
        {
            // Заменяем если найден хотя бы один condition tile
            return conditionTileCount >= 1;
        }
    }

    private float CalculateRotation(Vector3Int sourcePos, Vector3Int conditionPos)
    {
        int dx = conditionPos.x - sourcePos.x;
        int dy = conditionPos.y - sourcePos.y;

        // Определяем направление и возвращаем соответствующий угол
        if (dx > 0 && dy == 0) // Справа
            return 270f;
        else if (dx < 0 && dy == 0) // Слева
            return 90f;
        else if (dx == 0 && dy > 0) // Сверху
            return 0f;
        else if (dx == 0 && dy < 0) // Снизу
            return 180f;
        else if (dx > 0 && dy > 0) // Справа-вверху
            return 90f;
        else if (dx < 0 && dy > 0) // Слева-вверху
            return 180f;
        else if (dx < 0 && dy < 0) // Слева-внизу
            return 270f;
        else if (dx > 0 && dy < 0) // Справа-внизу
            return 0f; // Или 0, в зависимости от предпочтений

        return 0f; // По умолчанию
    }

    private void SetTileWithRotation(Tilemap tilemap, Vector3Int position, TileBase tile, float rotation)
    {
        // Создаем матрицу трансформации с поворотом
        Matrix4x4 matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.Euler(0f, 0f, rotation),
            Vector3.one
        );

        tilemap.SetTile(position, tile);
        tilemap.SetTransformMatrix(position, matrix);
    }

    private bool ShouldCheckPosition(int x, int y, TileReplacement replacement)
    {
        bool isCardinal = (x == 0 || y == 0); // Вертикальное или горизонтальное направление
        bool isDiagonal = (x != 0 && y != 0); // Диагональное направление

        if (isCardinal && replacement.includeCardinalDirections)
            return true;

        if (isDiagonal && replacement.includeDiagonals)
            return true;

        return false;
    }

    private bool AreTilesEqual(TileBase tile1, TileBase tile2)
    {
        if (tile1 == null || tile2 == null) return false;

        // Сравниваем по имени и экземпляру
        return tile1 == tile2 || tile1.name == tile2.name;
    }
}