using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class EmeraldStoneLvl3: CaveStonesKeys
{
   
    

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

                var descr = CanvasDescription.GetComponent<DescriptionItemEmeraldStoneLvl3>();
                //descr.cooldown = timer_cooldown;
                //descr.countStack = countBurnStack;
                //descr.coolDown = coolDown;
                descr.SetTextBody();
            }
        }
    }

}
