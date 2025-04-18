using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonShowItems : MonoBehaviour
{
    private string settingLanguage = "en";
    public float glowFadeDuration = 0.3f; // Длительность эффекта
    public void Start()
    {
        updateText();
    }

    public void OnMouseEnter()
    {
        transform.DOScale(1.05f, glowFadeDuration).SetEase(Ease.OutBack);
    }
    public void OnMouseExit()
    {
        transform.DOScale(1f, glowFadeDuration).SetEase(Ease.InOutSine);
    }
    public void updateText()
    {
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");

        string itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "ShowItem_button");

        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = itemText;
    }
}
