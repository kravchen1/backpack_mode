using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class BottleOfWine : Food
{
    public int poison = 5;
    public int critChance = 5;

    public override void Activation()
    {
        PlayerPrefs.SetInt("BottleOfWinePoison", poison);
        PlayerPrefs.SetInt("BottleOfWineCritChance", critChance);
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillStars();
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemBottleOfWine>();
                descr.poison = poison;
                descr.critChance = critChance;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
