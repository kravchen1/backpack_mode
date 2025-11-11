using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TilemapExporter : EditorWindow
{
    [MenuItem("Tools/Tilemap Exporter")]
    public static void ShowWindow()
    {
        GetWindow<TilemapExporter>("Tilemap Exporter");
    }

    private string fileName = "tilemap_export";
    private Vector2 scrollPosition;
    private bool includeInactiveTilemaps = false;
    private int chunkSize = 16;
    private bool exportByChunks = true;
    private bool useCompression = true;
    private CompressionMethod compressionMethod = CompressionMethod.RunLength;

    private enum CompressionMethod
    {
        RunLength,
        ChunkBased,
        Hybrid
    }

    private void OnGUI()
    {
        GUILayout.Label("Tilemap Exporter", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Exports all tilemaps data to files for later restoration", MessageType.Info);

        GUILayout.Space(10);

        // Настройки экспорта
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Export Settings", EditorStyles.boldLabel);

        fileName = EditorGUILayout.TextField("File Name", fileName);
        exportByChunks = EditorGUILayout.Toggle("Export by Chunks", exportByChunks);

        if (exportByChunks)
        {
            chunkSize = EditorGUILayout.IntField("Chunk Size", chunkSize);
            chunkSize = Mathf.Clamp(chunkSize, 4, 100);
        }

        includeInactiveTilemaps = EditorGUILayout.Toggle("Include Inactive Tilemaps", includeInactiveTilemaps);

        useCompression = EditorGUILayout.Toggle("Use Compression", useCompression);
        if (useCompression)
        {
            compressionMethod = (CompressionMethod)EditorGUILayout.EnumPopup("Compression Method", compressionMethod);
        }

        EditorGUILayout.EndVertical();

        GUILayout.Space(20);

        // Кнопка экспорта
        if (GUILayout.Button("Export Tilemaps", GUILayout.Height(30)))
        {
            ExportTilemaps();
        }

        GUILayout.Space(10);

        if (exportByChunks)
        {
            EditorGUILayout.HelpBox($"Will export tilemaps in chunks of {chunkSize}x{chunkSize} cells. Recommended for large worlds.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Will export all data in single file. Not recommended for large worlds.", MessageType.Warning);
        }

        if (useCompression)
        {
            EditorGUILayout.HelpBox($"Using {compressionMethod} compression to reduce file size.", MessageType.Info);
        }
    }

    private void ExportTilemaps()
    {
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>(includeInactiveTilemaps);
        if (allTilemaps.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No tilemaps found in scene!", "OK");
            return;
        }

        Debug.Log($"Found {allTilemaps.Length} tilemaps for export");

        string folderPath = Application.dataPath + "/ExportedTilemaps/" + fileName + "/";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        int totalFiles = 0;
        int totalTiles = 0;
        long originalSize = 0;
        long compressedSize = 0;

        foreach (Tilemap tilemap in allTilemaps)
        {
            if (!includeInactiveTilemaps && !tilemap.gameObject.activeInHierarchy)
                continue;

            Debug.Log($"Exporting tilemap: {tilemap.name}");

            if (exportByChunks)
            {
                var chunks = ExportTilemapByChunks(tilemap, folderPath);
                totalFiles += chunks.fileCount;
                totalTiles += chunks.tileCount;
                originalSize += chunks.originalSize;
                compressedSize += chunks.compressedSize;
            }
            else
            {
                var result = ExportTilemapSingle(tilemap, folderPath);
                totalFiles += result.fileCount;
                totalTiles += result.tileCount;
                originalSize += result.originalSize;
                compressedSize += result.compressedSize;
            }
        }

        AssetDatabase.Refresh();

        float compressionRatio = originalSize > 0 ? (float)compressedSize / originalSize : 0;
        EditorUtility.DisplayDialog("Success",
            $"Export completed!\n" +
            $"Total files: {totalFiles}\n" +
            $"Total tiles: {totalTiles}\n" +
            $"Compression: {compressionRatio:P2} of original\n" +
            $"Folder: Assets/ExportedTilemaps/{fileName}/", "OK");
    }

    private (int fileCount, int tileCount, long originalSize, long compressedSize) ExportTilemapByChunks(Tilemap tilemap, string folderPath)
    {
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        Debug.Log($"Tilemap '{tilemap.name}' bounds: {bounds}");

        int fileCount = 0;
        int totalTiles = 0;
        long totalOriginalSize = 0;
        long totalCompressedSize = 0;

        // Создаем метаданные для тайлмапы
        TilemapMetadata metadata = new TilemapMetadata();
        metadata.tilemapName = tilemap.name;
        metadata.position = tilemap.transform.position;
        metadata.rotation = tilemap.transform.rotation;
        metadata.scale = tilemap.transform.localScale;
        metadata.boundsMin = bounds.min;
        metadata.boundsMax = bounds.max;
        metadata.chunkSize = chunkSize;
        metadata.compressionMethod = useCompression ? compressionMethod.ToString() : "None";

        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        if (renderer != null)
        {
            metadata.sortingOrder = renderer.sortingOrder;
            metadata.sortingLayer = renderer.sortingLayerName;
        }

        // Разбиваем на чанки
        for (int x = bounds.xMin; x < bounds.xMax; x += chunkSize)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y += chunkSize)
            {
                List<TileInfo> tilesInChunk = new List<TileInfo>();

                // Собираем тайлы в чанке
                for (int cx = x; cx < x + chunkSize && cx < bounds.xMax; cx++)
                {
                    for (int cy = y; cy < y + chunkSize && cy < bounds.yMax; cy++)
                    {
                        Vector3Int cellPos = new Vector3Int(cx, cy, 0);
                        if (tilemap.HasTile(cellPos))
                        {
                            TileBase tile = tilemap.GetTile(cellPos);
                            if (tile != null)
                            {
                                TileInfo tileInfo = CreateTileInfo(tilemap, cellPos, tile);
                                tilesInChunk.Add(tileInfo);
                            }
                        }
                    }
                }

                if (tilesInChunk.Count > 0)
                {
                    ChunkData chunkData = new ChunkData();
                    chunkData.chunkX = x / chunkSize;
                    chunkData.chunkY = y / chunkSize;
                    chunkData.worldStartX = x;
                    chunkData.worldStartY = y;

                    if (useCompression)
                    {
                        chunkData.compressedTiles = CompressTiles(tilesInChunk, compressionMethod);
                        chunkData.useCompression = true;
                    }
                    else
                    {
                        chunkData.tiles = tilesInChunk;
                    }

                    // Сохраняем чанк
                    string chunkFileName = $"{tilemap.name}_chunk_{chunkData.chunkX}_{chunkData.chunkY}.json";
                    string chunkFilePath = folderPath + chunkFileName;

                    string jsonData = JsonUtility.ToJson(chunkData, true);
                    File.WriteAllText(chunkFilePath, jsonData);

                    // Считаем размеры
                    long originalSize = EstimateOriginalSize(tilesInChunk);
                    long compressedSize = new System.IO.FileInfo(chunkFilePath).Length;

                    totalOriginalSize += originalSize;
                    totalCompressedSize += compressedSize;
                    fileCount++;
                    totalTiles += tilesInChunk.Count;
                    metadata.chunkFiles.Add(chunkFileName);

                    Debug.Log($"Saved chunk {chunkData.chunkX},{chunkData.chunkY} with {tilesInChunk.Count} tiles " +
                             $"(compression: {(float)compressedSize / originalSize:P2})");
                }
            }
        }

        // Сохраняем метаданные
        if (fileCount > 0)
        {
            string metadataPath = folderPath + $"{tilemap.name}_metadata.json";
            string metadataJson = JsonUtility.ToJson(metadata, true);
            File.WriteAllText(metadataPath, metadataJson);
            fileCount++;
            totalCompressedSize += new System.IO.FileInfo(metadataPath).Length;
        }

        Debug.Log($"Exported {fileCount - 1} chunks from '{tilemap.name}' with {totalTiles} tiles");
        return (fileCount, totalTiles, totalOriginalSize, totalCompressedSize);
    }

    private (int fileCount, int tileCount, long originalSize, long compressedSize) ExportTilemapSingle(Tilemap tilemap, string folderPath)
    {
        TilemapData tilemapData = new TilemapData();
        tilemapData.tilemapName = tilemap.name;
        tilemapData.position = tilemap.transform.position;
        tilemapData.rotation = tilemap.transform.rotation;
        tilemapData.scale = tilemap.transform.localScale;

        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        if (renderer != null)
        {
            tilemapData.sortingOrder = renderer.sortingOrder;
            tilemapData.sortingLayer = renderer.sortingLayerName;
        }

        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        List<TileInfo> allTiles = new List<TileInfo>();

        foreach (Vector3Int cellPos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(cellPos))
            {
                TileBase tile = tilemap.GetTile(cellPos);
                if (tile != null)
                {
                    TileInfo tileInfo = CreateTileInfo(tilemap, cellPos, tile);
                    allTiles.Add(tileInfo);
                }
            }
        }

        if (useCompression)
        {
            tilemapData.compressedTiles = CompressTiles(allTiles, compressionMethod);
            tilemapData.useCompression = true;
        }
        else
        {
            tilemapData.tiles = allTiles;
        }

        string filePath = folderPath + $"{tilemap.name}.json";
        string jsonData = JsonUtility.ToJson(tilemapData, true);
        File.WriteAllText(filePath, jsonData);

        long originalSize = EstimateOriginalSize(allTiles);
        long compressedSize = new System.IO.FileInfo(filePath).Length;

        Debug.Log($"Exported {allTiles.Count} tiles from '{tilemap.name}' to single file " +
                 $"(compression: {(float)compressedSize / originalSize:P2})");
        return (1, allTiles.Count, originalSize, compressedSize);
    }

    private List<CompressedTileData> CompressTiles(List<TileInfo> tiles, CompressionMethod method)
    {
        switch (method)
        {
            case CompressionMethod.RunLength:
                return CompressRunLength(tiles);
            case CompressionMethod.ChunkBased:
                return CompressChunkBased(tiles);
            case CompressionMethod.Hybrid:
                return CompressHybrid(tiles);
            default:
                return CompressRunLength(tiles);
        }
    }

    private List<CompressedTileData> CompressRunLength(List<TileInfo> tiles)
    {
        List<CompressedTileData> compressed = new List<CompressedTileData>();
        if (tiles.Count == 0) return compressed;

        // Сортируем тайлы по позициям для лучшего сжатия
        var sortedTiles = tiles.OrderBy(t => t.position.y).ThenBy(t => t.position.x).ToList();

        CompressedTileData currentRun = null;

        foreach (var tile in sortedTiles)
        {
            if (currentRun == null)
            {
                currentRun = new CompressedTileData
                {
                    tileName = tile.tileName,
                    assetPath = tile.assetPath,
                    rotationZ = tile.rotationZ,
                    startPosition = new Vector2Int(tile.position.x, tile.position.y),
                    length = 1,
                    direction = 0 // horizontal
                };
            }
            else if (CanExtendRun(currentRun, tile, 0)) // horizontal
            {
                currentRun.length++;
            }
            else
            {
                compressed.Add(currentRun);
                currentRun = new CompressedTileData
                {
                    tileName = tile.tileName,
                    assetPath = tile.assetPath,
                    rotationZ = tile.rotationZ,
                    startPosition = new Vector2Int(tile.position.x, tile.position.y),
                    length = 1,
                    direction = 0
                };
            }
        }

        if (currentRun != null)
        {
            compressed.Add(currentRun);
        }

        Debug.Log($"RLE Compression: {tiles.Count} -> {compressed.Count} entries " +
                 $"({(float)compressed.Count / tiles.Count:P2})");
        return compressed;
    }

    private List<CompressedTileData> CompressChunkBased(List<TileInfo> tiles)
    {
        List<CompressedTileData> compressed = new List<CompressedTileData>();
        if (tiles.Count == 0) return compressed;

        // Группируем тайлы по типу и повороту
        var grouped = tiles.GroupBy(t => new { t.tileName, t.assetPath, t.rotationZ });

        foreach (var group in grouped)
        {
            var groupTiles = group.OrderBy(t => t.position.y).ThenBy(t => t.position.x).ToList();

            CompressedTileData currentChunk = null;
            const int maxChunkSize = 16; // Максимальный размер чанка

            foreach (var tile in groupTiles)
            {
                if (currentChunk == null)
                {
                    currentChunk = new CompressedTileData
                    {
                        tileName = group.Key.tileName,
                        assetPath = group.Key.assetPath,
                        rotationZ = group.Key.rotationZ,
                        startPosition = new Vector2Int(tile.position.x, tile.position.y),
                        length = 1,
                        direction = 0
                    };
                }
                else if (CanExtendRun(currentChunk, tile, maxChunkSize))
                {
                    currentChunk.length++;
                }
                else
                {
                    compressed.Add(currentChunk);
                    currentChunk = new CompressedTileData
                    {
                        tileName = group.Key.tileName,
                        assetPath = group.Key.assetPath,
                        rotationZ = group.Key.rotationZ,
                        startPosition = new Vector2Int(tile.position.x, tile.position.y),
                        length = 1,
                        direction = 0
                    };
                }
            }

            if (currentChunk != null)
            {
                compressed.Add(currentChunk);
            }
        }

        Debug.Log($"Chunk Compression: {tiles.Count} -> {compressed.Count} entries " +
                 $"({(float)compressed.Count / tiles.Count:P2})");
        return compressed;
    }

    private List<CompressedTileData> CompressHybrid(List<TileInfo> tiles)
    {
        // Пробуем оба метода и выбираем лучший
        var rleResult = CompressRunLength(tiles);
        var chunkResult = CompressChunkBased(tiles);

        return rleResult.Count < chunkResult.Count ? rleResult : chunkResult;
    }

    private bool CanExtendRun(CompressedTileData run, TileInfo nextTile, int maxLength = 0)
    {
        if (run.tileName != nextTile.tileName ||
            run.assetPath != nextTile.assetPath ||
            run.rotationZ != nextTile.rotationZ)
            return false;

        if (maxLength > 0 && run.length >= maxLength)
            return false;

        Vector2Int nextPos = new Vector2Int(nextTile.position.x, nextTile.position.y);
        Vector2Int expectedPos = run.startPosition + new Vector2Int(run.length, 0);

        return nextPos == expectedPos;
    }

    private long EstimateOriginalSize(List<TileInfo> tiles)
    {
        // Оцениваем исходный размер как если бы каждый тайл сохранялся отдельно
        return tiles.Count * 100; // Примерная оценка ~100 байт на тайл
    }

    private TileInfo CreateTileInfo(Tilemap tilemap, Vector3Int position, TileBase tile)
    {
        TileInfo tileInfo = new TileInfo();
        tileInfo.position = position;
        tileInfo.tileName = tile.name;
        tileInfo.tileType = tile.GetType().AssemblyQualifiedName;
        tileInfo.spriteName = GetSpriteNameFromTile(tile);
        tileInfo.assetPath = AssetDatabase.GetAssetPath(tile);

        // Получаем матрицу трансформации тайла
        Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(position);
        tileInfo.rotationZ = transformMatrix.rotation.eulerAngles.z;

        return tileInfo;
    }

    private string GetSpriteNameFromTile(TileBase tile)
    {
        if (tile == null) return "";

        System.Type tileType = tile.GetType();

        var spriteProperty = tileType.GetProperty("sprite");
        if (spriteProperty != null)
        {
            Sprite sprite = spriteProperty.GetValue(tile) as Sprite;
            if (sprite != null) return sprite.name;
        }

        var defaultSpriteField = tileType.GetField("m_DefaultSprite",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (defaultSpriteField != null)
        {
            Sprite defaultSprite = defaultSpriteField.GetValue(tile) as Sprite;
            if (defaultSprite != null) return defaultSprite.name;
        }

        var animatedSpritesField = tileType.GetField("m_AnimatedSprites",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (animatedSpritesField != null)
        {
            Sprite[] animatedSprites = animatedSpritesField.GetValue(tile) as Sprite[];
            if (animatedSprites != null && animatedSprites.Length > 0)
                return animatedSprites[0].name;
        }

        return "";
    }

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
        public int length; // количество подряд идущих тайлов
        public int direction; // 0 = horizontal, 1 = vertical
    }
}