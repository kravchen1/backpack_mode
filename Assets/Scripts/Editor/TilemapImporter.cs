using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TilemapImporter : EditorWindow
{
    [MenuItem("Tools/Tilemap Importer")]
    public static void ShowWindow()
    {
        GetWindow<TilemapImporter>("Tilemap Importer");
    }

    private string importFolder = "tilemap_export";
    private Vector2 scrollPosition;
    private bool clearExistingTiles = true;
    private bool createMissingTilemaps = true;
    private bool loadInBackground = true;
    private int chunksLoaded = 0;
    private int totalChunks = 0;

    private void OnGUI()
    {
        GUILayout.Label("Tilemap Importer", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Imports tilemap data from exported folder and restores tiles", MessageType.Info);

        GUILayout.Space(10);

        // Настройки импорта
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Import Settings", EditorStyles.boldLabel);

        importFolder = EditorGUILayout.TextField("Import Folder Name", importFolder);
        clearExistingTiles = EditorGUILayout.Toggle("Clear Existing Tiles", clearExistingTiles);
        createMissingTilemaps = EditorGUILayout.Toggle("Create Missing Tilemaps", createMissingTilemaps);
        loadInBackground = EditorGUILayout.Toggle("Load in Background", loadInBackground);

        EditorGUILayout.EndVertical();

        GUILayout.Space(20);

        // Кнопка импорта
        if (GUILayout.Button("Import Tilemaps", GUILayout.Height(30)))
        {
            EditorApplication.delayCall += () => ImportTilemaps();
        }

        if (totalChunks > 0)
        {
            GUILayout.Space(10);
            float progress = (float)chunksLoaded / totalChunks;
            EditorGUI.ProgressBar(GUILayoutUtility.GetRect(300, 20), progress,
                $"Loading chunks: {chunksLoaded}/{totalChunks}");

            if (chunksLoaded == totalChunks && totalChunks > 0)
            {
                EditorGUILayout.HelpBox("Import completed!", MessageType.Info);
            }
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox($"Folder: Assets/ExportedTilemaps/{importFolder}/", MessageType.Info);
    }

    private void ImportTilemaps()
    {
        string folderPath = Application.dataPath + "/ExportedTilemaps/" + importFolder + "/";
        if (!Directory.Exists(folderPath))
        {
            EditorUtility.DisplayDialog("Error", $"Folder not found: {folderPath}", "OK");
            return;
        }

        try
        {
            chunksLoaded = 0;
            totalChunks = 0;

            // Сначала находим все файлы метаданных
            string[] metadataFiles = Directory.GetFiles(folderPath, "*_metadata.json");
            if (metadataFiles.Length == 0)
            {
                // Пробуем найти обычные файлы (старый формат)
                string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
                if (jsonFiles.Length == 0)
                {
                    EditorUtility.DisplayDialog("Error", "No export files found in folder!", "OK");
                    return;
                }
                ImportLegacyFormat(jsonFiles);
                return;
            }

            // Очищаем существующие тайлы если нужно
            if (clearExistingTiles)
            {
                ClearAllTilemaps();
            }

            // Считаем общее количество чанков
            foreach (string metadataFile in metadataFiles)
            {
                string metadataJson = File.ReadAllText(metadataFile);
                TilemapMetadata metadata = JsonUtility.FromJson<TilemapMetadata>(metadataJson);
                totalChunks += metadata.chunkFiles.Count;
            }

            Debug.Log($"Found {metadataFiles.Length} tilemaps with {totalChunks} total chunks");

            // Загружаем каждый тайлмап
            foreach (string metadataFile in metadataFiles)
            {
                string metadataJson = File.ReadAllText(metadataFile);
                TilemapMetadata metadata = JsonUtility.FromJson<TilemapMetadata>(metadataJson);

                if (loadInBackground)
                {
                    EditorApplication.delayCall += () => LoadTilemapChunks(metadata, folderPath);
                }
                else
                {
                    LoadTilemapChunks(metadata, folderPath);
                }
            }

            if (!loadInBackground)
            {
                EditorUtility.DisplayDialog("Success", "Tilemaps imported successfully!", "OK");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Import failed: {e.Message}\n{e.StackTrace}");
            EditorUtility.DisplayDialog("Error", $"Import failed: {e.Message}", "OK");
        }
    }

    private void LoadTilemapChunks(TilemapMetadata metadata, string folderPath)
    {
        Tilemap tilemap = FindOrCreateTilemap(metadata.tilemapName);
        if (tilemap == null) return;

        // Восстанавливаем трансформ тайлмапы
        tilemap.transform.position = metadata.position;
        tilemap.transform.rotation = metadata.rotation;
        tilemap.transform.localScale = metadata.scale;

        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = metadata.sortingOrder;
            if (!string.IsNullOrEmpty(metadata.sortingLayer))
            {
                renderer.sortingLayerName = metadata.sortingLayer;
            }
        }

        Dictionary<string, TileBase> tileCache = new Dictionary<string, TileBase>();
        int tilesLoaded = 0;

        // Загружаем чанки
        foreach (string chunkFile in metadata.chunkFiles)
        {
            string chunkPath = folderPath + chunkFile;
            if (File.Exists(chunkPath))
            {
                string chunkJson = File.ReadAllText(chunkPath);
                ChunkData chunkData = JsonUtility.FromJson<ChunkData>(chunkJson);

                if (chunkData.useCompression && chunkData.compressedTiles != null)
                {
                    tilesLoaded += LoadCompressedTiles(tilemap, chunkData.compressedTiles, tileCache);
                }
                else
                {
                    tilesLoaded += LoadUncompressedTiles(tilemap, chunkData.tiles, tileCache);
                }

                chunksLoaded++;
                Debug.Log($"Loaded chunk {chunkData.chunkX},{chunkData.chunkY}");

                // Обновляем прогресс
                if (chunksLoaded % 10 == 0 || chunksLoaded == totalChunks)
                {
                    Repaint();
                }
            }
        }

        if (tilesLoaded > 0)
        {
            tilemap.RefreshAllTiles();
            Debug.Log($"Imported {tilesLoaded} tiles to '{metadata.tilemapName}'");
        }
    }

    private int LoadCompressedTiles(Tilemap tilemap, List<CompressedTileData> compressedTiles, Dictionary<string, TileBase> tileCache)
    {
        int tilesLoaded = 0;

        foreach (var compressed in compressedTiles)
        {
            TileBase tile = FindTile(compressed.tileName, compressed.assetPath, tileCache);
            if (tile != null)
            {
                for (int i = 0; i < compressed.length; i++)
                {
                    Vector3Int position;
                    if (compressed.direction == 0) // horizontal
                    {
                        position = new Vector3Int(
                            compressed.startPosition.x + i,
                            compressed.startPosition.y,
                            0
                        );
                    }
                    else // vertical
                    {
                        position = new Vector3Int(
                            compressed.startPosition.x,
                            compressed.startPosition.y + i,
                            0
                        );
                    }

                    tilemap.SetTile(position, tile);

                    // Восстанавливаем поворот тайла
                    if (compressed.rotationZ != 0)
                    {
                        Matrix4x4 transformMatrix = Matrix4x4.TRS(
                            Vector3.zero,
                            Quaternion.Euler(0, 0, compressed.rotationZ),
                            Vector3.one
                        );
                        tilemap.SetTransformMatrix(position, transformMatrix);
                    }

                    tilesLoaded++;
                }
            }
        }

        return tilesLoaded;
    }

    private int LoadUncompressedTiles(Tilemap tilemap, List<TileInfo> tiles, Dictionary<string, TileBase> tileCache)
    {
        int tilesLoaded = 0;

        foreach (TileInfo tileInfo in tiles)
        {
            TileBase tile = FindTile(tileInfo.tileName, tileInfo.assetPath, tileCache);
            if (tile != null)
            {
                tilemap.SetTile(tileInfo.position, tile);

                // Восстанавливаем поворот тайла
                if (tileInfo.rotationZ != 0)
                {
                    Matrix4x4 transformMatrix = Matrix4x4.TRS(
                        Vector3.zero,
                        Quaternion.Euler(0, 0, tileInfo.rotationZ),
                        Vector3.one
                    );
                    tilemap.SetTransformMatrix(tileInfo.position, transformMatrix);
                }

                tilesLoaded++;
            }
        }

        return tilesLoaded;
    }

    private void ImportLegacyFormat(string[] jsonFiles)
    {
        Debug.Log("Importing legacy format (single files)");

        if (clearExistingTiles)
        {
            ClearAllTilemaps();
        }

        Dictionary<string, TileBase> tileCache = new Dictionary<string, TileBase>();
        int totalTiles = 0;

        foreach (string jsonFile in jsonFiles)
        {
            string jsonData = File.ReadAllText(jsonFile);

            // Пробуем десериализовать как новый формат
            TilemapData tilemapData = JsonUtility.FromJson<TilemapData>(jsonData);
            if (tilemapData == null) continue;

            Tilemap tilemap = FindOrCreateTilemap(tilemapData.tilemapName);
            if (tilemap == null) continue;

            // Восстанавливаем трансформ
            tilemap.transform.position = tilemapData.position;
            tilemap.transform.rotation = tilemapData.rotation;
            tilemap.transform.localScale = tilemapData.scale;

            TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = tilemapData.sortingOrder;
                if (!string.IsNullOrEmpty(tilemapData.sortingLayer))
                {
                    renderer.sortingLayerName = tilemapData.sortingLayer;
                }
            }

            int tilesInMap = 0;
            if (tilemapData.useCompression && tilemapData.compressedTiles != null)
            {
                tilesInMap = LoadCompressedTiles(tilemap, tilemapData.compressedTiles, tileCache);
            }
            else
            {
                tilesInMap = LoadUncompressedTiles(tilemap, tilemapData.tiles, tileCache);
            }

            if (tilesInMap > 0)
            {
                tilemap.RefreshAllTiles();
                totalTiles += tilesInMap;
                Debug.Log($"Imported {tilesInMap} tiles to '{tilemapData.tilemapName}'");
            }
        }

        EditorUtility.DisplayDialog("Success", $"Imported {totalTiles} tiles from {jsonFiles.Length} files!", "OK");
    }

    private Tilemap FindOrCreateTilemap(string tilemapName)
    {
        Tilemap[] existingTilemaps = FindObjectsOfType<Tilemap>();
        Tilemap tilemap = existingTilemaps.FirstOrDefault(tm => tm.name == tilemapName);

        if (tilemap == null && createMissingTilemaps)
        {
            GameObject tilemapObject = new GameObject(tilemapName);
            tilemap = tilemapObject.AddComponent<Tilemap>();
            tilemapObject.AddComponent<TilemapRenderer>();
            Debug.Log($"Created new tilemap: {tilemapName}");
        }
        else if (tilemap == null)
        {
            Debug.LogWarning($"Tilemap '{tilemapName}' not found and creation is disabled");
        }

        return tilemap;
    }

    private TileBase FindTile(string tileName, string assetPath, Dictionary<string, TileBase> cache)
    {
        if (string.IsNullOrEmpty(tileName)) return null;

        string cacheKey = tileName + "_" + assetPath;
        if (cache.ContainsKey(cacheKey))
        {
            return cache[cacheKey];
        }

        TileBase tile = null;

        if (!string.IsNullOrEmpty(assetPath))
        {
            tile = AssetDatabase.LoadAssetAtPath<TileBase>(assetPath);
            if (tile != null)
            {
                cache[cacheKey] = tile;
                return tile;
            }
        }

        string[] guids = AssetDatabase.FindAssets($"{tileName} t:TileBase");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            tile = AssetDatabase.LoadAssetAtPath<TileBase>(path);
            if (tile != null && tile.name == tileName)
            {
                cache[cacheKey] = tile;
                return tile;
            }
        }

        Debug.LogWarning($"Tile not found: {tileName}");
        cache[cacheKey] = null;
        return null;
    }

    private void ClearAllTilemaps()
    {
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();
        foreach (Tilemap tilemap in allTilemaps)
        {
            tilemap.ClearAllTiles();
        }
        Debug.Log("Cleared all existing tiles from tilemaps");
    }

    // Классы данных (должны совпадать с экспортером)
    [System.Serializable]
    public class TilemapMetadata
    {
        public string tilemapName;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public int sortingOrder;
        public string sortingLayer;
        public Vector3Int boundsMin;
        public Vector3Int boundsMax;
        public int chunkSize;
        public string compressionMethod;
        public List<string> chunkFiles = new List<string>();
    }

    [System.Serializable]
    public class ChunkData
    {
        public int chunkX;
        public int chunkY;
        public int worldStartX;
        public int worldStartY;
        public bool useCompression = false;
        public List<TileInfo> tiles = new List<TileInfo>();
        public List<CompressedTileData> compressedTiles = new List<CompressedTileData>();
    }

    [System.Serializable]
    public class TilemapData
    {
        public string tilemapName;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public int sortingOrder;
        public string sortingLayer;
        public bool useCompression = false;
        public List<TileInfo> tiles = new List<TileInfo>();
        public List<CompressedTileData> compressedTiles = new List<CompressedTileData>();
    }

    [System.Serializable]
    public class TileInfo
    {
        public Vector3Int position;
        public string tileName;
        public string spriteName;
        public string tileType;
        public string assetPath;
        public float rotationZ;
    }

    [System.Serializable]
    public class CompressedTileData
    {
        public string tileName;
        public string assetPath;
        public float rotationZ;
        public Vector2Int startPosition;
        public int length;
        public int direction;
    }
}