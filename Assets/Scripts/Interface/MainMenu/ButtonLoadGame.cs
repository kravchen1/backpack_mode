using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonLoadGame : MonoBehaviour
{
    public void OnMouseDown()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
    }

    public void OnMouseUp()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);

        if(PlayerPrefs.HasKey("currentLocation"))
            SceneManager.LoadScene(PlayerPrefs.GetString("currentLocation"));
        else
            SceneManager.LoadScene("GenerateMapInternumFortress1");

    }



}
