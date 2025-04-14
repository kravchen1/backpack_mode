using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class Jam : Food
{
    public int avoidance = 5;//надо заменить

    public override void Activation()
    {
        PlayerPrefs.SetInt("JamAvoidance", avoidance);
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

                var descr = CanvasDescription.GetComponent<DescriptionItemJam>();
                descr.avoidance = avoidance;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
