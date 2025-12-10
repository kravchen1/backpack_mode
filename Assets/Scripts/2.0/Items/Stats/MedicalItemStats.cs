// Медицинское оборудование
using UnityEngine;

public class MedicalItemStats : ItemStats
{
    [Header("Medical Stats")]
    [HideInInspector][SerializeField] public int countHeal = 80;
    [HideInInspector][SerializeField] public int durabilityCost = 40;
    public MedicalType medicalType = MedicalType.Health;

    public enum MedicalType { Health, Stamina, Antidote, Bandage }

    public override void InitializeDescriptionTriples()
    {
        if (_descriptionTriples.Count > 0)
        {
            _descriptionTriples.Clear();
        }
        _descriptionTriples.AddRange(new[]
        {
            new DescriptionTriple("Type", "", ""),
            new DescriptionTriple("Rarity", "", ""),
            new DescriptionTriple("Quality", "", ""),
            new DescriptionTriple("Medical Type", medicalType.ToString(), ""),
            new DescriptionTriple("Weight", "", ""),
            new DescriptionTriple("Durability", "", ""),
            //new DescriptionTriple("Requirements", "", ""),
            new DescriptionTriple("Price", "", "")
        });
    }

    protected override string GetSpecificStatValue(string statKey)
    {
        //switch (statKey)
        //{
        //    case "Heal Amount":
        //        return $"{healAmount}";
        //    case "Use Time":
        //        return $"{useTime:0.0}s";
        //    case "Medical Type":
        //        return medicalType.ToString();
        //    case "Consumable":
        //        return isConsumable ? "Yes" : "No";
        //    default:
        //        return base.GetSpecificStatValue(statKey);
        //}
        return null;
    }
}