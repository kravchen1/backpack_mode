using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class FriedPotato : Food
{
    public int health = 5;//надо заменить
    public int stamina = 2;//надо заменить
    public override void Activation()
    {
        Heal(health);
        PlayerPrefs.SetFloat("FriedPotatoStamina", stamina);
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

                var descr = CanvasDescription.GetComponent<DescriptionItemFriedPotato>();
                descr.health = health;
                descr.stamina = stamina;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
