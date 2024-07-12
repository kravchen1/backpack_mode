using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;





public class BackPackObject : MonoBehaviour
{
    bool MouseDown = false;
    bool IsShop = true;
    public int CountAnchours = 2;

    Color32 color;

    public Object[] cells;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private void Start()
    {
        color = this.GetComponent<Image>().color;
    }

    void OnMouseDown()
    {
        if (IsShop == true)
        { 
            MouseDown = true;
            transform.localScale = transform.localScale * 0.95f;

            Debug.Log(this.tag.ToString());

            if (this.tag.ToString() == "bag")
            {
                Debug.Log(this.tag.ToString());
                for (int i = 0; i < cells.Length; i++)
                {
                    Debug.Log(cells[i].GetComponent<cell>().IsContent);
                    if (cells[i].GetComponent<cell>().IsContent == false)
                        cells[i].GetComponent<Image>().enabled = true;
                }
            }
        }
    }

    void OnMouseUp()
    {
        if (IsShop == true)
        {
            MouseDown = false;
            transform.localScale = transform.localScale / 0.95f;
            
            if (this.tag.ToString() == "bag")
            {
                for (int i = 0; i < cells.Length; i++)
                {
                    if (cells[i].GetComponent<cell>().IsContent == false)
                        cells[i].GetComponent<Image>().enabled = false;
                }
            }
        }
    }

    void Update()
    {
        if (IsShop == true)
        {
            if (MouseDown == true)
            { 
                Vector3 Cursor = Input.mousePosition;

                Cursor.z = 0;
                Cursor = Camera.main.ScreenToWorldPoint(Cursor);

               // Debug.Log(Cursor);

                Cursor.z = 0;
                transform.SetPositionAndRotation(Cursor, Quaternion.identity);
                this.transform.transform.position = Cursor;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log(collision.name + " Trigger " + this.name);
        this.GetComponent<Image>().color = new Color32(255, 255, 225, 100);
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
       // globaltest.text = PlayerPrefs.GetInt("level").ToString();
        PlayerPrefs.Save();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.name + " AnTrigger " + this.name);
        this.GetComponent<Image>().color = color;
    }

}
