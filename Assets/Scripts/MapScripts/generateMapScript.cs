using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.IO;
using System.Text;

public class generateMapScript : Map
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public GameObject carePoint;
    //private Color startColorCarePoint;
    //private LineRenderer2D lineRenderer;
    private GameObject endPoint;
    private GameObject start;
    [HideInInspector] public GameObject road;
    private GameObject battlePoint1, battlePoint2, battlePoint3, battlePoint4, battlePoint5;
    private GameObject shopPoint;
    private GameObject greenStandart;
    private GameObject treeStandart1;
    private GameObject treeStandart2;
    private GameObject treeStandart3;

    private GameObject playerPrefab;
    private GameObject fountainPrefab;
    private GameObject ChestOfFortune;
    private GameObject Forge;
    private carePosition carePosition = new carePosition();

    private float width, height;

    private float stepSize = 100;
    private float imageScale = 1;


    private bool roadToBoss = false;

    private void Awake()
    {
        
    }

    void InitializePrefabs()
    {
        if (PlayerPrefs.GetInt("mapLevel") == 7)
            endPoint = Resources.Load<GameObject>("greenStandart(1)PointInterestBoss");
        else
            endPoint = Resources.Load<GameObject>("greenStandart(1)PointInterestPortal");
        start = Resources.Load<GameObject>("greenStandart(1)PointStart");
        road = Resources.Load<GameObject>("roadStandart");
        battlePoint1 = Resources.Load<GameObject>("greenStandart(1)PointInterestBattle1");
        battlePoint2 = Resources.Load<GameObject>("greenStandart(1)PointInterestBattle2");
        battlePoint3 = Resources.Load<GameObject>("greenStandart(1)PointInterestBattle3");
        battlePoint4 = Resources.Load<GameObject>("greenStandart(1)PointInterestBattle4");
        battlePoint5 = Resources.Load<GameObject>("greenStandart(1)PointInterestBattle5");
        shopPoint = Resources.Load<GameObject>("greenStandart(1)PointInterestShop");
        greenStandart = Resources.Load<GameObject>("greenStandart");
        treeStandart1 = Resources.Load<GameObject>("treeStandart1");
        treeStandart2 = Resources.Load<GameObject>("treeStandart2");
        treeStandart3 = Resources.Load<GameObject>("treeStandart3");
        playerPrefab = Resources.Load<GameObject>(PlayerPrefs.GetString("characterClass"));
        fountainPrefab = Resources.Load<GameObject>("greenStandart(1)Fountain");
        ChestOfFortune = Resources.Load<GameObject>("greenStandart(1)ChestOfFortune");
        Forge = Resources.Load<GameObject>("greenStandart(1)Forge");

    }
    void generateEndPointTile(float x, float y)
    {
        endPointTile = Instantiate(endPoint, new Vector3(0, 0, 0), Quaternion.identity);
        endPointTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
        endPointTile.GetComponent<RectTransform>().localScale = new Vector2(imageScale, imageScale);

        endPointTile.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        carePosition.position = new Vector3(x, y, 0);
    }
    void generateStartTile()
    {
        startTile = Instantiate(start, new Vector3(0, 0, 0), Quaternion.identity);
        startTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
        startTile.GetComponent<RectTransform>().localScale = new Vector2(imageScale, imageScale);

        int r = Random.Range(2, 4);
        //Debug.Log(r);
        float x = 0f, y = 0f;

        switch (r)
        {
            //case 1:
            //    x = width / 2 - (width / 2) % stepSize - stepSize;
            //    y = 0;
            //    break;
            case 2:
                x = 0;
                y = 0;
                generateEndPointTile(width - ((width % stepSize) + stepSize), height - ((height % stepSize) + stepSize));
                break;
            case 3:
                x = width - ((width % stepSize) + stepSize);
                y = 0;
                generateEndPointTile(0, height - ((height % stepSize) + stepSize));
                break;
            default:
                x = 0;
                y = 0;
                break;

        }

        startTile.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        startPlayerPosition = new Vector3(x, y, 0);
        player = Instantiate(playerPrefab, startPlayerPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
        player.GetComponent<RectTransform>().anchoredPosition = startPlayerPosition;
    }

    public void GenerateStartAndBossTiles()
    {
        generateStartTile();
       // generateEndPointTile();
    }


    public void generateTile(GameObject gameObject, Vector3 position, bool Place = false)
    {
        startTile = Instantiate(gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        startTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
        startTile.GetComponent<RectTransform>().localScale = new Vector2(imageScale, imageScale);

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


    public void GenerateSquare(float width, float heigth, bool isFirstSquare = true)
    {
        var exitPos = Random.Range(1, (int)(width / stepSize)-1);
        for (int i = 0; i <= width; i += (int)stepSize)
        {
            var position = new Vector2(i, 0);
            if (isFirstSquare)
            {
                generateTile(greenStandart, position);
                tiles.Add(new Tile(greenStandart.name, position));
                generateTile(treeStandart1, position);
                tiles.Add(new Tile(treeStandart1.name, position));

            }
            else
            {
                if (!tiles.Any(e => e.tilePosition == position))
                {
                    if (i != exitPos * stepSize)
                    {
                        generateTile(greenStandart, position);
                        tiles.Add(new Tile(greenStandart.name, position));
                        generateTile(treeStandart1, position);
                        tiles.Add(new Tile(treeStandart1.name, position));
                    }
                }
            }
        }
        for (int i = 0; i <= width; i += (int)stepSize)
        {
            var position = new Vector2(i, heigth);
            if (isFirstSquare)
            {
                generateTile(greenStandart, position);
                tiles.Add(new Tile(greenStandart.name, position));
                generateTile(treeStandart1, position);
                tiles.Add(new Tile(treeStandart1.name, position));
            }
            else
            {
                if (!tiles.Any(e => e.tilePosition == position))
                {
                    if (i != exitPos * stepSize)
                    {
                        generateTile(greenStandart, position);
                        tiles.Add(new Tile(greenStandart.name, position));
                        generateTile(treeStandart1, position);
                        tiles.Add(new Tile(treeStandart1.name, position)); ;
                    }
                }
            }
        }
        exitPos = Random.Range(1, (int)(heigth / stepSize)-1);
        for (int i = 0; i <= heigth; i += (int)stepSize)
        {
            var position = new Vector2(0, i);
            if (isFirstSquare)
            {
                generateTile(greenStandart, position);
                tiles.Add(new Tile(greenStandart.name, position));
                generateTile(treeStandart1, position);
                tiles.Add(new Tile(treeStandart1.name, position));
            }
            else
            {
                if (!tiles.Any(e => e.tilePosition == position))
                {
                    if (i != exitPos * stepSize)
                    {
                        generateTile(greenStandart, position);
                        tiles.Add(new Tile(greenStandart.name, position));
                        generateTile(treeStandart1, position);
                        tiles.Add(new Tile(treeStandart1.name, position));
                    }
                }
            }
        }
        for (int i = 0; i <= heigth; i += (int)stepSize)
        {
            var position = new Vector2(width, i);
            if (isFirstSquare)
            {
                generateTile(greenStandart, position);
                tiles.Add(new Tile(greenStandart.name, position));
                generateTile(treeStandart1, position);
                tiles.Add(new Tile(treeStandart1.name, position));
            }
            else
            {
                if (!tiles.Any(e => e.tilePosition == position))
                {
                    if (i != exitPos * stepSize)
                    {
                        generateTile(greenStandart, position);
                        tiles.Add(new Tile(greenStandart.name, position));
                        generateTile(treeStandart1, position);
                        tiles.Add(new Tile(treeStandart1.name, position));
                    }
                }
            }
        }
    }

    public void MapGenerator()
    {
        var randomSqaureCount = Random.Range(3, 7);
        var newWidth = width - (width % stepSize);
        var newHeigth = height - (height % stepSize);
        GenerateSquare(newWidth, newHeigth);
        for (int i = 0; i < randomSqaureCount; i++)
        {
            var randomSide = Random.Range(0, 2);
            switch(randomSide)
            {
                case 0:
                    newWidth = Random.Range(4, (int)(newWidth / stepSize) -1) * stepSize;
                    break;
                case 1:
                    newHeigth = Random.Range(4, (int)(newHeigth / stepSize) - 1) * stepSize;
                    break;
            }
            GenerateSquare(newWidth, newHeigth, false);
            //newHeigth = newHeigth / 2;
        }
        //for (int i = 0; i < width; i += (int)stepSize)
        //{
        //    if (!tiles.Any(e => e.tilePosition == newCarePoint) && newCarePoint.x >= 0 && newCarePoint.y >= 0 && newCarePoint.x < width && newCarePoint.y < height - ((height % stepSize)))
        //    {
        //        tiles.Add(new Tile(road.name, newCarePoint));
        //        generateTile(road, newCarePoint);
        //    }
        //    else
        //        break;
        //}
        //int randomPlace = Random.Range(1, 10);
        //int randomBattlePoint = 0;
        //newCarePoint += existVectors[randomVector];
        //if (!tiles.Any(e => e.tilePosition == newCarePoint) && newCarePoint.x >= 0 && newCarePoint.y >= 0 && newCarePoint.x < width && newCarePoint.y < height - ((height % stepSize)))
        //{
        //    //pointInterestPoisitions.Add(newCarePoint);
        //    pointInterestStructure.Add(new InterestPointStructure(newCarePoint));
        //    switch (randomPlace)
        //    {
        //        case 1:
        //            generateTile(fountainPrefab, newCarePoint);
        //            tiles.Add(new Tile(fountainPrefab.name, newCarePoint));
        //            break;
        //        case 2:
        //            generateTile(ChestOfFortune, newCarePoint);
        //            tiles.Add(new Tile(ChestOfFortune.name, newCarePoint));
        //            break;
        //        //case 3:
        //        //    generateTile(Forge, newCarePoint);
        //        //    tiles.Add(new Tile(Forge.name, newCarePoint));
        //        //    break;
        //        default:
        //            randomBattlePoint = Random.Range(1, 6);
        //            switch (randomBattlePoint)
        //            {
        //                case 1:
        //                    generateTile(battlePoint1, newCarePoint);
        //                    tiles.Add(new Tile(battlePoint1.name, newCarePoint));
        //                    break;
        //                case 2:
        //                    generateTile(battlePoint2, newCarePoint);
        //                    tiles.Add(new Tile(battlePoint2.name, newCarePoint));
        //                    break;
        //                case 3:
        //                    generateTile(battlePoint3, newCarePoint);
        //                    tiles.Add(new Tile(battlePoint3.name, newCarePoint));
        //                    break;
        //                case 4:
        //                    generateTile(battlePoint4, newCarePoint);
        //                    tiles.Add(new Tile(battlePoint4.name, newCarePoint));
        //                    break;
        //                case 5:
        //                    generateTile(battlePoint5, newCarePoint);
        //                    tiles.Add(new Tile(battlePoint5.name, newCarePoint));
        //                    break;
        //            }
        //            break;
        //    }
        //}
    }
    public void InitializateGenerationMap()
    {
        //GenerateStartAndBossTiles();
           
        //var endPointTilePosition = endPointTile.GetComponent<RectTransform>().anchoredPosition;
        //var startTilePosition = startTile.GetComponent<RectTransform>().anchoredPosition;

        //tiles.Add(new Tile(endPoint.name, endPointTilePosition));
        //tiles.Add(new Tile(start.name, startTilePosition));

        var carePoint = startTilePosition;
        var countInterestPoint = Random.Range(5, 10);
        pointInterestPoisitions.Add(startTilePosition);
        pointInterestStructure.Add(new InterestPointStructure(startTilePosition));
        var index = pointInterestPoisitions.Count() - 1;
        var countCycle = 0;
        for (int i = 0; i < height; i += (int)stepSize)
        {
            for (int j = 0; j < width; j += (int)stepSize)
            {
                var vector = new Vector2(j, i);
                if (!tiles.Any(e => e.tilePosition == vector))
                {
                    tiles.Add(new Tile(greenStandart.name, vector));
                    generateTile(greenStandart, vector);
                }
            }
        }
        GenerateTileOnTile();
    }
    void GenerateTileOnTile()
    {
        int randomTree = 0;
        List<Tile> tilesTree = new List<Tile>();
        foreach (var tile in tiles.Where(e => e.tileName == "greenStandart"))
        {
            randomTree = Random.Range(0, 3);
            switch(randomTree)
            {
                case 0:
                    tilesTree.Add(new Tile(treeStandart1.name, tile.tilePosition));
                    generateTile(treeStandart1, tile.tilePosition);
                    break;
                case 1:
                    tilesTree.Add(new Tile(treeStandart2.name, tile.tilePosition));
                    generateTile(treeStandart2, tile.tilePosition);
                    break;
                case 2:
                    tilesTree.Add(new Tile(treeStandart3.name, tile.tilePosition));
                    generateTile(treeStandart3, tile.tilePosition);
                    break;
            }          
        }
        foreach (var tile in tilesTree)
        {
            tiles.Add(tile);
        }       
    }
    void GenerateMapFromFile()
    {
        LoadData("Assets/Saves/mapData.json");
        foreach (var tile in mapData.tiles)
        {
            var careGameObject = Resources.Load<GameObject>(tile.tileName);
            var careTile = Instantiate(careGameObject, new Vector3(0, 0, 0), Quaternion.identity);
            careTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
            careTile.GetComponent<RectTransform>().localScale = new Vector2(imageScale, imageScale);
            careTile.GetComponent<RectTransform>().anchoredPosition = tile.tilePosition;
            tiles.Add(tile);
        }
        //Debug.Log("PlayerPos1:" + mapData.playerPosition);
        player = Instantiate(playerPrefab, mapData.playerPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
        player.GetComponent<RectTransform>().anchoredPosition = mapData.playerPosition;
        //Debug.Log("PlayerPos1:" + player.transform.position);
        //startPlayerPosition = mapData.playerPosition;
    }
    
    void Start()
    {
        var canvas = this.GetComponent<Canvas>();
        width = canvas.GetComponent<RectTransform>().rect.size.x;
        height = canvas.GetComponent<RectTransform>().rect.size.y;

        //var z = ScriptableObject.CreateInstance<Map>();
        InitializePrefabs();
        if (!File.Exists("Assets/Saves/mapData.json"))
        {
            if(!PlayerPrefs.HasKey("mapLevel"))
                PlayerPrefs.SetInt("mapLevel", 1);
            else
                PlayerPrefs.SetInt("mapLevel", PlayerPrefs.GetInt("mapLevel")+1);
            //InitializePrefabs();
            //InitializateGenerationMap();
            //GenerateSquare(width - ((width % stepSize)), height - ((height % stepSize)));
            MapGenerator();
        }
        else
            GenerateMapFromFile();

    }

    // Update is called once per frame
    void Update()
    {

    }


}
