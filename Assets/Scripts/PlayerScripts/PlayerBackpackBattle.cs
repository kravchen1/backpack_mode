using UnityEngine;
using UnityEngine.UI;

public class PlayerBackpackBattle : MonoBehaviour
{
    public GameObject backpack;
    public GameObject hpBar;
    public GameObject staminaBar;

    public float hp = 74f;
    public float maxHP = 100f;

    public float stamina = 74f;
    public float maxStamina = 100f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.GetComponent<Image>().fillAmount = hp / maxHP;
        staminaBar.GetComponent<Image>().fillAmount = stamina / maxStamina;
    }
}
