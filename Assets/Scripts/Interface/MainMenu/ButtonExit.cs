using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonExit : MonoBehaviour
{
    [SerializeField] protected GameObject buttonClick;

    public float jumpHeight = 50f;    // Высота прыжка в пикселях
    public float jumpDuration = 0.5f; // Длительность в секундах
    private float staticY;
    private string settingLanguage = "en";
    public float glowFadeDuration = 0.3f; // Длительность эффекта
    public void Start()
    {
        staticY = transform.localPosition.y;
        updateText();
    }
    public void OnMouseDown()
    {
        buttonClick.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        buttonClick.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
        Jump();
    }
    public void OnMouseEnter()
    {
        transform.DOScale(1.05f, glowFadeDuration).SetEase(Ease.OutBack);
    }
    public void OnMouseExit()
    {
        transform.DOScale(1f, glowFadeDuration).SetEase(Ease.InOutSine);
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


    void Jump()
    {
        
        // Отменяем предыдущие анимации, чтобы не было наложений
        transform.DOKill();

        // Прыжок вверх и возврат с "пружинкой"
        transform.DOLocalMoveY(jumpHeight, jumpDuration / 2)
            .SetEase(Ease.OutQuad) // Плавный взлёт
            .OnComplete(() =>
            {
                // Падение вниз с эффектом "приземления"
                transform.DOLocalMoveY(staticY, jumpDuration / 2)
                    .SetEase(Ease.InOutBack); // Лёгкая "пружинка"
            });
    }
}
