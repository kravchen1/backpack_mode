using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RotateCustomizationBody : MonoBehaviour
{
    public int bodyIndex = 2;
    public TextMeshProUGUI textBody;
    public RotateCustomizationCharacter characterCustomization;

    public void RotateLeft()
    {
        bodyIndex--;
        if (bodyIndex < 1)
            bodyIndex = 3;

        UpdateBodyText();
        characterCustomization.UpdateBodyIndex(bodyIndex);
    }

    public void RotateRight()
    {
        bodyIndex++;
        if (bodyIndex > 3)
            bodyIndex = 1;

        UpdateBodyText();
        characterCustomization.UpdateBodyIndex(bodyIndex);
    }

    private void UpdateBodyText()
    {
        switch (bodyIndex)
        {
            case 1:
                textBody.text = "Худой";
                break;
            case 2:
                textBody.text = "Стройный";
                break;
            case 3:
                textBody.text = "Толстый";
                break;
        }
    }

    // Опционально: вызывать при старте, чтобы текст соответствовал начальному значению
    private void Start()
    {
        UpdateBodyText();
    }

    public void RandomBody()
    {
        bodyIndex = Random.Range(1, 4);
        UpdateBodyText();
        characterCustomization.UpdateBodyIndex(bodyIndex);
    }
}