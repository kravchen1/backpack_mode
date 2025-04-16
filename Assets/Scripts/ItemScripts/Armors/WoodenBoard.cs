using System.Collections;
using UnityEngine;

public class WoodenBoard : Junk
{
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
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform) as GameObject;

                var descr = CanvasDescription.GetComponent<DescriptionItemWoodenBoard>();
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
