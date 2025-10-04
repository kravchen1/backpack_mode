using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerCharacterIcon : MonoBehaviour
{
    [Header("UI Elements")]
    public Image characterImage;
    public TextMeshProUGUI nameText;
    public Image selectionBorder;
    public Image backgroundImage;
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public Image staminaBar;
    public TextMeshProUGUI staminaText;


    public List<GameObject> backpacks;
    public GameObject backpackCanvasForThisIcon;

    [Header("Colors")]
    public Color playerColor = Color.blue;
    public Color enemyColor = Color.red;
    public Color selectedColor = Color.yellow;

    //public PlayerDataManager Character { get; private set; }


    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        //SpriteRenderer spriteRenderer = PlayerDataManager.Instance.playerCharacter.transform.GetChild(0).GetComponent<SpriteRenderer>();
        //playerIcon.GetComponent<Image>().sprite = spriteRenderer.sprite;
        gameObject.SetActive(true);

        characterImage.sprite = PlayerDataManager.Instance.playerCharacter.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;

        nameText.text = PlayerDataManager.Instance.PlayerName;
        backgroundImage.color = playerColor;
        selectionBorder.color = selectedColor;
        selectionBorder.gameObject.SetActive(false);

        UpdateBars();

        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnIconClick);

    }

    public void UpdateBars()
    {
        if (PlayerDataManager.Instance == null) return;

        float healthPercent = (float)PlayerDataManager.Instance.Stats.CurrentHealth / PlayerDataManager.Instance.Stats.MaxHealth;
        healthBar.fillAmount = healthPercent;
        healthText.text = $"{PlayerDataManager.Instance.Stats.CurrentHealth}/{PlayerDataManager.Instance.Stats.MaxHealth}";

        //healthBar.color =
        //    healthPercent > 0.6f ? Color.green :
        //    healthPercent > 0.3f ? Color.yellow : Color.red;


        float staminaPercent = (float)PlayerDataManager.Instance.Stats.CurrentStamina / PlayerDataManager.Instance.Stats.MaxStamina;
        staminaBar.fillAmount = staminaPercent;
        staminaText.text = $"{PlayerDataManager.Instance.Stats.CurrentStamina}/{PlayerDataManager.Instance.Stats.MaxStamina}";

        //staminaBar.color =
        //    healthPercent > 0.6f ? Color.green :
        //    healthPercent > 0.3f ? Color.yellow : Color.red;
    }

    public void SetSelected(bool selected)
    {
        selectionBorder.gameObject.SetActive(selected);
    }


    public void OnIconClick()
    {
        chooseBackpack();
        if (BattleManager.Instance != null && PlayerDataManager.Instance != null)
            BattleManager.Instance.PlayerTarget();
    }

    void chooseBackpack()
    {
        foreach(var backpack in backpacks)
        {
            backpack.transform.localScale = Vector3.zero;
        }
        backpackCanvasForThisIcon.transform.localScale = Vector3.one;
    }
}