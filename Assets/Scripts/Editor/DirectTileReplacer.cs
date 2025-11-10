using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

public class DirectTileReplacer : EditorWindow
{
    [MenuItem("Tools/Direct Tile Replacer")]
    public static void ShowWindow()
    {
        GetWindow<DirectTileReplacer>("Direct Tile Replacer");
    }

    [System.Serializable]
    public class TileReplacement
    {
        public TileBase sourceTile;
        public TileBase targetTile;
    }

    public List<TileReplacement> tileReplacements = new List<TileReplacement>();

    private Vector2 scrollPosition;

    private void OnGUI()
    {
        GUILayout.Label("Direct Tile Replacement", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Configure which tiles should be replaced with which", MessageType.Info);

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
            tileReplacements.Add(new TileReplacement());
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
        EditorGUI.BeginDisabledGroup(tileReplacements.Count == 0);
        if (GUILayout.Button("Replace Tiles", GUILayout.Height(30)))
        {
            if (ValidateReplacements())
            {
                if (ReplaceTiles())
                {
                    EditorUtility.DisplayDialog("Success", "Tiles replaced successfully!", "OK");
                }
            }
        }
        EditorGUI.EndDisabledGroup();

        if (tileReplacements.Count == 0)
        {
            EditorGUILayout.HelpBox("Add at least one tile replacement to continue", MessageType.Warning);
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("This tool will replace all source tiles with target tiles in all tilemaps.", MessageType.Info);
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
                            tilemap.SetTile(cellPos, replacement.targetTile);
                            tilemapReplaced++;
                            break; // Прерываем после первой найденной замены
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
            Debug.LogWarning("No matching source tiles found!");
            return false;
        }

        return true;
    }

    private bool AreTilesEqual(TileBase tile1, TileBase tile2)
    {
        if (tile1 == null || tile2 == null) return false;

        // Сравниваем по имени и экземпляру
        return tile1 == tile2 || tile1.name == tile2.name;
    }
}