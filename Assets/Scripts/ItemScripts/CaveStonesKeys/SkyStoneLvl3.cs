using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class SkyStoneLvl3 : CaveStonesKeys
{
    
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //CoolDownStart();
        }

        //if (SceneManager.GetActiveScene().name == "BackPackShop")
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
            FillnestedObjectStarsStars(256);
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemSkyStoneLvl3>();
                //descr.cooldown = timer_cooldown;
                //descr.countStack = countBurnStack;
                //descr.coolDown = coolDown;
                descr.SetTextBody();
            }
        }
    }

}
