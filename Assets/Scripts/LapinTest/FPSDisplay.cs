using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshPro fpsText; // Ссылка на UI текст для отображения FPS
    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f; // Вычисляем среднее время между кадрами

        // Обновляем текстовое поле с текущим FPS
        if (fpsText != null)
        {
            fpsText.text = "FPS: " + Mathf.Ceil(1.0f / deltaTime).ToString();
        }
    }
}
