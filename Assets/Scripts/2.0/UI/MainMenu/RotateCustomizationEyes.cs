using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RotateCustomizationEyes : MonoBehaviour
{
    public int eyesIndex = 1;
    public TextMeshProUGUI textEyes;
    public RotateCustomizationCharacter characterCustomization;

    public void RotateLeft()
    {
        eyesIndex--;
        int maxEyesIndex = CharacterAppearanceManager.Instance.GetEyeVariantCount;

        if (eyesIndex < 1)
            eyesIndex = maxEyesIndex-1;

        UpdateEyesText();
        characterCustomization.UpdateEyeIndex(eyesIndex);
    }

    public void RotateRight()
    {
        eyesIndex++;
        int maxEyesIndex = CharacterAppearanceManager.Instance.GetEyeVariantCount;

        if (eyesIndex > maxEyesIndex-1)
            eyesIndex = 1;

        UpdateEyesText();
        characterCustomization.UpdateEyeIndex(eyesIndex);
    }

    private void UpdateEyesText()
    {
        int maxEyesIndex = CharacterAppearanceManager.Instance.GetEyeVariantCount;

        // Проверяем, существуют ли глаза с таким индексом
        if (CharacterAppearanceManager.Instance.HasEyeVariant(eyesIndex))
        {
            textEyes.text = $"{eyesIndex}";
        }
        else
        {
            textEyes.text = "Недоступно";
            Debug.LogWarning($"Eyes variant with index {eyesIndex} not found!");
        }
    }

    // Опционально: вызывать при старте, чтобы текст соответствовал начальному значению
    private void Start()
    {
        // Убедимся, что индекс в допустимых пределах
        int maxEyesIndex = CharacterAppearanceManager.Instance.GetEyeVariantCount;
        if (eyesIndex > maxEyesIndex)
        {
            eyesIndex = 1;
        }

        UpdateEyesText();
    }

    public void RandomEyes()
    {
        eyesIndex = Random.Range(1, CharacterAppearanceManager.Instance.GetEyeVariantCount);
        UpdateEyesText();
        characterCustomization.UpdateEyeIndex(eyesIndex);
    }
}