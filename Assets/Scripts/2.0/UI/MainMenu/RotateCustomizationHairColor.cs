using UnityEngine;
using UnityEngine.UI;

public class RotateCustomizationHairColor : MonoBehaviour
{
    public int hairColorIndex = 0;
    public Image colorImage;
    public RotateCustomizationCharacter characterCustomization;

    // Цвета для причесок (натуральные + креативные)
    private Color[] hairColors = new Color[]
    {
        // Натуральные оттенки блонд
        new Color(0.98f, 0.95f, 0.82f),   // Платиновый блонд
        new Color(0.95f, 0.88f, 0.65f),   // Золотистый блонд
        new Color(0.85f, 0.75f, 0.55f),   // Пепельный блонд
        new Color(0.78f, 0.65f, 0.45f),   // Медовый блонд
        
        // Рыжие оттенки
        new Color(0.85f, 0.45f, 0.25f),   // Медно-рыжий
        new Color(0.75f, 0.35f, 0.15f),   // Ярко-рыжий
        new Color(0.65f, 0.28f, 0.12f),   // Тёмно-рыжий
        new Color(0.55f, 0.22f, 0.08f),   // Коричнево-рыжий
        
        // Коричневые оттенки
        new Color(0.65f, 0.45f, 0.3f),    // Светло-коричневый
        new Color(0.55f, 0.35f, 0.2f),    // Каштановый
        new Color(0.45f, 0.28f, 0.15f),   // Шоколадный
        new Color(0.35f, 0.22f, 0.12f),   // Тёмно-коричневый
        
        // Тёмные оттенки
        new Color(0.25f, 0.18f, 0.1f),    // Чёрно-коричневый
        new Color(0.15f, 0.1f, 0.05f),    // Иссиня-чёрный
        new Color(0.08f, 0.05f, 0.02f),   // Натуральный чёрный
        
        // Седые оттенки
        new Color(0.9f, 0.9f, 0.9f),      // Светло-седой
        new Color(0.7f, 0.7f, 0.7f),      // Стальной серый
        new Color(0.5f, 0.5f, 0.5f),      // Тёмно-седой
        
        // Яркие/креативные цвета
        new Color(1f, 0.2f, 0.2f),        // Ярко-красный
        new Color(0.8f, 0.1f, 0.4f),      // Фуксия
        new Color(0.6f, 0.1f, 0.8f),      // Фиолетовый
        new Color(0.3f, 0.2f, 0.9f),      // Синий
        new Color(0.1f, 0.6f, 0.9f),      // Голубой
        new Color(0.2f, 0.8f, 0.4f),      // Изумрудно-зелёный
        new Color(0.9f, 0.8f, 0.1f),      // Золотой
        new Color(1f, 0.6f, 0.1f),        // Оранжевый
        new Color(1f, 1f, 0.3f),          // Неоново-жёлтый
        new Color(0.8f, 1f, 0.3f),        // Лаймовый
        new Color(0.3f, 1f, 0.8f),        // Бирюзовый
        new Color(1f, 0.4f, 0.8f),        // Розовый
        new Color(0.5f, 0.1f, 0.5f),      // Пурпурный
        new Color(0.9f, 0.9f, 1f)         // Белоснежный
    };

    public void RotateLeft()
    {
        hairColorIndex--;
        if (hairColorIndex < 0)
            hairColorIndex = hairColors.Length - 1;

        UpdateHairColor();
        characterCustomization.UpdateHairColor(GetCurrentColor());
    }

    public void RotateRight()
    {
        hairColorIndex++;
        if (hairColorIndex >= hairColors.Length)
            hairColorIndex = 0;

        UpdateHairColor();
        characterCustomization.UpdateHairColor(GetCurrentColor());
    }

    private void UpdateHairColor()
    {
        if (colorImage != null && hairColorIndex >= 0 && hairColorIndex < hairColors.Length)
        {
            colorImage.color = hairColors[hairColorIndex];
        }
    }

    private void Start()
    {
        UpdateHairColor();
    }

    public Color GetCurrentColor()
    {
        if (hairColorIndex >= 0 && hairColorIndex < hairColors.Length)
        {
            return hairColors[hairColorIndex];
        }
        return Color.black;
    }

    public void SetColorByIndex(int index)
    {
        if (index >= 0 && index < hairColors.Length)
        {
            hairColorIndex = index;
            UpdateHairColor();
            characterCustomization.UpdateHairColor(GetCurrentColor());
        }
    }

    public void RandomHairColor()
    {
        hairColorIndex = Random.Range(0, hairColors.Length);
        UpdateHairColor();
        characterCustomization.UpdateHairColor(GetCurrentColor());
    }
}