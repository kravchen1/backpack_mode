using UnityEngine;
using UnityEngine.UI;

public class RotateCustomizationEyesColor : MonoBehaviour
{
    public int eyesColorIndex = 0;
    public Image colorImage;
    public RotateCustomizationCharacter characterCustomization;

    // Цвета для глаз (натуральные + фантастические)
    private Color[] eyesColors = new Color[]
    {
        // Голубые оттенки
        new Color(0.7f, 0.8f, 0.95f),     // Светло-голубой
        new Color(0.5f, 0.7f, 0.9f),      // Небесно-голубой
        new Color(0.3f, 0.5f, 0.8f),      // Ярко-голубой
        new Color(0.2f, 0.4f, 0.7f),      // Стальной голубой
        new Color(0.1f, 0.3f, 0.6f),      // Тёмно-голубой
        
        // Зелёные оттенки
        new Color(0.6f, 0.8f, 0.5f),      // Светло-зелёный
        new Color(0.4f, 0.7f, 0.4f),      // Изумрудный
        new Color(0.3f, 0.6f, 0.3f),      // Ярко-зелёный
        new Color(0.2f, 0.5f, 0.2f),      // Лесной зелёный
        new Color(0.15f, 0.4f, 0.15f),    // Тёмно-зелёный
        
        // Карие оттенки
        new Color(0.8f, 0.6f, 0.4f),      // Светло-карий
        new Color(0.7f, 0.5f, 0.3f),      // Медовый
        new Color(0.6f, 0.4f, 0.2f),      // Ореховый
        new Color(0.5f, 0.35f, 0.15f),    // Тёмно-карий
        new Color(0.4f, 0.25f, 0.1f),     // Шоколадный
        
        // Серые оттенки
        new Color(0.8f, 0.8f, 0.85f),     // Светло-серый
        new Color(0.6f, 0.6f, 0.7f),      // Стальной серый
        new Color(0.4f, 0.4f, 0.5f),      // Дымчатый
        new Color(0.3f, 0.3f, 0.4f),      // Грозовой серый
        
        // Янтарные/специальные
        new Color(0.9f, 0.7f, 0.3f),      // Янтарный
        new Color(0.8f, 0.5f, 0.2f),      // Медный
        new Color(0.7f, 0.8f, 0.9f),      // Ледяной голубой
        new Color(0.9f, 0.9f, 0.7f),      // Жёлто-зелёный
        
        // Фантастические цвета
        new Color(1f, 0.3f, 0.3f),        // Красный
        new Color(1f, 0.5f, 0.1f),        // Оранжевый
        new Color(0.8f, 0.2f, 0.8f),      // Фиолетовый
        new Color(0.9f, 0.9f, 0.2f),      // Жёлтый
        new Color(0.3f, 0.9f, 0.9f),      // Бирюзовый
        new Color(0.9f, 0.4f, 0.9f),      // Розовый
        new Color(0.2f, 0.9f, 0.5f),      // Изумрудный
        new Color(1f, 1f, 1f),            // Белый (альбинос)
        new Color(0.05f, 0.05f, 0.1f)     // Чёрный
    };

    public void RotateLeft()
    {
        eyesColorIndex--;
        if (eyesColorIndex < 0)
            eyesColorIndex = eyesColors.Length - 1;

        UpdateEyesColor();
        characterCustomization.UpdateEyeColor(GetCurrentColor());
    }

    public void RotateRight()
    {
        eyesColorIndex++;
        if (eyesColorIndex >= eyesColors.Length)
            eyesColorIndex = 0;

        UpdateEyesColor();
        characterCustomization.UpdateEyeColor(GetCurrentColor());
    }

    private void UpdateEyesColor()
    {
        if (colorImage != null && eyesColorIndex >= 0 && eyesColorIndex < eyesColors.Length)
        {
            colorImage.color = eyesColors[eyesColorIndex];
        }
    }

    private void Start()
    {
        UpdateEyesColor();
    }

    public Color GetCurrentColor()
    {
        if (eyesColorIndex >= 0 && eyesColorIndex < eyesColors.Length)
        {
            return eyesColors[eyesColorIndex];
        }
        return Color.blue;
    }

    public void SetColorByIndex(int index)
    {
        if (index >= 0 && index < eyesColors.Length)
        {
            eyesColorIndex = index;
            UpdateEyesColor();
            characterCustomization.UpdateEyeColor(GetCurrentColor());
        }
    }

    public void RandomEyesColor()
    {
        eyesColorIndex = Random.Range(0, eyesColors.Length);
        UpdateEyesColor();
        characterCustomization.UpdateEyeColor(GetCurrentColor());
    }
}