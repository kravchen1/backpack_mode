using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterIcon : MonoBehaviour
{
    [Header("UI Elements")]
    public Image characterImageHead;
    public Image characterImageHair;
    public Image characterImageEyes;
    public Image characterImageBody;
    public Image characterImageArmor;
    public Image characterImageWeapon;

    public TextMeshProUGUI nameText;
    public Image selectionBorder;
    public Image backgroundImage;
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public Image staminaBar;
    public TextMeshProUGUI staminaText;


    public List<GameObject> backpacks;
    public GameObject backpackCanvasForThisIcon;
    public List<CharacterIcon> anotherIcons;
    public PlayerCharacterIcon playerIcon;

    [Header("Colors")]
    public Color playerColor = Color.blue;
    public Color enemyColor = Color.red;
    public Color selectedColor = Color.yellow;

    public NPCDataManager NPCCharacter { get; private set; }

    public void FixedUpdate()
    {
        UpdateBars();
        CheckAlive();
    }

    public void Initialize(NPCDataManager character, bool isEnemy)
    {
        gameObject.SetActive(true);
        NPCCharacter = character;

        characterImageHead.sprite = character.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;

        nameText.text = character.CharacterName;
        backgroundImage.color = isEnemy ? enemyColor : playerColor;
        selectionBorder.color = selectedColor;
        selectionBorder.gameObject.SetActive(false);

        UpdateBars();

        var button = GetComponent<Button>();
        if (button != null && isEnemy)
            button.onClick.AddListener(OnIconClickEnemyTeam);

        if (button != null && isEnemy)
            button.onClick.AddListener(OnIconClickPlayerTeam);

    }

    public void UpdateBars()
    {
        if (NPCCharacter == null) return;

        float healthPercent = (float)NPCCharacter.Stats.CurrentHealth / NPCCharacter.Stats.MaxHealth;
        healthBar.fillAmount = healthPercent;
        healthText.text = $"{NPCCharacter.Stats.CurrentHealth}/{NPCCharacter.Stats.MaxHealth}";


        float staminaPercent = (float)NPCCharacter.Stats.CurrentStamina / NPCCharacter.Stats.MaxStamina;
        staminaBar.fillAmount = staminaPercent;
        staminaText.text = $"{NPCCharacter.Stats.CurrentStamina:0.0}/{NPCCharacter.Stats.MaxStamina:0.0}";
    }

    public void SetSelected(bool selected)
    {
        selectionBorder.gameObject.SetActive(selected);
    }

    public void OnIconClickEnemyTeam()
    {
        chooseBackpack();
        if (BattleManager.Instance != null && NPCCharacter != null)
            BattleManager.Instance.OnTargetSelected(NPCCharacter);

        foreach (var anotherIcon in anotherIcons)
        {
            anotherIcon.SetSelected(false);
        }
        SetSelected(true);
    }

    public void OnIconClickPlayerTeam()
    {
        chooseBackpack();

        foreach (var anotherIcon in anotherIcons)
        {
            anotherIcon.SetSelected(false);
        }
        playerIcon.SetSelected(false);
        SetSelected(true);
    }

    void CheckAlive()
    {
        if(!NPCCharacter.IsAlive)
        {
            this.gameObject.SetActive(false);
        }
    }

    void chooseBackpack()
    {
        foreach (var backpack in backpacks)
        {
            backpack.GetComponent<RectTransform>().position = new Vector3(4000f, 0f, 0f);
        }
        backpackCanvasForThisIcon.GetComponent<RectTransform>().position = new Vector3(0f, 0f, 0f);
    }
}