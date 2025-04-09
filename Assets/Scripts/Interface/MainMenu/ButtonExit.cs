using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonExit : MonoBehaviour
{
    [SerializeField] protected GameObject buttonClick;

    private string settingLanguage = "en";

    public void Start()
    {
        updateText();
    }
    public void OnMouseDown()
    {
        buttonClick.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        buttonClick.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
    }

    public void OnMouseUp()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        Application.Quit();
    }

    public void updateText()
    {
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");

        string itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Exit_button");

        gameObject.GetComponentInChildren<TextMeshPro>().text = itemText;
    }

}
