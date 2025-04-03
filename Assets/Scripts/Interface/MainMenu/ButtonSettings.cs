using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonSettings : MonoBehaviour
{
    [SerializeField] protected GameObject mainCanvas;
    [SerializeField] protected GameObject settingsCanvas;
    [SerializeField] protected GameObject buttonClick;

    private string settingLanguage = "en";

    public void Start()
    {
        //updateText();
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
        ChangeActive();
    }

    public virtual void ChangeActive()
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            settingsCanvas.SetActive(!settingsCanvas.activeSelf);
            //mainCanvas.SetActive(!mainCanvas.activeSelf);
            var childColliders = mainCanvas.GetComponentsInChildren<Collider2D>();
            foreach (var collider in childColliders)
            {
                collider.enabled = !collider.enabled;
            }
        }
        else
        {
            settingsCanvas.SetActive(!settingsCanvas.activeSelf);
            mainCanvas.GetComponent<CanvasGroup>().interactable = !mainCanvas.GetComponent<CanvasGroup>().interactable;
        }
    }

    public void updateText()
    {
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");

        string itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Settings_button");

        gameObject.GetComponentInChildren<TextMeshPro>().text = itemText;
    }
}
