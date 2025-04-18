using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonLoadGame : MonoBehaviour
{
    [SerializeField] protected GameObject buttonClick;
    [SerializeField] protected float timeToRotate = 0.1f;
    [SerializeField] protected float angleToRotate = 0.1f;
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
        transform.DORotate(new Vector3(0, 0, angleToRotate), timeToRotate)
           .SetEase(Ease.InOutSine).OnComplete(() =>
           {
               transform.DORotate(new Vector3(0, 0, 0), timeToRotate)
           .SetEase(Ease.InOutSine).OnComplete(() => {
               if (PlayerPrefs.HasKey("currentLocation"))
                   // SceneManager.LoadScene(PlayerPrefs.GetString("currentLocation"));
                   SceneLoader.Instance.LoadScene(PlayerPrefs.GetString("currentLocation"));
           });
               
           }
       );
        
        //else
        //    //SceneManager.LoadScene("GenerateMapInternumFortress1");
        //    SceneLoader.Instance.LoadScene("GenerateMapInternumFortress1");
    }

    public void updateText()
    {
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");

        string itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "LoadGame_button");

        gameObject.GetComponentInChildren<TextMeshPro>().text = itemText;
    }



}
