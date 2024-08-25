using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBackpackBattle : MonoBehaviour
{
    public GameObject backpack;
    public GameObject armorBar;
    public GameObject hpBar;
    public GameObject staminaBar;
    public GameObject expBar;

    public float armor = 0f;
    public float armorMax = 0f;

    public float hp = 74f;
    public float maxHP = 100f;

    public float stamina = 74f;
    public float staminaMax = 100f;
    public float staminaRegenerate = 1f;



    private Canvas canvas;
    private Image[] hpBarImages;
    private Image[] staminaBarImages;
    private Image[] armorBarImages;

    private Text[] textBarHP;
    private Text[] textBarStamina;
    private Text[] textBarArmor;

    void Start()
    {
        canvas = armorBar.GetComponentInParent<Canvas>();

        hpBarImages = hpBar.GetComponentsInChildren<Image>();
        staminaBarImages = staminaBar.GetComponentsInChildren<Image>();
        armorBarImages = armorBar.GetComponentsInChildren<Image>();

        textBarHP = hpBar.GetComponentsInChildren<Text>();
        textBarStamina = staminaBar.GetComponentsInChildren<Text>();
        textBarArmor = armorBar.GetComponentsInChildren<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] != null)
            {
                if(images[i].tag == "Bar")
                {
                    images[i].fillAmount = currentValue / maxValue;
                }
            }
        }
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != null)
            {
                if (texts[i].tag == "TextBar")
                {
                    texts[i].text = currentValue + "/" + maxValue;
                }
            }
        }
    }

    public void staminaRegenerating()
    {
        if (stamina < staminaMax)
        {
            stamina += staminaRegenerate * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        changeImages(hpBarImages,textBarHP, hp, maxHP);
        changeImages(staminaBarImages,textBarStamina, stamina, staminaMax);
        if (armorMax > 0)
        {
            changeImages(armorBarImages, textBarArmor, armor, armorMax);
        }

        ShowArmorBar();
        staminaRegenerating();

    }
}
