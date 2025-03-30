using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ManaFlask : Flask
{
    public int giveStack = 27;
    
    public override void StartActivation()
    {
        if (Player != null)
        {
            Player.menuFightIconData.AddBuff(giveStack, "IconMana");
            CreateLogMessage("Mana flask give " + giveStack.ToString(), Player.isPlayer);
            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                CanvasDescription.GetComponent<DescriptionItemManaFlask>().giveStack = giveStack;
                CanvasDescription.GetComponent<DescriptionItemManaFlask>().SetTextBody();
            }
        }
    }

}
