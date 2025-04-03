using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonShowItems : MonoBehaviour
{
    private string settingLanguage = "en";

    public void Start()
    {
        updateText();
    }


    public void updateText()
    {
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");

        string itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "ShowItem_button");

        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = itemText;
    }
}
