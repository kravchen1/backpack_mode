using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonNewGame : MonoBehaviour
{
    [SerializeField] protected GameObject mainCanvas;
    [SerializeField] protected GameObject chooseCharCanvas;
    [SerializeField] protected GameObject buttonClick;


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


    //public override void OnMouseUpAsButton()
    //{
    //    switch (gameObject.name)
    //    {
    //        case "Button_MainMenu":
    //            ChangeActive();
    //            break;
    //        case "Button_NewGame":
    //            Time.timeScale = 1f;
    //            ChangeActive();
    //            break;
    //    }
    //}

    protected void ChangeActive()
    {
        mainCanvas.SetActive(!mainCanvas.activeSelf);
        chooseCharCanvas.SetActive(!chooseCharCanvas.activeSelf);
    }
}
