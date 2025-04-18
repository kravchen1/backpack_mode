using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ManaAmulet : Stuff
{
    public int countManaStack = 2;
    public int countDebuffStack = 2;

    //public GameObject LogPoisonStackCharacter, LogPoisonStackEnemy;
    //public GameObject LogBleedStackCharacter, LogBleedStackEnemy;
    //public GameObject LogFrostStackCharacter, LogFrostStackEnemy;

    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            if (Player != null && Enemy != null)
            {
                if (Player.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                {
                    bool b = false;
                    foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                    {
                        if (icon.countStack >= countManaStack)
                        {
                            b = true;
                           // CreateLogMessage("Mana amulet spend " + countManaStack.ToString(), Player.isPlayer);
                            logManager.CreateLogMessageUse(originalName, "mana", countManaStack, Player.isPlayer);
                            //CreateLogMessage("ManaAmulet removed " + countManaStack.ToString() + " mana and inflict " + countDebuffStack.ToString() + " debuffs");
                            int r = Random.Range(0, 3);
                            string text = "ICON";
                            switch (r)
                            {
                                case 0:
                                    text += "POISON";
                                    //if (Player.isPlayer)
                                    //{
                                    //    CreateLogMessage(LogPoisonStackCharacter, "Mana amulet inflict " + countDebuffStack.ToString());
                                    //}
                                    //else
                                    //{
                                    //    CreateLogMessage(LogPoisonStackEnemy, "Mana amulet inflict " + countDebuffStack.ToString());
                                    //}
                                    logManager.CreateLogMessageInflict(originalName, "poison", countDebuffStack, Player.isPlayer);
                                    break;
                                case 1:
                                    text += "BLEED";
                                    //if (Player.isPlayer)
                                    //{
                                    //    CreateLogMessage(LogBleedStackCharacter, "Mana amulet inflict " + countDebuffStack.ToString());
                                    //}
                                    //else
                                    //{
                                    //    CreateLogMessage(LogBleedStackEnemy, "Mana amulet inflict " + countDebuffStack.ToString());
                                    //}
                                    logManager.CreateLogMessageInflict(originalName, "bleed", countDebuffStack, Player.isPlayer);
                                    break;
                                case 2:
                                    text += "FROST";
                                    //if (Player.isPlayer)
                                    //{
                                    //    CreateLogMessage(LogFrostStackCharacter, "Mana amulet inflict " + countDebuffStack.ToString());
                                    //}
                                    //else
                                    //{
                                    //    CreateLogMessage(LogFrostStackEnemy, "Mana amulet inflict " + countDebuffStack.ToString());
                                    //}
                                    logManager.CreateLogMessageInflict(originalName, "frost", countDebuffStack, Player.isPlayer);
                                    break;
                            }
                            Enemy.menuFightIconData.AddDebuff(countDebuffStack, text);
                            if(r==2)
                                Enemy.menuFightIconData.CalculateFireFrostStats();//true = Player
                        }
                    }
                    if (b)
                    {
                        Player.menuFightIconData.DeleteBuff(countManaStack, "ICONMANA");
                        CheckNestedObjectActivation("StartBag");
                        CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                    }
                }
                //Debug.Log("шлем дал" + countBurnStack.ToString() + " эффектов горения");
                //var calculateFight = GameObject.FindGameObjectWithTag("CalculatedFight").GetComponent<CalculatedFight>();
                //Player.menuFightIconData.CalculateFireFrostStats();//true = Player
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

                var descr = CanvasDescription.GetComponent<DescriptionItemManaAmulet>();
                descr.cooldown = timer_cooldown;
                descr.countNeedManaStack = countManaStack;
                descr.countDebuffStack = countDebuffStack;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }
}
