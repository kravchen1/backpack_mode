
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MudWorm : Weapon
{
    public int removesRandomDebuff;

    public GameObject LogPoisonStackCharacter, LogPoisonStackEnemy;
    public GameObject LogFrostStackCharacter, LogFrostStackEnemy;
    public GameObject LogBlindStackCharacter, LogBlindStackEnemy;
    public GameObject LogBleedStackCharacter, LogBleedStackEnemy;
    //private float timer1sec = 1f;
    //public int countIncreasesCritDamage = 10;
    public override void ActivationEffect(int resultDamage)
    {
        RemovedDebuff();
    }
    public void CreateMessageLog(string iconName)
    {
        switch (iconName)
        {
            case "IconPoison":
                //if (Player.isPlayer)
                //{
                //    CreateLogMessage(LogPoisonStackCharacter, "Mud worm remove 1");
                //}
                //else
                //{
                //    CreateLogMessage(LogPoisonStackEnemy, "Mud worm remove 1");
                //}
                logManager.CreateLogMessageRemove(originalName, "poison", 1, Player.isPlayer);
                break;
            case "IconFrost":
                //if (Player.isPlayer)
                //{
                //    CreateLogMessage(LogFrostStackCharacter, "Mud worm remove 1");
                //}
                //else
                //{
                //    CreateLogMessage(LogFrostStackEnemy, "Mud worm remove 1");
                //}
                logManager.CreateLogMessageRemove(originalName, "frost", 1, Player.isPlayer);
                break;
            case "IconBlind":
                //if (Player.isPlayer)
                //{
                //    CreateLogMessage(LogBlindStackCharacter, "Mud worm remove 1");
                //}
                //else
                //{
                //    CreateLogMessage(LogBleedStackEnemy, "Mud worm remove 1");
                //}
                logManager.CreateLogMessageRemove(originalName, "blind", 1, Player.isPlayer);
                break;
            case "IconBleed":
                //if (Player.isPlayer)
                //{
                //    CreateLogMessage(LogBleedStackCharacter, "Mud worm remove 1");
                //}
                //else
                //{
                //    CreateLogMessage(LogBleedStackEnemy, "Mud worm remove 1");
                //}
                logManager.CreateLogMessageRemove(originalName, "bleed", 1, Player.isPlayer);
                break;
        }
    }

    public void RemovedDebuff()
    {
        if (Player.menuFightIconData.icons.Where(e => e.Buff == false).Count() > 0)
        {
            var debuffs = Player.menuFightIconData.icons.Where(e => e.Buff == false).ToList();

            int countPlayerDebuff = 0;
            foreach (var icon in debuffs)
            {
                countPlayerDebuff += icon.countStack;
            }

            if (countPlayerDebuff >= removesRandomDebuff)
            {
                int removeNow = 0;
                while (removeNow < removesRandomDebuff)
                {
                    int r = UnityEngine.Random.Range(0, debuffs.Count);
                    //Debug.Log(Buffs[r].sceneGameObjectIcon.name.Replace("(Clone)", ""));
                    string debuff = debuffs[r].sceneGameObjectIcon.name.Replace("(Clone)", "");
                    Player.menuFightIconData.DeleteBuff(1, debuff);
                    CreateMessageLog(debuff);
                    removeNow++;
                }
            }
            else
            {
                int removeNow = 0;
                while (removeNow < countPlayerDebuff)
                {
                    int r = UnityEngine.Random.Range(0, debuffs.Count);
                    string debuff = debuffs[r].sceneGameObjectIcon.name.Replace("(Clone)", "");
                    Enemy.menuFightIconData.DeleteBuff(1, debuff);
                    CreateMessageLog(debuff);
                    removeNow++;
                }
            }
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

                var descr = CanvasDescription.GetComponent<DescriptionItemMudWorm>();
                //descr.countIncreasesCritDamage = countIncreasesCritDamage;
                descr.SetTextBody();

                
                if (Player != null)
                {
                    descr.damageMin = attackMin + Player.menuFightIconData.CalculateAddPower();
                    descr.damageMax = attackMax + Player.menuFightIconData.CalculateAddPower();
                    descr.accuracyPercent = Player.menuFightIconData.ReturnBlindAndAccuracy(accuracy);
                    descr.critDamage = (int)(Player.menuFightIconData.CalculateCritDamage(critDamage) * 100);
                    descr.chanceCrit = chanceCrit + (int)Player.menuFightIconData.CalculateChanceCrit();
                }
                else
                {
                    descr.damageMin = attackMin;
                    descr.damageMax = attackMax;
                    descr.accuracyPercent = accuracy;
                    descr.critDamage = critDamage;
                    descr.chanceCrit = chanceCrit;
                }
                descr.staminaCost = stamina;
                descr.removesRandomDebuff = removesRandomDebuff;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
