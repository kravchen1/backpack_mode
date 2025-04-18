using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ManaHelmet : Flask
{
    public int hpDrop = 71;
    public int countArmorStack = 34;
    public int countResistStack = 10;
    public int countSpendManaStack = 2;

    private bool isUse = false;

    //public GameObject LogArmorStackCharacter, LogArmorStackEnemy;
    //public GameObject LogResistanceStackCharacter, LogResistanceStackEnemy;

    float f_ToPercent(float a, float p)
    {
        return (a / 100 * p); //возвращает значение из процентов, например если передать 200 и 30, то вернёт 60
    }
    public override void Activation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                if(Player.hp < f_ToPercent(Player.maxHP, hpDrop))
                {
                    foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                    {
                        if (icon.countStack >= countSpendManaStack)
                        {
                            isUse = true;
                            //CreateLogMessage("Mana helmet spend " + countSpendManaStack.ToString(), Player.isPlayer);
                            logManager.CreateLogMessageUse(originalName, "mana", countSpendManaStack, Player.isPlayer);
                            animator.speed = 5f;
                            animator.Play(originalName + "Activation", 0, 0f);
                        }
                    }
                    if(isUse)
                    {
                        //if (Player.isPlayer)
                        //{
                        //    CreateLogMessage(LogArmorStackCharacter, "Mana helmet give " + countArmorStack.ToString());
                        //    CreateLogMessage(LogResistanceStackCharacter, "Mana helmet give " + countResistStack.ToString());
                        //}
                        //else
                        //{
                        //    CreateLogMessage(LogArmorStackEnemy, "Mana helmet give " + countArmorStack.ToString());
                        //    CreateLogMessage(LogResistanceStackEnemy, "Mana helmet give " + countResistStack.ToString());
                        //}
                        logManager.CreateLogMessageGive(originalName, "armor", countArmorStack, Player.isPlayer);
                        logManager.CreateLogMessageGive(originalName, "resist", countResistStack, Player.isPlayer);

                        Player.menuFightIconData.DeleteBuff(countSpendManaStack, "ICONMANA");
                        Player.armor = Player.armor + countArmorStack;
                        Player.armorMax = Player.armorMax + countArmorStack;
                        Player.menuFightIconData.AddBuff(countResistStack, "ICONRESISTANCE");
                        CheckNestedObjectActivation("StartBag");
                        CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                    }
                }
            }
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
                var descr = CanvasDescription.GetComponent<DescriptionItemManaHelmet>();
                descr.hpDrop = hpDrop;
                descr.countArmorStack = countArmorStack;
                descr.countResistStack = countResistStack;
                descr.countSpendManaStack = countSpendManaStack;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
