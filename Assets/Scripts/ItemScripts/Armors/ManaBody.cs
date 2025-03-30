using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ManaBody : Armor
{
    public int countStarArmorStack = 5;
    public int countStarManaStack = 3;
    public int countManaStack = 1;

    public GameObject LogArmorStackCharacter, LogArmorStackEnemy;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            FillnestedObjectStarsStars(256, "Mana");
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
        }
    }
    public override void StartActivation()
    {
        if (Player != null)
        {
            int countStar = stars.Where(e => e.GetComponent<Cell>().nestedObject != null).Count();
            int resultArmor = (startBattleArmorCount + countStar * countStarArmorStack);
            int resultMana = countManaStack + countStar * countStarManaStack;
            Player.armor = Player.armor + resultArmor;
            Player.armorMax = Player.armorMax + resultArmor;
            Player.menuFightIconData.AddBuff(resultMana, "IconMana");

            CreateLogMessage("Mana Body give " + resultMana.ToString(), Player.isPlayer);

            if (Player.isPlayer)
            {
                CreateLogMessage(LogArmorStackCharacter, "Mana Body give " + resultArmor.ToString());
            }
            else
            {
                CreateLogMessage(LogArmorStackEnemy, "Dinosaur give " + resultArmor.ToString());
            }

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillnestedObjectStarsStars(256, "Mana");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                CanvasDescription.GetComponent<DescriptionItemManaBody>().countStarArmorStack = countStarArmorStack;
                CanvasDescription.GetComponent<DescriptionItemManaBody>().countStarManaStack = countStarManaStack;
                CanvasDescription.GetComponent<DescriptionItemManaBody>().countManaStack = countManaStack;
                CanvasDescription.GetComponent<DescriptionItemManaBody>().armor = startBattleArmorCount;
                CanvasDescription.GetComponent<DescriptionItemManaBody>().SetTextBody();
            }
        }
    }

}
