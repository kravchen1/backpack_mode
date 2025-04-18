using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartEarthBag : Bag
{
    public int countArmorStack = 5;
    public override void StarActivation(Item item)
    {
        if (Player != null)
        {
            Player.armorMax += countArmorStack;
            Player.armor += countArmorStack;
            //CreateLogMessage("StartEarthBag give " + countArmorStack.ToString(), Player.isPlayer);
            logManager.CreateLogMessageGive(originalName, "armor", countArmorStack, Player.isPlayer);
        }
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

                var descr = CanvasDescription.GetComponent<DescriptionItemEarthBag>();
                descr.countArmorStack = countArmorStack;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }
}