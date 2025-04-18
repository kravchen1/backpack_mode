using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class VampireAmulet : Armor
{
    private int currentTick = 0;
    private void Start()
    {
        FillStars();
    }

    public override void StartActivation()
    {
        if (stars.Where(e => e.GetComponent<Cell>().nestedObject != null).Count() == stars.Count)
        {
            PlayerPrefs.SetInt("VampireAmulet", 1);
        }
    }

    protected override void FillStars()
    {
        FillnestedObjectStarsStars(256, "Vampire");
    }
    public override void ShowDescription()
    {  
        //yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillStars();
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                var descr = CanvasDescription.GetComponent<DescriptionItemVampireAmulet>();
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
