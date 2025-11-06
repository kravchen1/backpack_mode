using UnityEngine;

public class FlashLightStats : ItemStats
{
    [Header("FlashLight Stats")]
    public float _flashLightRadius = 7f;
    public float _flashLightIntensity = 0.5f;

    public override void InitializeDescriptionTriples()
    {
        if (_descriptionTriples.Count > 0)
        {
            _descriptionTriples.Clear();
        }

        _descriptionTriples.AddRange(new[]
        {
            new DescriptionTriple("Description", "", ""),
            new DescriptionTriple("Flash Light Radius", "", ""),
            new DescriptionTriple("Flash Light Intensity", "", ""),
            new DescriptionTriple("Weight", "", ""),
            new DescriptionTriple("Durability", "", ""),
            new DescriptionTriple("Requirements", "", ""),
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
}