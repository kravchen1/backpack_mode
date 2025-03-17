using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class FireFlask : Flask
{
    //public float howActivation = 30; //��� 30%(����� ������) ��� ���� ��������� ���������
    //public float percentHP = 20; //���� ��������� ����� �����������
    //public float percentRegenerate = 5; //���� ��������� ����� ��������������
    //public float timerRegenerate = 1; //��� ����� � �������� ����� ����������� �����������
    //public float maxTimeRegenerate = 4; //������ ��� ����� ����������� �����������

    public int countStack = 5;
    public int giveStack = 15;


    private bool isUse = false;
    private bool usable = false;
    private int currentTick = 0;
    
    public override void Activation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
                {
                    if (icon.countStack >= countStack)
                    {
                        isUse = true;

                        animator.speed = 5f;
                        animator.Play(originalName + "Activation", 0, 0f);
                    }
                }
                if (isUse)
                {
                    Player.menuFightIconData.AddBuff(giveStack, "ICONBURN");
                    Player.menuFightIconData.CalculateFireFrostStats();//true = Player
                    CheckNestedObjectActivation("StartBag");
                    CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                    CreateLogMessage("FireFlask give " + giveStack.ToString(), Player.isPlayer);

                }
            }
        }
    }

    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            Activation();
        }

        //if (SceneManager.GetActiveScene().name == "BackPackShop")
        else if (SceneManager.GetActiveScene().name != "GenerateMap" && SceneManager.GetActiveScene().name != "Cave")
        {
            defaultItemUpdate();
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
                var descr = CanvasDescription.GetComponent<DescriptionItemFireFlask>();
                descr.giveStack = giveStack;
                descr.countStack = countStack;
                descr.SetTextBody();
            }
        }
    }

}
