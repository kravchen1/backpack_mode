using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

public class TilemapCleaner : EditorWindow
{
    [MenuItem("Tools/Tilemap Cleaner")]
    public static void ShowWindow()
    {
        GetWindow<TilemapCleaner>("Tilemap Cleaner");
    }

    private Vector2 scrollPosition;
    private List<string> tilemapNames = new List<string>();
    private bool showConfirmation = false;

    private void OnEnable()
    {
        RefreshTilemapList();
    }

    private void OnGUI()
    {
        GUILayout.Label("Tilemap Cleaner", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("This tool will clear ALL tiles from ALL tilemaps in the scene.", MessageType.Warning);

        GUILayout.Space(10);

        // Обновление списка тайлмапов
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh Tilemap List"))
        {
            RefreshTilemapList();
        }

        if (GUILayout.Button("Clear All Tiles"))
        {
            showConfirmation = true;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Отображение списка тайлмапов
        if (tilemapNames.Count > 0)
        {
            GUILayout.Label($"Found {tilemapNames.Count} tilemaps in scene:", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));

            foreach (string tilemapName in tilemapNames)
            {
                EditorGUILayout.LabelField("• " + tilemapName);
            }

            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.HelpBox("No tilemaps found in the current scene.", MessageType.Info);
        }

        GUILayout.Space(20);

        // Кнопка очистки с подтверждением
        if (showConfirmation)
        {
            DisplayConfirmationDialog();
        }
        else
        {
            if (tilemapNames.Count > 0)
            {
                if (GUILayout.Button("Clear All Tiles", GUILayout.Height(30)))
                {
                    showConfirmation = true;
                }
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Button("Clear All Tiles", GUILayout.Height(30));
                EditorGUI.EndDisabledGroup();
            }
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Warning: This action cannot be undone! Make sure to save your scene before proceeding.", MessageType.Error);
    }

    private void DisplayConfirmationDialog()
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("ARE YOU SURE?", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This will permanently remove ALL tiles from ALL tilemaps in the scene. This action cannot be undone!", MessageType.Error);

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("YES, CLEAR ALL TILES", GUILayout.Height(30)))
        {
            if (ClearAllTiles())
            {
                EditorUtility.DisplayDialog("Success", "All tiles have been cleared from all tilemaps!", "OK");
                RefreshTilemapList();
            }
            showConfirmation = false;
        }

        if (GUILayout.Button("CANCEL", GUILayout.Height(30)))
        {
            showConfirmation = false;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void RefreshTilemapList()
    {
        tilemapNames.Clear();
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();

        foreach (Tilemap tilemap in allTilemaps)
        {
            if (tilemap != null)
            {
                tilemapNames.Add(tilemap.name + " (" + tilemap.gameObject.name + ")");
            }
        }
    }

    private bool ClearAllTiles()
    {
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();
        if (allTilemaps.Length == 0)
        {
            Debug.LogError("No tilemaps found in scene!");
            return false;
        }

        Debug.Log($"Found {allTilemaps.Length} tilemaps to clear");

        int totalCleared = 0;

        foreach (Tilemap tilemap in allTilemaps)
        {
            if (tilemap != null)
            {
                // Получаем границы перед очисткой для подсчета
                tilemap.CompressBounds();
                BoundsInt bounds = tilemap.cellBounds;

                int tileCount = 0;
                foreach (Vector3Int cellPos in bounds.allPositionsWithin)
                {
                    if (tilemap.HasTile(cellPos))
                    {
                        tileCount++;
                    }
                }

                // Очищаем все тайлы
                tilemap.ClearAllTiles();

                // Обновляем тайлмап
                tilemap.RefreshAllTiles();

                Debug.Log($"Cleared {tileCount} tiles from tilemap: {tilemap.name}");
                totalCleared += tileCount;
            }
        }

        Debug.Log($"Cleaning completed! Total tiles cleared: {totalCleared}");

        // Обновляем сцену
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        return true;
    }
}