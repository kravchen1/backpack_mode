using UnityEngine;

public class FlashLightStats : ItemStats
{
    //[Header("FlashLight Stats")]
    [HideInInspector] public float _flashLightRadius = 7f;
    [HideInInspector] public float _flashLightIntensity = 0.5f;

    public override void InitializeDescriptionTriples()
    {
        if (_descriptionTriples.Count > 0)
        {
            _descriptionTriples.Clear();
        }

        _descriptionTriples.AddRange(new[]
        {
            new DescriptionTriple("Description", "", ""),
            new DescriptionTriple("Type", "", ""),
            new DescriptionTriple("Rarity", "", ""),
            new DescriptionTriple("Quality", "", ""),
            new DescriptionTriple("Weight", "", ""),
            new DescriptionTriple("Durability", "", ""),
            new DescriptionTriple("Flash Light Radius", "", ""),
            new DescriptionTriple("Flash Light Intensity", "", ""),
            new DescriptionTriple("Price", "", "")
        });
    }

    protected override string GetSpecificStatValue(string statKey)
    {
        switch (statKey)
        {
            case "Flash Light Radius":
                return $"{_flashLightRadius:+#;-#;0.0}%";
            case "Flash Light Intensity":
                return $"{_flashLightIntensity:+#;-#;0.0}%";
            default:
                return base.GetSpecificStatValue(statKey);
        }
    }

    protected override void LoadFromDataManager()
    {
        base.LoadFromDataManager();

        if (string.IsNullOrEmpty(itemKey)) return;
        var dataManager = ItemDataManager.Instance;
        if (dataManager == null) return;

        // Загрузка параметров ближнего боя
        _flashLightRadius = dataManager.GetItemData(itemKey, "_flashLightRadius", _flashLightRadius);
        _flashLightIntensity = dataManager.GetItemData(itemKey, "_flashLightIntensity", _flashLightIntensity);
    }
}