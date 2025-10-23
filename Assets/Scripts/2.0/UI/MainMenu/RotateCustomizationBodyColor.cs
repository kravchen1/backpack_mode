using UnityEngine;
using UnityEngine.UI;

public class RotateCustomizationBodyColor : MonoBehaviour
{
    public int bodyColorIndex = 0; // Индекс цвета (0-31)
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
        bodyColorIndex--;
        if (bodyColorIndex < 0)
            bodyColorIndex = skinColors.Length - 1;

        UpdateBodyColor();
        characterCustomization.UpdateBodyColor(GetCurrentColor());
    }

    public void RotateRight()
    {
        bodyColorIndex++;
        if (bodyColorIndex >= skinColors.Length)
            bodyColorIndex = 0;

        UpdateBodyColor();
        characterCustomization.UpdateBodyColor(GetCurrentColor());
    }

    private void UpdateBodyColor()
    {
        if (colorImage != null && bodyColorIndex >= 0 && bodyColorIndex < skinColors.Length)
        {
            colorImage.color = skinColors[bodyColorIndex];
        }
    }

    // Опционально: вызывать при старте, чтобы цвет соответствовал начальному значению
    private void Start()
    {
        UpdateBodyColor();
    }

    // Метод для получения текущего цвета (может пригодиться)
    public Color GetCurrentColor()
    {
        if (bodyColorIndex >= 0 && bodyColorIndex < skinColors.Length)
        {
            return skinColors[bodyColorIndex];
        }
        return Color.white;
    }

    // Метод для установки цвета по индексу
    public void SetColorByIndex(int index)
    {
        if (index >= 0 && index < skinColors.Length)
        {
            bodyColorIndex = index;
            UpdateBodyColor();
            characterCustomization.UpdateBodyColor(GetCurrentColor());
        }
    }

    public void RandomBodyColor()
    {
        bodyColorIndex = Random.Range(0, skinColors.Length);
        UpdateBodyColor();
        characterCustomization.UpdateBodyColor(GetCurrentColor());
    }
}