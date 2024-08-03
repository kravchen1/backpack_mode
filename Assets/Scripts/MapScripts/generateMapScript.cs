using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class generateMapScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public GameObject carePoint;
    //private Color startColorCarePoint;
    //private LineRenderer2D lineRenderer;
    private GameObject boss;
    private GameObject start;
    private float carePosition;

    private float width;
    private float height;

    private GameObject bossTile;
    private GameObject startTile;

    private void Awake()
    {
        boss = Resources.Load<GameObject>("greenStandart(1)PointInterestBoss");
        start = Resources.Load<GameObject>("greenStandart(1)PointStart");





    }

    void generateBossTile()
    {//greenStandart(1)PointStart
        bossTile = Instantiate(boss, new Vector3(0, 0, 0), Quaternion.identity);
        bossTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
        bossTile.GetComponent<RectTransform>().localScale = new Vector2(0.25f, 0.25f);

        var x = width / 2 - (width / 2) % 25 - 25;
        var y = height - ((height % 25) + 25);

        bossTile.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
    }

    void generateStartTile()
    {
        startTile = Instantiate(start, new Vector3(0, 0, 0), Quaternion.identity);
        startTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
        startTile.GetComponent<RectTransform>().localScale = new Vector2(0.25f, 0.25f);

        int r = Random.Range(1, 4);
        Debug.Log(r);
        float x = 0f, y = 0f;

        switch (r)
        {
            case 1:
                x = width / 2 - (width / 2) % 25 - 25;
                y = 0;
                break;
            case 2:
                x = 0;
                y = 0;
                break;
            case 3:
                x = width - ((width % 25) + 25);
                y = 0;
                break;
            default: 
                x = 0; 
                y = 0; 
                break;

        }

        startTile.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
    }


    void Start()
    {
        var canvas = GetComponent<Canvas>();
        width = canvas.GetComponent<RectTransform>().rect.size.x;
        height = canvas.GetComponent<RectTransform>().rect.size.y;

        generateBossTile();
        generateStartTile();



    }

    // Update is called once per frame
    void Update()
    {
    }


}
