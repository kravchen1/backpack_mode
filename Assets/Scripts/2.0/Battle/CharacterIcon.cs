using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterIcon : MonoBehaviour
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

    public NPCDataManager NPCCharacter { get; private set; }

    public void Initialize(NPCDataManager character, bool isEnemy)
    {
        gameObject.SetActive(true);
        NPCCharacter = character;

        characterImage.sprite = character.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;

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

        //healthBar.color =
        //    healthPercent > 0.6f ? Color.green :
        //    healthPercent > 0.3f ? Color.yellow : Color.red;


        float staminaPercent = (float)NPCCharacter.Stats.CurrentStamina / NPCCharacter.Stats.MaxStamina;
        staminaBar.fillAmount = staminaPercent;
        staminaText.text = $"{NPCCharacter.Stats.CurrentStamina}/{NPCCharacter.Stats.MaxStamina}";

        //staminaBar.color =
        //    healthPercent > 0.6f ? Color.green :
        //    healthPercent > 0.3f ? Color.yellow : Color.red;
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
    }

    public void OnIconClickPlayerTeam()
    {
        chooseBackpack();
        if (BattleManager.Instance != null && NPCCharacter != null)
            BattleManager.Instance.OnTargetSelected(NPCCharacter);
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