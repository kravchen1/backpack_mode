using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class VampireGloves : Stuff
{
    public int countBleedStack = 1;

    public override void StarActivation(Item item)
    {
        //Активация звёздочек(предмет): накладывает n кровотечения
        if(Enemy != null)
        {
            Enemy.menuFightIconData.AddDebuff(countBleedStack, "ICONBLEED");
            //CreateLogMessage("Vampire gloves inflict " + countBleedStack.ToString(), Player.isPlayer);

            logManager.CreateLogMessageInflict(originalName, "bleed", countBleedStack, Player.isPlayer);
        }
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
                FillnestedObjectStarsStars(256);
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                var descr = CanvasDescription.GetComponent<DescriptionItemVampireGloves>();
                descr.countBleedStack = countBleedStack;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
