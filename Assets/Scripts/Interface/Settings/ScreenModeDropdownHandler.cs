using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenModeDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown screenModeDropdown; // Ссылка на компонент Dropdown

    void Start()
    {
        // Инициализация Dropdown
        InitializeDropdown();

        // Добавляем обработчик события изменения выбора
        screenModeDropdown.onValueChanged.AddListener(SetScreenMode);
    }

    // Инициализация Dropdown
    public void InitializeDropdown()
    {
        // Добавляем опции в Dropdown
        screenModeDropdown.ClearOptions();
        string langsett = PlayerPrefs.GetString("LanguageSettings");

        switch (langsett)
        {
            case "en":
                screenModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "Windowed", "Fullscreen" });
                break;
            case "ru":
                screenModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "В окне", "Весь экран" });
                break;
            case "zh":
                screenModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "窗口化", "全屏" });
                break;
            case "zh_tw":
                screenModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "視窗模式", "全螢幕" });
                break;
        }
        

        // Загружаем сохраненный режим экрана
        int savedMode = PlayerPrefs.GetInt("ScreenMode", 1); // 1 - Fullscreen, 0 - Windowed
        screenModeDropdown.value = savedMode;

        // Применяем сохраненный режим экрана
        SetScreenMode(savedMode, false); // false - не сохраняем, так как это начальная загрузка
    }

    // Метод для изменения режима экрана
    public void SetScreenMode(int modeIndex)
    {
        SetScreenMode(modeIndex, true); // true - сохраняем настройки
    }

    private void SetScreenMode(int modeIndex, bool saveSettings)
    {
        // modeIndex: 0 - Windowed, 1 - Fullscreen
        bool isFullscreen = modeIndex == 1;

        // Устанавливаем разрешение в зависимости от режима
        if (!isFullscreen)
        {
            // Оконный режим с разрешением 1920x1080 (можно изменить)
            Screen.SetResolution(PlayerPrefs.GetInt("WindowedResoultionWidth",1920), PlayerPrefs.GetInt("WindowedResoultionHeight", 1080), false);
        }
        else
        {
            // Полноэкранный режим с текущим разрешением экрана
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }

        // Сохраняем выбранный режим, если нужно
        if (saveSettings)
        {
            PlayerPrefs.SetInt("ScreenMode", modeIndex);
            PlayerPrefs.Save();
        }
    }


    private void Update()
    {
        // Обработка изменения размера окна
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            // Здесь можно добавить логику для обновления UI или других элементов при изменении размера окна
            SetWindowResize();
        }
        else
        {
            if(PlayerPrefs.HasKey("WindowedResoultionWidth"))
            {
                PlayerPrefs.DeleteKey("WindowedResoultionWidth");
                PlayerPrefs.DeleteKey("WindowedResoultionHeight");
            }
        }    
    }

    private void SetWindowResize()
    {
        PlayerPrefs.SetInt("WindowedResoultionWidth", Screen.width);
        PlayerPrefs.SetInt("WindowedResoultionHeight", Screen.height);
    }
}