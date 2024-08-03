using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class generateMapScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public GameObject carePoint;
    //private Color startColorCarePoint;
    //private LineRenderer2D lineRenderer;
    private GameObject boss;
    private GameObject start;
    private GameObject road;
    private GameObject battlePoint;
    private GameObject shopPoint;

    private carePosition carePosition = new carePosition();

    private float width, height;
    private Vector3 moveDownVector = new Vector3(0f, -25.0f, 0.0f);


    private GameObject bossTile;
    private GameObject startTile;

    private void Awake()
    {
        boss = Resources.Load<GameObject>("greenStandart(1)PointInterestBoss");
        start = Resources.Load<GameObject>("greenStandart(1)PointStart");
        road = Resources.Load<GameObject>("roadStandart");
        battlePoint = Resources.Load<GameObject>("greenStandart(1)PointInterestBattle"); 
        shopPoint = Resources.Load<GameObject>("greenStandart(1)PointInterestShop"); 




    }

    void generateBossTile()
    {//greenStandart(1)PointStart
        bossTile = Instantiate(boss, new Vector3(0, 0, 0), Quaternion.identity);
        bossTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
        bossTile.GetComponent<RectTransform>().localScale = new Vector2(0.25f, 0.25f);

        var x = width / 2 - (width / 2) % 25 - 25;
        var y = height - ((height % 25) + 25);

        bossTile.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        carePosition.position = new Vector3(x, y, 0);
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


    void generateTile(GameObject gameObject, Vector3 position, bool Place = false)
    {
        startTile = Instantiate(gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        startTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
        startTile.GetComponent<RectTransform>().localScale = new Vector2(0.25f, 0.25f);

        startTile.GetComponent<RectTransform>().anchoredPosition = position;
    }


    

    void moveAndGenerateRoadAndPointDown(int countRoad, Vector3 moveVector)
    {
        for (int i1 = 0; i1 < countRoad; i1++)
        {
            carePosition.position += moveVector;
            generateTile(road, carePosition.position);
        }
        carePosition.position += moveVector;

        int randomPlace = Random.Range(1, 3);

        switch (randomPlace)
        {
            case 1:
                generateTile(shopPoint, carePosition.position);
                break;
            case 2:
                generateTile(battlePoint, carePosition.position);
                break;
            default:
                Debug.Log("ты долбоёб?");
                break;
        }
    }

    void fullRoadGeneration() //пока ток вниз)
    {
        int randomCountRoads, randomLength;


        
        randomCountRoads = Random.Range(1, 2);


        for (int i = 0; i < randomCountRoads; i++)
        {
            randomLength = Random.Range(2, 4);
            if (i == 0)
            {
                //if (carePosition.position.x <= 0)
                    moveAndGenerateRoadAndPointDown(randomLength, moveDownVector);
            }
            
        }    
    }

    void Start()
    {
        var canvas = GetComponent<Canvas>();
        width = canvas.GetComponent<RectTransform>().rect.size.x;
        height = canvas.GetComponent<RectTransform>().rect.size.y;

        generateBossTile();
        generateStartTile();

        

        fullRoadGeneration();


    }

    // Update is called once per frame
    void Update()
    {
    }


}
