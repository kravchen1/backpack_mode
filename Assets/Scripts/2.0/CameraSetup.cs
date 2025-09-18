using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    public float targetOrthoSize = 8.5f;

    void Start()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Жестко выставляем ортографический режим и размер
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = targetOrthoSize;

            // Выводим в консоль текущие настройки для проверки
            Debug.Log("Camera is Orthographic: " + mainCamera.orthographic);
            Debug.Log("Camera Ortho Size is: " + mainCamera.orthographicSize);
        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }
    }
}