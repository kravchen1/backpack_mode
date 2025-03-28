using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown languageDropdown; // Ссылка на компонент Dropdown

    void Start()
    {
        
        // Инициализация Dropdown
        InitializeDropdown();

        // Добавляем обработчик события изменения выбора
        languageDropdown.onValueChanged.AddListener(SetLanguage);
    }

    // Инициализация Dropdown
    private void InitializeDropdown()
    {
        // Добавляем опции в Dropdown
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new System.Collections.Generic.List<string> { "English", "Русский", "繁體中文", "簡體中文" });

        if (!PlayerPrefs.HasKey("LanguageSettings"))
        {
            PlayerPrefs.SetString("LanguageSettings", "en");
        }

        // Загружаем сохранен
        string savedMode = PlayerPrefs.GetString("LanguageSettings"); 

        switch (savedMode)
        {
            case "en":
                languageDropdown.value = 0;
                break;
            case "ru":
                languageDropdown.value = 1;
                break;
            case "zh":
                languageDropdown.value = 2;
                break;
            case "zh_tw":
                languageDropdown.value = 3;
                break;
        }

    }

    // Метод для изменения режима экрана
    //0 - English
    //1 - Russian
    //2 - 繁體中文 kitayskiy tradic
    //3 - 簡體中文 kitayskuy uproshenn
    public void SetLanguage(int modeIndex)
    {
        switch(modeIndex)
        {
            case 0:
                PlayerPrefs.SetString("LanguageSettings", "en");
                break;
            case 1:
                PlayerPrefs.SetString("LanguageSettings", "ru");
                break;
            case 2:
                PlayerPrefs.SetString("LanguageSettings", "zh");
                break;
            case 3:
                PlayerPrefs.SetString("LanguageSettings", "zh_tw");
                break;
        }        
    }
}