using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonLoadGame : MonoBehaviour
{
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

        if(PlayerPrefs.HasKey("currentLocation"))
           // SceneManager.LoadScene(PlayerPrefs.GetString("currentLocation"));
            SceneLoader.Instance.LoadScene(PlayerPrefs.GetString("currentLocation"));
        //else
        //    //SceneManager.LoadScene("GenerateMapInternumFortress1");
        //    SceneLoader.Instance.LoadScene("GenerateMapInternumFortress1");

    }



}
