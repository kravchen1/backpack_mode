using UnityEngine;
using UnityEngine.UI;

public class RotateCustomizationHeadColor : MonoBehaviour
{
    public int headColorIndex = 0; // Индекс цвета (0-31)
    public Image colorImage; // Image для отображения цвета
    public RotateCustomizationCharacter characterCustomization;

    // Массив из 32 цветов (оттенки кожи + основные цвета)
    private Color[] skinColors = new Color[]
    {
        // Белый (по умолчанию)
        Color.white,
        
        // Оттенки кожи (светлые)
        new Color(1f, 0.9f, 0.8f),    // Очень светлый
        new Color(0.98f, 0.85f, 0.75f), // Светлый
        new Color(0.95f, 0.8f, 0.7f),   // Светло-бежевый
        new Color(0.92f, 0.75f, 0.65f), // Бежевый
        new Color(0.9f, 0.7f, 0.6f),    // Средне-бежевый
        new Color(0.85f, 0.65f, 0.55f), // Тёмно-бежевый
        
        // Оттенки кожи (средние)
        new Color(0.8f, 0.6f, 0.5f),    // Светло-коричневый
        new Color(0.75f, 0.55f, 0.45f), // Коричневый
        new Color(0.7f, 0.5f, 0.4f),    // Средне-коричневый
        new Color(0.65f, 0.45f, 0.35f), // Тёмно-коричневый
        new Color(0.6f, 0.4f, 0.3f),    // Шоколадный
        new Color(0.55f, 0.35f, 0.25f), // Тёмный шоколад
        
        // Оттенки кожи (тёмные)
        new Color(0.5f, 0.3f, 0.2f),    // Тёмный
        new Color(0.45f, 0.25f, 0.15f), // Очень тёмный
        new Color(0.4f, 0.2f, 0.1f),    // Почти чёрный
        new Color(0.35f, 0.15f, 0.05f), // Глубокий тёмный
        
        // Красные оттенки
        new Color(1f, 0.6f, 0.6f),      // Светло-красный
        new Color(1f, 0.4f, 0.4f),      // Красный
        new Color(0.8f, 0.2f, 0.2f),    // Тёмно-красный
        new Color(0.6f, 0.1f, 0.1f),    // Бордовый
        
        // Зелёные оттенки
        new Color(0.6f, 1f, 0.6f),      // Светло-зелёный
        new Color(0.4f, 0.8f, 0.4f),    // Зелёный
        new Color(0.2f, 0.6f, 0.2f),    // Тёмно-зелёный
        new Color(0.1f, 0.4f, 0.1f),    // Изумрудный
        
        // Жёлтые оттенки
        new Color(1f, 1f, 0.6f),        // Светло-жёлтый
        new Color(1f, 1f, 0.4f),        // Жёлтый
        new Color(0.8f, 0.8f, 0.2f),    // Золотистый
        new Color(0.6f, 0.6f, 0.1f),    // Тёмно-жёлтый
        
        // Синие оттенки
        new Color(0.6f, 0.6f, 1f),      // Светло-синий
        new Color(0.4f, 0.4f, 1f),      // Синий
        new Color(0.2f, 0.2f, 0.8f),    // Тёмно-синий
        new Color(0.1f, 0.1f, 0.6f)     // Полуночный синий
    };

    public void RotateLeft()
    {
        headColorIndex--;
        if (headColorIndex < 0)
            headColorIndex = skinColors.Length - 1;

        UpdateHeadColor();
        characterCustomization.UpdateHeadColor(GetCurrentColor());
    }

    public void RotateRight()
    {
        headColorIndex++;
        if (headColorIndex >= skinColors.Length)
            headColorIndex = 0;

        UpdateHeadColor();
        characterCustomization.UpdateHeadColor(GetCurrentColor());
    }

    private void UpdateHeadColor()
    {
        if (colorImage != null && headColorIndex >= 0 && headColorIndex < skinColors.Length)
        {
            colorImage.color = skinColors[headColorIndex];
        }
    }

    // Опционально: вызывать при старте, чтобы цвет соответствовал начальному значению
    private void Start()
    {
        UpdateHeadColor();
    }

    // Метод для получения текущего цвета (может пригодиться)
    public Color GetCurrentColor()
    {
        if (headColorIndex >= 0 && headColorIndex < skinColors.Length)
        {
            return skinColors[headColorIndex];
        }
        return Color.white;
    }

    // Метод для установки цвета по индексу
    public void SetColorByIndex(int index)
    {
        if (index >= 0 && index < skinColors.Length)
        {
            headColorIndex = index;
            UpdateHeadColor();
            characterCustomization.UpdateHeadColor(GetCurrentColor());
        }
    }

    public void RandomHeadColor()
    {
        headColorIndex = Random.Range(0, skinColors.Length);
        UpdateHeadColor();
        characterCustomization.UpdateHairColor(GetCurrentColor());
    }
}