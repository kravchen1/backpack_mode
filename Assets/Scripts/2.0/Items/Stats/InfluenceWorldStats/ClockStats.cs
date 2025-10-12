using UnityEngine;

public class ClockStats : ItemStats
{
    [Header("Clock Stats")]
    public bool isShowTime = true;
    public bool isShowDate = false;

    public override void InitializeDescriptionTriples()
    {
        if (_descriptionTriples.Count > 0)
        {
            _descriptionTriples.Clear();
        }

        _descriptionTriples.AddRange(new[]
        {
            new DescriptionTriple("Flash Light Intensity", "", ""),
            new DescriptionTriple("Weight", "", ""),
            new DescriptionTriple("Durability", "", ""),
            new DescriptionTriple("Requirements", "", ""),
            new DescriptionTriple("Price", "", "")
        });

        if(isShowTime)
        {
            _descriptionTriples.Add(new DescriptionTriple("Show Time","",""));        
        }
        if (isShowDate)
        {
            _descriptionTriples.Add(new DescriptionTriple("Show Date", "", ""));
        }
    }

    protected override string GetSpecificStatValue(string statKey)
    {
        switch (statKey)
        {
            case "Show Time":
                return isShowTime.ToString();
            case "Show Date":
                return isShowDate.ToString();
            default:
                return base.GetSpecificStatValue(statKey);
        }
    }
}