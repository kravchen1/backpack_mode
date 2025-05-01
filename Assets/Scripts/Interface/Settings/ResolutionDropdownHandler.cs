using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ResolutionDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown; // Ссылка на компонент Dropdown

    void Start()
    {
        // Инициализация Dropdown
        InitializeDropdown();
    }

    // Инициализация Dropdown
    private void InitializeDropdown()
    {
        // Очищаем текущие опции Dropdown
        resolutionDropdown.ClearOptions();

        // Создаем список с фиксированными разрешениями
        var filteredResolutions = new List<string>();
        int defaultResolutionIndex = 1; // 1440p (2K) по умолчанию

        


        // Добавляем три фиксированных разрешения
        filteredResolutions.Add("1920 x 1080"); // Full HD
        filteredResolutions.Add("2560 x 1440"); // 2K (QHD)
        filteredResolutions.Add("3840 x 2160"); // 4K (UHD)


        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolution = filteredResolutions[i];
            // Проверяем, является ли текущее разрешение активным
            if (resolution == Screen.currentResolution.width + " x " + Screen.currentResolution.height)
            {
                defaultResolutionIndex = i;
                break;
            }
        }

        // Добавляем опции в Dropdown
        resolutionDropdown.AddOptions(filteredResolutions);

        // Устанавливаем 2K (1440p) как выбранное по умолчанию
        resolutionDropdown.value = defaultResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Добавляем обработчик события изменения выбора
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    // Метод для изменения разрешения
    public void SetResolution(int resolutionIndex)
    {
        switch (resolutionIndex)
        {
            case 0: // 1920x1080
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 1: // 2560x1440
                Screen.SetResolution(2560, 1440, Screen.fullScreen);
                break;
            case 2: // 3840x2160
                Screen.SetResolution(3840, 2160, Screen.fullScreen);
                break;
            default:
                // Если что-то пошло не так, устанавливаем 1440p
                Screen.SetResolution(2560, 1440, Screen.fullScreen);
                break;
        }
    }
}