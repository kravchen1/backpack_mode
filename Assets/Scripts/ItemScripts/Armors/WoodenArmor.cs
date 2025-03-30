using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class WoodenArmor : Armor
{
    public override void StartActivation()
    {
        if (Player != null)
        {
            Player.armor = Player.armor + startBattleArmorCount;
            Player.armorMax = Player.armorMax + startBattleArmorCount;
            //Debug.Log("FireBody give " + startBattleArmorCount + " armor");
            CreateLogMessage("WoodenArmor give " + startBattleArmorCount.ToString(), Player.isPlayer);
            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
    }

    
    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillnestedObjectStarsStars(256);
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemWoodenArmor>();
                descr.armor = startBattleArmorCount;
                descr.SetTextBody();
            }
        }
    }

}
