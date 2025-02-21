using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class FireBody : Armor
{
    private bool isUse = false;
    public int DamageForStack = 5;
    public int SpendStack = 2;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
        }

    }


    public override void StartActivation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                Player.armor = Player.armor + startBattleArmorCount;
                Player.armorMax = Player.armorMax + startBattleArmorCount;
                isUse = true;
                //Debug.Log("FireBody give " + startBattleArmorCount + " armor");
                CreateLogMessage("FireBody give " + startBattleArmorCount.ToString() + " armor");
                CheckNestedObjectActivation("StartBag");
            }
        }
    }

    public override void StarActivation(Item item)
    {
        //��������� ��������(������� ����): ������ 1 ������ ������� � ������� ����� 5 �����
        if (Player != null && Enemy != null)
        {
            if (Player.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
            {
                bool b = false;
                foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
                {
                    if (icon.countStack >= SpendStack)
                    {
                        //Player.menuFightIconData.DeleteBuff(SpendStack, "ICONBURN");
                        b = true;
                        //Enemy.hp -= DamageForStack;
                        Attack(DamageForStack, false);
                        //Debug.Log("FiryBody �����" + SpendStack.ToString() + " ��� � ������� 5 �����");
                        CreateLogMessage("FireBody removed " + SpendStack.ToString() + " burn and apply " + DamageForStack.ToString() + " damage");
                        //animator.SetTrigger(originalName + "StarActivation");
                        //animator.Play("New State");
                        animator.Play(originalName + "Activation2", 0, 0f);
                        //animator.StartPlayback
                    }
                }
                if (b)
                {
                    Player.menuFightIconData.DeleteBuff(SpendStack, "ICONBURN");
                    Player.menuFightIconData.CalculateFireFrostStats();//true = Player
                }
            }
        }
    }

    private void CoolDownStart()
    {
        if (timer_locked_outStart)
        {
            timerStart -= Time.deltaTime;

            if (timerStart <= 0)
            {
                timer_locked_outStart = false;
                //animator.speed = 1f / timer_cooldown;
                StartActivation();
                animator.Play("New State");
            }
        }
    }
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //FillnestedObjectStarsStars(256, "RareWeapon");
            CoolDownStart();
        }

        // if (SceneManager.GetActiveScene().name == "BackPackShop")
        else
        {
            defaultItemUpdate();
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillnestedObjectStarsStars(256, "RareWeapon");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                CanvasDescription.GetComponent<DescriptionItemFireBody>().SpendStack = SpendStack;
                CanvasDescription.GetComponent<DescriptionItemFireBody>().DamageForStack = DamageForStack;
                CanvasDescription.GetComponent<DescriptionItemFireBody>().SetTextBody();
            }
        }
    }

}
