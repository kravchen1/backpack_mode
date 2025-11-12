using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AdvancedPrefabSpawner : EditorWindow
{
    [MenuItem("Tools/Advanced Prefab Spawner")]
    public static void ShowWindow()
    {
        GetWindow<AdvancedPrefabSpawner>("Advanced Prefab Spawner");
    }

    [System.Serializable]
    public class PrefabSpawnInfo
    {
        public GameObject prefabToSpawn;
        public int spawnCount = 1;
    }

    [System.Serializable]
    public class SpawnConfig
    {
        public List<GameObject> containerObjects = new List<GameObject>();
        public List<PrefabSpawnInfo> prefabsToSpawn = new List<PrefabSpawnInfo>();
        public Vector2Int xRange = new Vector2Int(-47, 47);
        public Vector2Int yRange = new Vector2Int(-47, 47);
        public bool spawnAsChild = true;
    }

    public List<SpawnConfig> spawnConfigs = new List<SpawnConfig>();
    private Vector2 scrollPosition;
    private bool hasValidationError = false;
    private string validationMessage = "";

    // SerializedObject для работы со списками
    private SerializedObject serializedObject;
    private SerializedProperty spawnConfigsProperty;

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        spawnConfigsProperty = serializedObject.FindProperty("spawnConfigs");
    }

    private void OnGUI()
    {
        serializedObject.Update();

        GUILayout.Label("Advanced Prefab Spawner", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Spawn multiple prefabs in multiple containers with integer positions", MessageType.Info);

        GUILayout.Space(10);

        ValidateConfigsSilent();

        if (hasValidationError)
        {
            EditorGUILayout.HelpBox(validationMessage, MessageType.Error);
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < spawnConfigs.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"Spawn Config {i + 1}", EditorStyles.boldLabel);

            // Секция контейнеров - теперь с перетаскиванием списка
            EditorGUILayout.LabelField("Container Objects:", EditorStyles.boldLabel);

            SerializedProperty configProperty = spawnConfigsProperty.GetArrayElementAtIndex(i);
            SerializedProperty containersProperty = configProperty.FindPropertyRelative("containerObjects");

            EditorGUILayout.PropertyField(containersProperty, new GUIContent("Containers List"), true);

            GUILayout.Space(10);

            // Секция префабов
            EditorGUILayout.LabelField("Prefabs to Spawn:", EditorStyles.boldLabel);

            SerializedProperty prefabsProperty = configProperty.FindPropertyRelative("prefabsToSpawn");
            EditorGUILayout.PropertyField(prefabsProperty, new GUIContent("Prefabs List"), true);

            GUILayout.Space(10);

            // Диапазоны координат
            EditorGUILayout.LabelField("Integer Position Ranges (Local):", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("X Range:", GUILayout.Width(60));
            spawnConfigs[i].xRange.x = EditorGUILayout.IntField("Min", spawnConfigs[i].xRange.x);
            spawnConfigs[i].xRange.y = EditorGUILayout.IntField("Max", spawnConfigs[i].xRange.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y Range:", GUILayout.Width(60));
            spawnConfigs[i].yRange.x = EditorGUILayout.IntField("Min", spawnConfigs[i].yRange.x);
            spawnConfigs[i].yRange.y = EditorGUILayout.IntField("Max", spawnConfigs[i].yRange.y);
            EditorGUILayout.EndHorizontal();

            spawnConfigs[i].spawnAsChild = EditorGUILayout.Toggle("Spawn as Child", spawnConfigs[i].spawnAsChild);

            // Кнопка удаления конфига
            if (GUILayout.Button("Remove This Config", GUILayout.Height(25)))
            {
                spawnConfigs.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        // Кнопки управления
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add New Config"))
        {
            spawnConfigs.Add(new SpawnConfig());
        }

        if (GUILayout.Button("Clear All"))
        {
            if (EditorUtility.DisplayDialog("Clear All",
                "Are you sure you want to clear all configurations?", "Yes", "No"))
            {
                spawnConfigs.Clear();
            }
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // Кнопка спавна
        EditorGUI.BeginDisabledGroup(spawnConfigs.Count == 0 || hasValidationError);
        if (GUILayout.Button("Spawn Prefabs", GUILayout.Height(30)))
        {
            serializedObject.ApplyModifiedProperties();
            if (ValidateConfigsWithDialog() && SpawnPrefabs())
            {
                EditorUtility.DisplayDialog("Success", "Prefabs spawned successfully!", "OK");
            }
        }
        EditorGUI.EndDisabledGroup();

        if (spawnConfigs.Count == 0)
        {
            EditorGUILayout.HelpBox("Add at least one spawn configuration to continue", MessageType.Warning);
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Spawns multiple prefabs in multiple containers with integer local positions.", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }

    private void ValidateConfigsSilent()
    {
        hasValidationError = false;
        validationMessage = "";

        if (spawnConfigs.Count == 0)
            return;

        foreach (var config in spawnConfigs)
        {
            if (config.containerObjects.Count == 0)
            {
                hasValidationError = true;
                validationMessage = "One or more configs have no container objects!";
                return;
            }

            if (config.prefabsToSpawn.Count == 0)
            {
                hasValidationError = true;
                validationMessage = "One or more configs have no prefabs to spawn!";
                return;
            }

            foreach (var container in config.containerObjects)
            {
                if (container == null)
                {
                    hasValidationError = true;
                    validationMessage = "One or more container objects are not assigned!";
                    return;
                }
            }

            foreach (var prefabInfo in config.prefabsToSpawn)
            {
                if (prefabInfo.prefabToSpawn == null)
                {
                    hasValidationError = true;
                    validationMessage = "One or more prefabs are not assigned!";
                    return;
                }
            }

            if (config.xRange.x > config.xRange.y)
            {
                hasValidationError = true;
                validationMessage = "Invalid X range - min cannot be greater than max!";
                return;
            }

            if (config.yRange.x > config.yRange.y)
            {
                hasValidationError = true;
                validationMessage = "Invalid Y range - min cannot be greater than max!";
                return;
            }
        }
    }

    private bool ValidateConfigsWithDialog()
    {
        foreach (var config in spawnConfigs)
        {
            if (config.containerObjects.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "One or more configs have no container objects!", "OK");
                return false;
            }

            if (config.prefabsToSpawn.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "One or more configs have no prefabs to spawn!", "OK");
                return false;
            }

            foreach (var container in config.containerObjects)
            {
                if (container == null)
                {
                    EditorUtility.DisplayDialog("Error", "One or more container objects are not assigned!", "OK");
                    return false;
                }
            }

            foreach (var prefabInfo in config.prefabsToSpawn)
            {
                if (prefabInfo.prefabToSpawn == null)
                {
                    EditorUtility.DisplayDialog("Error", "One or more prefabs are not assigned!", "OK");
                    return false;
                }
            }

            if (config.xRange.x > config.xRange.y)
            {
                EditorUtility.DisplayDialog("Error", "Invalid X range - min cannot be greater than max!", "OK");
                return false;
            }

            if (config.yRange.x > config.yRange.y)
            {
                EditorUtility.DisplayDialog("Error", "Invalid Y range - min cannot be greater than max!", "OK");
                return false;
            }
        }
        return true;
    }

    private bool SpawnPrefabs()
    {
        int totalSpawned = 0;
        HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();

        foreach (var config in spawnConfigs)
        {
            if (config.containerObjects.Count == 0 || config.prefabsToSpawn.Count == 0)
                continue;

            foreach (var container in config.containerObjects)
            {
                if (container == null) continue;

                usedPositions.Clear();
                int containerSpawned = 0;

                // Спавним все префабы для этого контейнера
                foreach (var prefabInfo in config.prefabsToSpawn)
                {
                    for (int i = 0; i < prefabInfo.spawnCount; i++)
                    {
                        Vector3Int randomPosition = FindFreeIntegerPosition(config, usedPositions);
                        if (randomPosition == new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue))
                        {
                            Debug.LogWarning($"No more free positions in container: {container.name}");
                            break;
                        }

                        usedPositions.Add(randomPosition);

                        GameObject spawnedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabInfo.prefabToSpawn);

                        if (spawnedPrefab != null)
                        {
                            if (config.spawnAsChild)
                            {
                                spawnedPrefab.transform.SetParent(container.transform);
                                spawnedPrefab.transform.localPosition = randomPosition;
                                spawnedPrefab.transform.localRotation = Quaternion.identity;
                            }
                            else
                            {
                                spawnedPrefab.transform.position = container.transform.TransformPoint(randomPosition);
                                spawnedPrefab.transform.rotation = container.transform.rotation;
                            }

                            spawnedPrefab.name = $"{prefabInfo.prefabToSpawn.name}_{container.name}_{containerSpawned + 1}";
                            containerSpawned++;
                            totalSpawned++;
                            Undo.RegisterCreatedObjectUndo(spawnedPrefab, "Spawn Prefab");
                        }
                    }
                }

                Debug.Log($"Spawned {containerSpawned} prefabs in container: {container.name}");
            }
        }

        Debug.Log($"Process completed! Total prefabs spawned: {totalSpawned}");

        if (totalSpawned == 0)
        {
            Debug.LogWarning("No prefabs were spawned!");
            return false;
        }

        return true;
    }

    private Vector3Int FindFreeIntegerPosition(SpawnConfig config, HashSet<Vector3Int> usedPositions, int maxAttempts = 1000)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3Int randomPos = new Vector3Int(
                Random.Range(config.xRange.x, config.xRange.y + 1),
                Random.Range(config.yRange.x, config.yRange.y + 1),
                0
            );

            if (!usedPositions.Contains(randomPos))
            {
                return randomPos;
            }
        }

        return new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
    }
}