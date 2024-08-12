using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.IO;
using System.Text;
//using System;

public class generateMapScript : Map
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
    private GameObject greenStandart;

    private carePosition carePosition = new carePosition();

    private float width, height;
    private Vector3 moveDownVector = new Vector3(0f, -25.0f, 0.0f);




    private bool roadToBoss = false;

    private void Awake()
    {
        boss = Resources.Load<GameObject>("greenStandart(1)PointInterestBoss");
        start = Resources.Load<GameObject>("greenStandart(1)PointStart");
        road = Resources.Load<GameObject>("roadStandart");
        battlePoint = Resources.Load<GameObject>("greenStandart(1)PointInterestBattle");
        shopPoint = Resources.Load<GameObject>("greenStandart(1)PointInterestShop");
        greenStandart = Resources.Load<GameObject>("greenStandart");
    }

    void generateBossTile()
    {
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
        //Debug.Log(r);
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
        startPlayerPosition = new Vector3(x, y, 0); 
        Debug.Log(startTilePosition);
    }


    void generateTile(GameObject gameObject, Vector3 position, bool Place = false)
    {
        startTile = Instantiate(gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        startTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
        startTile.GetComponent<RectTransform>().localScale = new Vector2(0.25f, 0.25f);

        startTile.GetComponent<RectTransform>().anchoredPosition = position;
    }
    public class VectorStructure
    {
        public Vector2 movingVector;
        public bool canMove;
        public VectorStructure(Vector2 movingVector, bool canMove = false)
        {
            this.movingVector = movingVector;
            this.canMove = canMove;
        }
    }
    public class InterestPointStructure
    {
        public Vector2 position;
        public bool canBuild;
        public InterestPointStructure(Vector2 position, bool canBuild = true)
        {
            this.position = position;
            this.canBuild = canBuild;
        }
    }
    public void MapGenerator(InterestPointStructure carePoint)
    {
        if (carePoint.canBuild)
        {
            var canBuild = Random.Range(0, 2);
            if (canBuild == 0 && roadToBoss)
            {
                carePoint.canBuild = false;
                return;
            }
            Dictionary<int, VectorStructure> movingVectors = new Dictionary<int, VectorStructure>();
            movingVectors.Add(0, new VectorStructure(new Vector2(0, 25f)));
            movingVectors.Add(1, new VectorStructure(new Vector2(0, -25f)));
            movingVectors.Add(2, new VectorStructure(new Vector2(-25f, 0)));
            movingVectors.Add(3, new VectorStructure(new Vector2(25f, 0)));
            foreach (var vector in movingVectors)
            {
                if (tiles.Any(e => e.tilePosition == bossTile.GetComponent<RectTransform>().anchoredPosition + vector.Value.movingVector))
                {
                    roadToBoss = true;
                }
            }

            var bossPos = bossTile.GetComponent<RectTransform>().anchoredPosition;
            var newCarePoint = carePoint.position;
            if ((carePoint.position + movingVectors[0].movingVector).y >= height || tiles.Any(e => e.tilePosition == carePoint.position + movingVectors[0].movingVector))
                movingVectors[0].canMove = false;
            else
                movingVectors[0].canMove = true;
            if ((carePoint.position + movingVectors[1].movingVector).y < 0 || tiles.Any(e => e.tilePosition == carePoint.position + movingVectors[1].movingVector))
                movingVectors[1].canMove = false;
            else
                movingVectors[1].canMove = true;
            if ((carePoint.position + movingVectors[2].movingVector).x < 0 || tiles.Any(e => e.tilePosition == carePoint.position + movingVectors[2].movingVector))
                movingVectors[2].canMove = false;
            else
                movingVectors[2].canMove = true;
            if ((carePoint.position + movingVectors[3].movingVector).x >= width || tiles.Any(e => e.tilePosition == carePoint.position + movingVectors[3].movingVector))
                movingVectors[3].canMove = false;
            else
                movingVectors[3].canMove = true;
            Dictionary<int, Vector2> existVectors = new Dictionary<int, Vector2>();
            int vectorIndex = 0;
            foreach (var vector in movingVectors.Where(e => e.Value.canMove == true))
            {
                existVectors.Add(vectorIndex, vector.Value.movingVector);
                vectorIndex++;
            }
            var randomVector = Random.Range(0, existVectors.Count());
            if (existVectors.Count() == 0)
            {
                return;
            }
            else
            {
                var randomRoadCount = Random.Range(2, 4);
                for (int i = 0; i < randomRoadCount; i++)
                {
                    newCarePoint += existVectors[randomVector];
                    if (!tiles.Any(e => e.tilePosition == newCarePoint) && newCarePoint.x >= 0 && newCarePoint.y >= 0 && newCarePoint.x < width && newCarePoint.y < height - ((height % 25)))
                    {
                        tiles.Add(new Tile(road.name,newCarePoint));
                        generateTile(road, newCarePoint);
                    }
                    else
                        break;
                }
                int randomPlace = Random.Range(1, 3);
                newCarePoint += existVectors[randomVector];
                if (!tiles.Any(e => e.tilePosition == newCarePoint) && newCarePoint.x >= 0 && newCarePoint.y >= 0 && newCarePoint.x < width && newCarePoint.y < height - ((height % 25)))
                {
                    //pointInterestPoisitions.Add(newCarePoint);
                    pointInterestStructure.Add(new InterestPointStructure(newCarePoint));
                    switch (randomPlace)
                    {
                        case 1:
                            generateTile(shopPoint, newCarePoint);
                            tiles.Add(new Tile(shopPoint.name, newCarePoint));
                            break;
                        case 2:
                            generateTile(battlePoint, newCarePoint);
                            tiles.Add(new Tile(battlePoint.name, newCarePoint));
                            break;
                        default:
                            Debug.Log("ты долбоёб?");
                            break;
                    }
                }
            }
        }
        else
            return;
    }

    public void InitializateGenerationMap()
    {
        //using (FileStream fileStream = new FileStream("C:\\text.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
        //{
        //    fileStream.Seek(0, SeekOrigin.End);
        //    byte[] buffer = Encoding.Default.GetBytes("zalupa");
        //    fileStream.Write(buffer, 0, buffer.Length);
        //}
        generateBossTile();
        generateStartTile();
        var bossTilePosition = bossTile.GetComponent<RectTransform>().anchoredPosition;
        var startTilePosition = startTile.GetComponent<RectTransform>().anchoredPosition;

        tiles.Add(new Tile(boss.name, bossTilePosition));
        tiles.Add(new Tile(start.name, startTilePosition));

        var carePoint = startTilePosition;
        var countInterestPoint = Random.Range(5, 10);
        pointInterestPoisitions.Add(startTilePosition);
        pointInterestStructure.Add(new InterestPointStructure(startTilePosition));
        var index = pointInterestPoisitions.Count() - 1;
        var countCycle = 0;
        while (!roadToBoss)
        {
            countCycle++;
            var countPoints = pointInterestStructure.Count();
            for (int i = 0; i < countPoints; i++)
            {
                MapGenerator(pointInterestStructure[i]);
            }
            if (countCycle > 100)
            {
                Debug.Log("Мы закциклились епта");
                break;
            }
        }
        for (int i = 0; i < height; i += 25)
        {
            for (int j = 0; j < width; j += 25)
            {
                var vector = new Vector2(j, i);
                if (!tiles.Any(e => e.tilePosition == vector))
                {
                    tiles.Add(new Tile(greenStandart.name, vector));
                    generateTile(greenStandart, vector);
                }
            }
        }
    }

    void GenerateMapFromFile()
    {
        LoadData();
        foreach (var tile in mapData.tiles)
        {
            var careGameObject = Resources.Load<GameObject>(tile.tileName);
            var careTile = Instantiate(careGameObject, new Vector3(0, 0, 0), Quaternion.identity);
            careTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
            careTile.GetComponent<RectTransform>().localScale = new Vector2(0.25f, 0.25f);
            careTile.GetComponent<RectTransform>().anchoredPosition = tile.tilePosition;
        }
    }

    void Start()
    {
        var canvas = this.GetComponent<Canvas>();
        width = canvas.GetComponent<RectTransform>().rect.size.x;
        height = canvas.GetComponent<RectTransform>().rect.size.y;

        //var z = ScriptableObject.CreateInstance<Map>();
        if (!File.Exists(mapDataFilePath))
            InitializateGenerationMap();
        else
            GenerateMapFromFile();

    }

    // Update is called once per frame
    void Update()
    {

    }


}
