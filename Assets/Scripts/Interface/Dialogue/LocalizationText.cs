using UnityEngine;

[System.Serializable]
public class LocalizationText
{
    [TextArea(3, 10)]
    public string ruText;
    [TextArea(3, 10)]
    public string enText;
    [TextArea(3, 10)]
    public string zhText;
    [TextArea(3, 10)]
    public string zh_twText;

    public string GetText()
    {
        string languageSettings = PlayerPrefs.GetString("LanguageSettings");

        switch (languageSettings)
        {
            case "en":
                return enText;
            case "ru":
                return ruText;
            case "zh":
                return zhText;
            case "zh_tw":
                return zh_twText;
            default:
                return enText;
        }
    }

}