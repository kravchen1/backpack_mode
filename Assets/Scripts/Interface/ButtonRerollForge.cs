using TMPro;
using UnityEngine;

public class ButtonRerollForge : Button
{
    private int countRerollBeforePriceIncrease = 10;
    private int countIncrease = 1;
    public TextMeshProUGUI textPrice;
    public GameObject forge;

    private CharacterStats stat;
    private void Awake()
    {
        stat = GameObject.FindGameObjectWithTag("Stat").GetComponent<CharacterStats>();


        if (!PlayerPrefs.HasKey("countRerollBeforePriceIncreaseForge"))
        {
            PlayerPrefs.SetInt("countRerollBeforePriceIncreaseForge", 0);
        }

        if (!PlayerPrefs.HasKey("priceRerollForge"))
        {
            PlayerPrefs.SetInt("priceRerollForge", 1);
        }
        else
        {
            textPrice.text = PlayerPrefs.GetInt("priceRerollForge").ToString();
        }
    }

    override public void OnMouseUpAsButton()
    {
        bool rerolling = false;
        if (stat.playerCoins - PlayerPrefs.GetInt("priceRerollForge") >= 0)
        {
            var listForgeData = forge.GetComponent<GenerateForgeItems>().listForgeData;
            if (listForgeData != null)
                listForgeData.Clear();

            var objectsForge = GameObject.FindGameObjectsWithTag("ForgeItem");

            foreach(var objectForge in objectsForge)
            {
                Destroy(objectForge.gameObject);
            }
            //var listForgeData = f
            //bool rerolling = false;

            forge.GetComponent<GenerateForgeItems>().GenerateRandomItem();
            rerolling = true;

            if (rerolling)
            {
                stat.playerCoins -= PlayerPrefs.GetInt("priceRerollForge");
                PlayerPrefs.SetInt("countRerollBeforePriceIncreaseForge", PlayerPrefs.GetInt("countRerollBeforePriceIncreaseForge") + 1);

                if (PlayerPrefs.GetInt("countRerollBeforePriceIncreaseForge") == countRerollBeforePriceIncrease)
                {
                    PlayerPrefs.SetInt("countRerollBeforePriceIncreaseForge", 0);
                    PlayerPrefs.SetInt("priceRerollForge", PlayerPrefs.GetInt("priceRerollForge") + countIncrease);
                }


                textPrice.text = PlayerPrefs.GetInt("priceRerollForge").ToString();
            }


        }
    }
}
