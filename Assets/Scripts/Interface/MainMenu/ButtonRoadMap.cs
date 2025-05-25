using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonRoadMap : MonoBehaviour
{
    [SerializeField] protected GameObject mainCanvas;
    [SerializeField] protected GameObject roadMapCanvas;
    [SerializeField] protected List<GameObject> pointDescription = new List<GameObject>();
    //[SerializeField] protected GameObject buttonClick;

    private UISlideAnimation slideAnimation;

    public float glowFadeDuration = 0.3f; // Длительность эффекта
    private string settingLanguage = "en";

    public void Start()
    {
        slideAnimation = roadMapCanvas.GetComponent<UISlideAnimation>();
    }
    public void OnMouseEnter()
    {
        transform.DOScale(1.05f, glowFadeDuration).SetEase(Ease.OutBack);
    }
    public void OnMouseExit()
    {
        transform.DOScale(1f, glowFadeDuration).SetEase(Ease.InOutSine);
    }
    //public void OnMouseDown()
   // {
        //buttonClick.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        //buttonClick.GetComponent<AudioSource>().Play();
        //gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
        //if (SceneManager.GetActiveScene().name == "Main")
        //{
            //Shake();
        //}
    //}

    //public void OnMouseUp()
    //{
        //gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        //ChangeActive();
    //}

    public void ChangeActive()
    {
        if (roadMapCanvas.activeSelf)
        {
            foreach (var description in pointDescription.Where(p => p.gameObject.activeSelf == true))
            {
                description.gameObject.SetActive(false);
            }
            slideAnimation.Hide();
        }
        else
        {
            slideAnimation.Show();
        }
    }

    public void Shake()
    {

        // Отменяем предыдущие анимации, чтобы не было наложений
        transform.DOKill();

        transform.DOShakePosition(0.5f, new Vector3(5f, 5f, 0), 10, 90).SetEase(Ease.OutBounce);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (roadMapCanvas.activeSelf && pointDescription.Where(p => p.gameObject.activeSelf == true).Count() == 0)
            {
                ChangeActive();
            }
        }
    }
}
