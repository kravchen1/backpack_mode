using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class cell : MonoBehaviour
{
    private bool EmptyBag;
    private bool EmptyItem;

    public bool hide;
    public float SpeedActivation;

    public Text globaltest;
    private Item item;
    Color32 color;

    public bool IsContent = false;
    private void Start()
    {
        color = this.GetComponent<Image>().color;
        if (hide == true)
        {
           // this.SetA
            // transform.localScale = new Vector3(2.0f, 1.0f, 1.0f);
            //object.Equals(this, false);
             GetComponent<Image>().enabled = false;
           // GetComponent<Image>().color. = new Color32(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b,0) ;

        }
    }
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log(collision.name + " Trigger " + this.name);
        this.GetComponent<Image>().color = new Color32(255, 255, 225, 100);
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        globaltest.text = PlayerPrefs.GetInt("level").ToString();
        PlayerPrefs.Save();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.name + " AnTrigger " + this.name);
        this.GetComponent<Image>().color = color;
    }
    */
}
