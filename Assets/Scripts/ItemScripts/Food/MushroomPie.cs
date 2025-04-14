using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class MushroomPie : Food
{
    public int health = 5;//надо заменить
    public int poison = 2;//надо заменить
    public override void Activation()
    {
        Heal(health);
        PlayerPrefs.SetInt("MushroomPiePoison", poison);
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillStars();
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemMushroomPie>();
                descr.health = health;
                descr.poison = poison;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
