using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResolutionDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    private void Start()
    {
        InitializeDropdown();
    }

    private void InitializeDropdown()
    {
        resolutionDropdown.ClearOptions();

        // Доступные разрешения (можно добавить больше)
        string[] resolutions = {
            "1920 x 1080",   // 1080p
            "2560 x 1440",   // 1440p (QHD)
            "3840 x 2160"    // 4K
        };

        resolutionDropdown.AddOptions(new List<string>(resolutions));
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // Устанавливаем текущее разрешение по умолчанию
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i] == $"{Screen.width} x {Screen.height}")
            {
                resolutionDropdown.value = i;
                break;
            }
        }
    }

    public void SetResolution(int index)
    {
        string[] res = resolutionDropdown.options[index].text.Split('x');
        int width = int.Parse(res[0].Trim());
        int height = int.Parse(res[1].Trim());

        // Устанавливаем разрешение рендеринга
        Screen.SetResolution(width, height, Screen.fullScreen);

        // Принудительно обновляем фильтрацию текстур (если нужно)
        QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel());
    }
}