using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSettings : MonoBehaviour
{
    [SerializeField] protected GameObject mainCanvas;
    [SerializeField] protected GameObject settingsCanvas;
    [SerializeField] protected GameObject buttonClick;

    public float jumpHeight = -260;    // Высота прыжка в пикселях
    public float jumpDuration = 0.2f; // Длительность в секундах
    public float glowFadeDuration = 0.3f; // Длительность эффекта
    private float staticY;
    private string settingLanguage = "en";

    public void Start()
    {
        staticY = transform.localPosition.y;
        updateText();
    }

    // При наведении курсора
    //public void OnMouseEnter()
    //{
    //    transform.DOScale(1.05f, glowFadeDuration).SetEase(Ease.OutBack);
    //}

    //// При уходе курсора
    //public void OnMouseExit()
    //{
    //    transform.DOScale(1f, glowFadeDuration).SetEase(Ease.InOutSine);
    //}
    public void OnMouseDown()
    {
        buttonClick.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        buttonClick.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
        if (SceneManager.GetActiveScene().name == "Main")
        {
            Shake();
        }
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
            //settingsCanvas.SetActive(!settingsCanvas.activeSelf);
            if (settingsCanvas.activeSelf)
            {
                settingsCanvas.GetComponent<UISlideAnimation>().Hide();
            }
            else
            {
                settingsCanvas.GetComponent<UISlideAnimation>().Show();
            }
        }
        //    //mainCanvas.SetActive(!mainCanvas.activeSelf);
        //    var childColliders = mainCanvas.GetComponentsInChildren<Collider2D>();
        //    foreach (var collider in childColliders)
        //    {
        //        Debug.Log("coliders");
        //        collider.enabled = !collider.enabled;
        //    }
        //}
        else
        {
            settingsCanvas.SetActive(!settingsCanvas.activeSelf);
            mainCanvas.GetComponent<CanvasGroup>().interactable = !mainCanvas.GetComponent<CanvasGroup>().interactable;
        }
    }

    void Shake()
    {

        // Отменяем предыдущие анимации, чтобы не было наложений
        transform.DOKill();

        transform.DOShakePosition(0.5f, new Vector3(5f, 5f, 0), 10, 90).SetEase(Ease.OutBounce);
    }

    public void updateText()
    {
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");

        string itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Settings_button");

        gameObject.GetComponentInChildren<TextMeshPro>().text = itemText;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsCanvas.activeSelf)
            {
                ChangeActive();
            }
        }
    }
}
