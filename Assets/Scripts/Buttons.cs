using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Xml.Linq;

public class Buttons : MonoBehaviour
{
    public int scenary;
    //1 - new game
    //2 - load game
    public Object scene1;


    void OnMouseDown()
    {
        transform.localScale = new Vector2(0.35f, 0.35f);

        if(scenary == 3)
        {
            GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
        }

        SceneManager.LoadScene(scene1.name);
    }

    void OnMouseUp()
    {
        transform.localScale = new Vector2(0.3f, 0.3f);
    }
}

