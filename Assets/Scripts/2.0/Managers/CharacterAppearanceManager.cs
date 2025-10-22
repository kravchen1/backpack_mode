using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Data Structures
[System.Serializable]
public class BodyPrefabs
{
    [Tooltip("Unique identifier for this appearance set")]
    public int index;

    [Space(5)]
    public Sprite body1; // Down
    public Sprite body2; // Side
    public Sprite body3; // Up
}

[System.Serializable]
public class HairPrefabs
{
    [Tooltip("Unique identifier for this appearance set")]
    public int index;

    [Space(5)]
    public Sprite hair1; // Down
    public Sprite hair2; // Side
    public Sprite hair3; // Up
}

[System.Serializable]
public class EyesPrefabs
{
    [Tooltip("Unique identifier for this appearance set")]
    public int index;

    [Space(5)]
    public Sprite eye1; // Down
    public Sprite eye2; // Side
}
#endregion

public class CharacterAppearanceManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Head Sprites")]
    [SerializeField] private Sprite headDown;
    [SerializeField] private Sprite headSide;
    [SerializeField] private Sprite headUp;

    [Header("Body Variants")]
    [SerializeField] private List<BodyPrefabs> bodyPrefabs = new List<BodyPrefabs>();

    [Header("Hair Variants")]
    [SerializeField] private List<HairPrefabs> hairPrefabs = new List<HairPrefabs>();

    [Header("Eyes Variants")]
    [SerializeField] private List<EyesPrefabs> eyesPrefabs = new List<EyesPrefabs>();
    #endregion

    public static CharacterAppearanceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #region Public API - Head
    public Sprite GetHeadUp() => headUp;
    public Sprite GetHeadDown() => headDown;
    public Sprite GetHeadSide() => headSide;
    #endregion

    #region Public API - Body
    public Sprite GetBodyUp(int index)
    {
        BodyPrefabs bodyPrefab = GetBodyPrefab(index);
        return bodyPrefab?.body3;
    }

    public Sprite GetBodyDown(int index)
    {
        BodyPrefabs bodyPrefab = GetBodyPrefab(index);
        return bodyPrefab?.body1;
    }

    public Sprite GetBodySide(int index)
    {
        BodyPrefabs bodyPrefab = GetBodyPrefab(index);
        return bodyPrefab?.body2;
    }
    #endregion

    #region Public API - Hair
    public Sprite GetHairUp(int index)
    {
        HairPrefabs hairPrefab = GetHairPrefab(index);
        return hairPrefab?.hair3;
    }

    public Sprite GetHairDown(int index)
    {
        HairPrefabs hairPrefab = GetHairPrefab(index);
        return hairPrefab?.hair1;
    }

    public Sprite GetHairSide(int index)
    {
        HairPrefabs hairPrefab = GetHairPrefab(index);
        return hairPrefab?.hair2;
    }
    #endregion

    #region Public API - Eyes
    public Sprite GetEyeDown(int index)
    {
        EyesPrefabs eyePrefab = GetEyePrefab(index);
        return eyePrefab?.eye1;
    }

    public Sprite GetEyeSide(int index)
    {
        EyesPrefabs eyePrefab = GetEyePrefab(index);
        return eyePrefab?.eye2;
    }
    #endregion

    #region Validation & Data Integrity
    public bool HasBodyVariant(int index) => GetBodyPrefab(index) != null;
    public bool HasHairVariant(int index) => GetHairPrefab(index) != null;
    public bool HasEyeVariant(int index) => GetEyePrefab(index) != null;

    public int GetBodyVariantCount => bodyPrefabs.Count;
    public int GetHairVariantCount => hairPrefabs.Count;
    public int GetEyeVariantCount => eyesPrefabs.Count;
    #endregion

    #region Private Helper Methods
    private BodyPrefabs GetBodyPrefab(int index)
    {
        return bodyPrefabs.FirstOrDefault(prefab => prefab.index == index);
    }

    private HairPrefabs GetHairPrefab(int index)
    {
        return hairPrefabs.FirstOrDefault(prefab => prefab.index == index);
    }

    private EyesPrefabs GetEyePrefab(int index)
    {
        return eyesPrefabs.FirstOrDefault(prefab => prefab.index == index);
    }
    #endregion

    #region Editor Methods
#if UNITY_EDITOR
    [ContextMenu("Log Available Variants")]
    private void LogAvailableVariants()
    {
        Debug.Log($"Body Variants: {GetBodyVariantCount}");
        Debug.Log($"Hair Variants: {GetHairVariantCount}");
        Debug.Log($"Eye Variants: {GetEyeVariantCount}");
    }

    [ContextMenu("Validate Data Integrity")]
    private void ValidateDataIntegrity()
    {
        ValidateUniqueIndices(bodyPrefabs, "Body");
        ValidateUniqueIndices(hairPrefabs, "Hair");
        ValidateUniqueIndices(eyesPrefabs, "Eyes");
    }

    private void ValidateUniqueIndices<T>(List<T> list, string typeName) where T : class
    {
        var indices = new HashSet<int>();
        var duplicateIndices = new List<int>();

        foreach (var item in list)
        {
            int index = (int)item.GetType().GetField("index").GetValue(item);
            if (!indices.Add(index))
            {
                duplicateIndices.Add(index);
            }
        }

        if (duplicateIndices.Count > 0)
        {
            Debug.LogWarning($"{typeName} Prefabs: Found duplicate indices: {string.Join(", ", duplicateIndices)}");
        }
    }
#endif
    #endregion
}