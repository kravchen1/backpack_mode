using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class VampireAmulet : Armor
{
    private bool isUse = false;
    private bool usable = false;
    private int currentTick = 0;
    private void Start()
    {
        FillnestedObjectStarsStars(256, "Vampire");
    }

    public override void StartActivation()
    {
        if (!isUse)
        {
            if (stars.Where(e => e.GetComponent<Cell>().nestedObject != null).Count() == stars.Count)
            {
                PlayerPrefs.SetInt("VampireAmulet", 1);
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
                StartActivation();
                //animator.speed = 1f / timer_cooldown;
                //animator.Play(originalName + "Activation");
            }
        }
    }

    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDownStart();
        }

        //if (SceneManager.GetActiveScene().name == "BackPackShop")
        else
        {
            defaultItemUpdate();
        }
    }

    public override IEnumerator ShowDescription()
    {
        FillnestedObjectStarsStars(256, "Vampire");
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillnestedObjectStarsStars(256);
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                var descr = CanvasDescription.GetComponent<DescriptionItemVampireAmulet>();
                //descr.hpDrop = hpDrop;
                //descr.countArmorStack = countArmorStack;
                //descr.countArmorStack = countResistStack;
                //descr.countArmorStack = countSpendManaStack;
                descr.SetTextBody();
            }
        }
    }

}
