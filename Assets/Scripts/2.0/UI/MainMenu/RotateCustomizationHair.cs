using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RotateCustomizationHair : MonoBehaviour
{
    public int hairIndex = 1;
    public TextMeshProUGUI textHair;
    public RotateCustomizationCharacter characterCustomization;

    public void RotateLeft()
    {
        hairIndex--;
        int maxHairIndex = CharacterAppearanceManager.Instance.GetHairVariantCount;

        if (hairIndex < 0)
            hairIndex = maxHairIndex-1;

        UpdateHairText();
        characterCustomization.UpdateHairIndex(hairIndex);
    }

    public void RotateRight()
    {
        hairIndex++;
        int maxHairIndex = CharacterAppearanceManager.Instance.GetHairVariantCount;

        if (hairIndex > maxHairIndex-1)
            hairIndex = 0;

        UpdateHairText();
        characterCustomization.UpdateHairIndex(hairIndex);
    }

    private void UpdateHairText()
    {
        int maxHairIndex = CharacterAppearanceManager.Instance.GetHairVariantCount;

        // Проверяем, существует ли прическа с таким индексом
        if (CharacterAppearanceManager.Instance.HasHairVariant(hairIndex))
        {
            textHair.text = $"{hairIndex}";
        }
        else
        {
            textHair.text = "Недоступно";
            Debug.LogWarning($"Hair variant with index {hairIndex} not found!");
        }
    }

    // Опционально: вызывать при старте, чтобы текст соответствовал начальному значению
    private void Start()
    {
        // Убедимся, что индекс в допустимых пределах
        int maxHairIndex = CharacterAppearanceManager.Instance.GetHairVariantCount;
        if (hairIndex > maxHairIndex)
        {
            hairIndex = 1;
        }

        UpdateHairText();
    }

    public void RandomHair()
    {
        hairIndex = Random.Range(0, CharacterAppearanceManager.Instance.GetHairVariantCount);
        UpdateHairText();
        characterCustomization.UpdateHairIndex(hairIndex);
    }
}