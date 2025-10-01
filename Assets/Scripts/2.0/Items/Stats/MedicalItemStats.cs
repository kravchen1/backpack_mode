// Медицинское оборудование
using UnityEngine;

public class MedicalItemStats : ItemStats
{
    [Header("Medical Stats")]
    public int healAmount = 80;
    public float useTime = 2.0f;
    public bool isConsumable = true;
    public MedicalType medicalType = MedicalType.Health;

    public enum MedicalType { Health, Stamina, Antidote, Bandage }

    protected override void InitializeDescriptionTriples()
    {
        _descriptionTriples.AddRange(new[]
        {
            new DescriptionTriple("Type", "", ""),
            new DescriptionTriple("Rarity", "", ""),
            new DescriptionTriple("Quality", "", ""),
            new DescriptionTriple("Heal Amount", $"{healAmount}", ""),
            new DescriptionTriple("Use Time", $"{useTime:0.0}s", ""),
            new DescriptionTriple("Medical Type", medicalType.ToString(), ""),
            new DescriptionTriple("Consumable", isConsumable ? "Yes" : "No", ""),
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
            case "Heal Amount":
                return $"{healAmount}";
            case "Use Time":
                return $"{useTime:0.0}s";
            case "Medical Type":
                return medicalType.ToString();
            case "Consumable":
                return isConsumable ? "Yes" : "No";
            default:
                return base.GetSpecificStatValue(statKey);
        }
    }
}