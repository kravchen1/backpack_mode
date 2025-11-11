using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class TilemapAnalyzer : EditorWindow
{
    [MenuItem("Tools/Tilemap Analyzer")]
    public static void ShowWindow()
    {
        GetWindow<TilemapAnalyzer>("Tilemap Analyzer");
    }

    private string analyzeFolder = "tilemap_export";
    private Vector2 scrollPosition;
    private string analysisResult = "";

    private void OnGUI()
    {
        GUILayout.Label("Tilemap Analyzer", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Analyzes exported tilemap data and generates a comprehensive report", MessageType.Info);

        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Analysis Settings", EditorStyles.boldLabel);
        analyzeFolder = EditorGUILayout.TextField("Analysis Folder Name", analyzeFolder);
        EditorGUILayout.EndVertical();

        GUILayout.Space(20);

        if (GUILayout.Button("Analyze Tilemap Data", GUILayout.Height(30)))
        {
            AnalyzeTilemaps();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Copy to Clipboard", GUILayout.Height(25)))
        {
            EditorGUIUtility.systemCopyBuffer = analysisResult;
            EditorUtility.DisplayDialog("Success", "Analysis result copied to clipboard!", "OK");
        }

        if (GUILayout.Button("Save to File", GUILayout.Height(25)))
        {
            SaveAnalysisToFile();
        }

        GUILayout.Space(10);

        if (!string.IsNullOrEmpty(analysisResult))
        {
            GUILayout.Label("Analysis Result:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
            EditorGUILayout.TextArea(analysisResult, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.HelpBox("Folder: Assets/ExportedTilemaps/" + analyzeFolder + "/", MessageType.Info);
    }

    private void AnalyzeTilemaps()
    {
        string folderPath = Application.dataPath + "/ExportedTilemaps/" + analyzeFolder + "/";
        if (!Directory.Exists(folderPath))
        {
            EditorUtility.DisplayDialog("Error", "Folder not found: " + folderPath, "OK");
            return;
        }

        StringBuilder report = new StringBuilder();
        report.AppendLine("=== TILEMAP EXPORT ANALYSIS REPORT ===");
        report.AppendLine("Generated: " + System.DateTime.Now);
        report.AppendLine("Folder: " + folderPath);
        report.AppendLine();

        try
        {
            // Собираем статистику по файлам
            string[] allFiles = Directory.GetFiles(folderPath, "*.json");
            report.AppendLine("Total JSON files: " + allFiles.Length);
            report.AppendLine();

            // Анализируем метаданные
            string[] metadataFiles = Directory.GetFiles(folderPath, "*_metadata.json");
            report.AppendLine("=== METADATA ANALYSIS ===");
            report.AppendLine("Metadata files found: " + metadataFiles.Length);
            report.AppendLine();

            List<TilemapAnalysis> tilemapAnalyses = new List<TilemapAnalysis>();

            foreach (string metadataFile in metadataFiles)
            {
                string metadataJson = File.ReadAllText(metadataFile);
                TilemapMetadata metadata = JsonUtility.FromJson<TilemapMetadata>(metadataJson);

                TilemapAnalysis analysis = new TilemapAnalysis();
                analysis.metadata = metadata;
                analysis.metadataFile = Path.GetFileName(metadataFile);

                // Анализируем чанки для этого тайлмапа
                foreach (string chunkFile in metadata.chunkFiles)
                {
                    string chunkPath = folderPath + chunkFile;
                    if (File.Exists(chunkPath))
                    {
                        string chunkJson = File.ReadAllText(chunkPath);
                        ChunkData chunkData = JsonUtility.FromJson<ChunkData>(chunkJson);
                        analysis.chunks.Add(chunkData);
                        analysis.totalChunks++;
                    }
                }

                tilemapAnalyses.Add(analysis);
            }

            // Генерируем детальный отчет
            GenerateDetailedReport(report, tilemapAnalyses, folderPath);

            analysisResult = report.ToString();
            Repaint();
        }
        catch (System.Exception e)
        {
            analysisResult = "Analysis failed: " + e.Message + "\n" + e.StackTrace;
            Repaint();
        }
    }

    private void GenerateDetailedReport(StringBuilder report, List<TilemapAnalysis> analyses, string folderPath)
    {
        report.AppendLine("=== DETAILED TILEMAP ANALYSIS ===");
        report.AppendLine();

        foreach (var analysis in analyses)
        {
            report.AppendLine("Tilemap: " + analysis.metadata.tilemapName);
            report.AppendLine("Metadata file: " + analysis.metadataFile);
            report.AppendLine("Position: " + analysis.metadata.position);
            report.AppendLine("Rotation: " + analysis.metadata.rotation.eulerAngles);
            report.AppendLine("Scale: " + analysis.metadata.scale);
            report.AppendLine("Bounds: " + analysis.metadata.boundsMin + " to " + analysis.metadata.boundsMax);
            report.AppendLine("Chunk size: " + analysis.metadata.chunkSize);
            report.AppendLine("Compression: " + analysis.metadata.compressionMethod);
            report.AppendLine("Total chunks: " + analysis.totalChunks);
            report.AppendLine();

            // Анализ чанков
            report.AppendLine("  CHUNK ANALYSIS:");
            int totalTiles = 0;
            var tileStatistics = new Dictionary<string, int>();
            var rotationStatistics = new Dictionary<float, int>();

            foreach (var chunk in analysis.chunks)
            {
                int chunkTiles = 0;

                if (chunk.useCompression && chunk.compressedTiles != null)
                {
                    foreach (var compressedTile in chunk.compressedTiles)
                    {
                        chunkTiles += compressedTile.length;

                        // Статистика по типам тайлов
                        string tileKey = compressedTile.tileName + " (" + compressedTile.assetPath + ")";
                        if (!tileStatistics.ContainsKey(tileKey))
                            tileStatistics[tileKey] = 0;
                        tileStatistics[tileKey] += compressedTile.length;

                        // Статистика по вращениям
                        if (!rotationStatistics.ContainsKey(compressedTile.rotationZ))
                            rotationStatistics[compressedTile.rotationZ] = 0;
                        rotationStatistics[compressedTile.rotationZ] += compressedTile.length;
                    }
                }
                else if (chunk.tiles != null)
                {
                    chunkTiles = chunk.tiles.Count;
                    foreach (var tile in chunk.tiles)
                    {
                        // Статистика по типам тайлов
                        string tileKey = tile.tileName + " (" + tile.assetPath + ")";
                        if (!tileStatistics.ContainsKey(tileKey))
                            tileStatistics[tileKey] = 0;
                        tileStatistics[tileKey]++;

                        // Статистика по вращениям
                        if (!rotationStatistics.ContainsKey(tile.rotationZ))
                            rotationStatistics[tile.rotationZ] = 0;
                        rotationStatistics[tile.rotationZ]++;
                    }
                }

                totalTiles += chunkTiles;
                report.AppendLine("    Chunk [" + chunk.chunkX + "," + chunk.chunkY + "]: " + chunkTiles + " tiles");
            }

            report.AppendLine();
            report.AppendLine("  TOTAL TILES: " + totalTiles);
            report.AppendLine();

            // Статистика по тайлам
            report.AppendLine("  TILE TYPE STATISTICS:");
            var sortedTileStats = tileStatistics.OrderByDescending(x => x.Value).Take(20); // Топ 20
            foreach (var stat in sortedTileStats)
            {
                report.AppendLine("    " + stat.Key + ": " + stat.Value + " tiles");
            }

            report.AppendLine();
            report.AppendLine("  ROTATION STATISTICS:");
            var sortedRotationStats = rotationStatistics.OrderBy(x => x.Key);
            foreach (var stat in sortedRotationStats)
            {
                report.AppendLine("    Rotation " + stat.Key + "°: " + stat.Value + " tiles");
            }

            report.AppendLine();
            report.AppendLine("---");
            report.AppendLine();
        }

        // Общая статистика
        report.AppendLine("=== OVERALL STATISTICS ===");
        report.AppendLine("Total tilemaps analyzed: " + analyses.Count);
        report.AppendLine("Total chunks: " + analyses.Sum(a => a.totalChunks));

        // Подсчет общего количества тайлов
        int overallTotalTiles = 0;
        foreach (var analysis in analyses)
        {
            foreach (var chunk in analysis.chunks)
            {
                if (chunk.useCompression && chunk.compressedTiles != null)
                {
                    foreach (var compressedTile in chunk.compressedTiles)
                    {
                        overallTotalTiles += compressedTile.length;
                    }
                }
                else if (chunk.tiles != null)
                {
                    overallTotalTiles += chunk.tiles.Count;
                }
            }
        }
        report.AppendLine("Total tiles: " + overallTotalTiles);

        // Анализ файлов
        report.AppendLine();
        report.AppendLine("=== FILE SYSTEM ANALYSIS ===");
        string[] allJsonFiles = Directory.GetFiles(folderPath, "*.json");
        long totalSize = 0;
        foreach (string file in allJsonFiles)
        {
            FileInfo fileInfo = new FileInfo(file);
            totalSize += fileInfo.Length;
            report.AppendLine(Path.GetFileName(file) + ": " + (fileInfo.Length / 1024.0).ToString("F2") + " KB");
        }
        report.AppendLine("Total size: " + (totalSize / 1024.0 / 1024.0).ToString("F2") + " MB");
        report.AppendLine("Average file size: " + (totalSize / allJsonFiles.Length / 1024.0).ToString("F2") + " KB");

        // Сводка по сжатию
        report.AppendLine();
        report.AppendLine("=== COMPRESSION SUMMARY ===");
        var compressionMethods = analyses.Select(a => a.metadata.compressionMethod).Distinct();
        foreach (string method in compressionMethods)
        {
            int count = analyses.Count(a => a.metadata.compressionMethod == method);
            report.AppendLine(method + ": " + count + " tilemaps");
        }
    }

    private void SaveAnalysisToFile()
    {
        string filePath = EditorUtility.SaveFilePanel("Save Analysis Report",
            Application.dataPath, "tilemap_analysis.txt", "txt");

        if (!string.IsNullOrEmpty(filePath))
        {
            File.WriteAllText(filePath, analysisResult);
            EditorUtility.DisplayDialog("Success", "Analysis saved to: " + filePath, "OK");
        }
    }

    // Вспомогательные классы для анализа
    [System.Serializable]
    private class TilemapAnalysis
    {
        public TilemapMetadata metadata;
        public string metadataFile;
        public int totalChunks;
        public List<ChunkData> chunks = new List<ChunkData>();
    }

    // Классы данных должны совпадать с экспортером
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