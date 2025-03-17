using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        // Получаем все доступные разрешения
        Resolution[] allResolutions = Screen.resolutions;

        // Фильтруем уникальные разрешения (игнорируя частоту обновления)
        var uniqueResolutions = allResolutions
            .GroupBy(r => new { r.width, r.height }) // Группируем по ширине и высоте
            .Select(g => g.First()) // Берем первое разрешение из каждой группы
            .ToList();

        // Фильтруем разрешения, оставляя только 16:9
        var filteredResolutions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < uniqueResolutions.Count; i++)
        {
            Resolution resolution = uniqueResolutions[i];

            // Проверяем соотношение сторон
            float aspectRatio = (float)resolution.width / resolution.height;
            if (Mathf.Approximately(aspectRatio, 16f / 9f)) // Примерно 1.777
            {
                // Добавляем разрешение в список
                string option = resolution.width + " x " + resolution.height;
                filteredResolutions.Add(option);

                // Проверяем, является ли текущее разрешение активным
                if (resolution.width == Screen.currentResolution.width &&
                    resolution.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = filteredResolutions.Count - 1;
                }
            }
        }

        // Добавляем отфильтрованные опции в Dropdown
        resolutionDropdown.AddOptions(filteredResolutions);

        // Устанавливаем текущее разрешение как выбранное
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Добавляем обработчик события изменения выбора
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    // Метод для изменения разрешения
    public void SetResolution(int resolutionIndex)
    {
        // Получаем все доступные разрешения
        Resolution[] allResolutions = Screen.resolutions;

        // Фильтруем уникальные разрешения (игнорируя частоту обновления)
        var uniqueResolutions = allResolutions
            .GroupBy(r => new { r.width, r.height }) // Группируем по ширине и высоте
            .Select(g => g.First()) // Берем первое разрешение из каждой группы
            .ToList();

        // Фильтруем разрешения, чтобы найти выбранное
        int count = 0;
        for (int i = 0; i < uniqueResolutions.Count; i++)
        {
            Resolution resolution = uniqueResolutions[i];

            // Проверяем соотношение сторон
            float aspectRatio = (float)resolution.width / resolution.height;
            if (Mathf.Approximately(aspectRatio, 16f / 9f)) // Примерно 1.777
            {
                if (count == resolutionIndex)
                {
                    // Устанавливаем выбранное разрешение
                    Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
                    break;
                }
                count++;
            }
        }
    }
}