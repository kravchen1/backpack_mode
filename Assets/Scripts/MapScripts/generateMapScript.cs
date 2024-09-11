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
    private GameObject road;
    private GameObject battlePoint1, battlePoint2, battlePoint3, battlePoint4, battlePoint5;
    private GameObject shopPoint;
    private GameObject greenStandart;
    private GameObject treeStandart1;
    private GameObject treeStandart2;
    private GameObject treeStandart3;

    private GameObject playerPrefab;
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

    }
    void generateEndPointTile()
    {
        endPointTile = Instantiate(endPoint, new Vector3(0, 0, 0), Quaternion.identity);
        endPointTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
        endPointTile.GetComponent<RectTransform>().localScale = new Vector2(imageScale, imageScale);

        var x = width / 2 - (width / 2) % stepSize - stepSize;
        var y = height - ((height % stepSize) + stepSize);

        endPointTile.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        carePosition.position = new Vector3(x, y, 0);
    }
    void generateStartTile()
    {
        startTile = Instantiate(start, new Vector3(0, 0, 0), Quaternion.identity);
        startTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
        startTile.GetComponent<RectTransform>().localScale = new Vector2(imageScale, imageScale);

        int r = Random.Range(1, 4);
        //Debug.Log(r);
        float x = 0f, y = 0f;

        switch (r)
        {
            case 1:
                x = width / 2 - (width / 2) % stepSize - stepSize;
                y = 0;
                break;
            case 2:
                x = 0;
                y = 0;
                break;
            case 3:
                x = width - ((width % stepSize) + stepSize);
                y = 0;
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
        //Debug.Log(startPlayerPosition);
    }
    void generateTile(GameObject gameObject, Vector3 position, bool Place = false)
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
            movingVectors.Add(0, new VectorStructure(new Vector2(0, stepSize)));
            movingVectors.Add(1, new VectorStructure(new Vector2(0, -stepSize)));
            movingVectors.Add(2, new VectorStructure(new Vector2(-stepSize, 0)));
            movingVectors.Add(3, new VectorStructure(new Vector2(stepSize, 0)));
            foreach (var vector in movingVectors)
            {
                if (tiles.Any(e => e.tilePosition == endPointTile.GetComponent<RectTransform>().anchoredPosition + vector.Value.movingVector))
                {
                    roadToBoss = true;
                }
            }

            var bossPos = endPointTile.GetComponent<RectTransform>().anchoredPosition;
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
                var randomRoadCount = Random.Range(1, 3);
                for (int i = 0; i < randomRoadCount; i++)
                {
                    newCarePoint += existVectors[randomVector];
                    if (!tiles.Any(e => e.tilePosition == newCarePoint) && newCarePoint.x >= 0 && newCarePoint.y >= 0 && newCarePoint.x < width && newCarePoint.y < height - ((height % stepSize)))
                    {
                        tiles.Add(new Tile(road.name,newCarePoint));
                        generateTile(road, newCarePoint);
                    }
                    else
                        break;
                }
                int randomPlace = Random.Range(1, 10);
                int randomBattlePoint = 0;
                newCarePoint += existVectors[randomVector];
                if (!tiles.Any(e => e.tilePosition == newCarePoint) && newCarePoint.x >= 0 && newCarePoint.y >= 0 && newCarePoint.x < width && newCarePoint.y < height - ((height % stepSize)))
                {
                    //pointInterestPoisitions.Add(newCarePoint);
                    pointInterestStructure.Add(new InterestPointStructure(newCarePoint));
                    switch (randomPlace)
                    {
                        case 1:
                            generateTile(shopPoint, newCarePoint);
                            tiles.Add(new Tile(shopPoint.name, newCarePoint));
                            break;
                        //case 2:
                        //    generateTile(battlePoint, newCarePoint);
                        //    tiles.Add(new Tile(battlePoint.name, newCarePoint));
                        //    break;
                        default:
                            randomBattlePoint = Random.Range(1, 6);
                            switch(randomBattlePoint)
                            {
                                case 1:
                                    generateTile(battlePoint1, newCarePoint);
                                    tiles.Add(new Tile(battlePoint1.name, newCarePoint));
                                    break;
                                case 2:
                                    generateTile(battlePoint2, newCarePoint);
                                    tiles.Add(new Tile(battlePoint2.name, newCarePoint));
                                    break;
                                case 3:
                                    generateTile(battlePoint3, newCarePoint);
                                    tiles.Add(new Tile(battlePoint3.name, newCarePoint));
                                    break;
                                case 4:
                                    generateTile(battlePoint4, newCarePoint);
                                    tiles.Add(new Tile(battlePoint4.name, newCarePoint));
                                    break;
                                case 5:
                                    generateTile(battlePoint5, newCarePoint);
                                    tiles.Add(new Tile(battlePoint5.name, newCarePoint));
                                    break;
                            }
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
        generateStartTile();
        generateEndPointTile();    
        var endPointTilePosition = endPointTile.GetComponent<RectTransform>().anchoredPosition;
        var startTilePosition = startTile.GetComponent<RectTransform>().anchoredPosition;

        tiles.Add(new Tile(endPoint.name, endPointTilePosition));
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
        LoadData();
        foreach (var tile in mapData.tiles)
        {
            var careGameObject = Resources.Load<GameObject>(tile.tileName);
            var careTile = Instantiate(careGameObject, new Vector3(0, 0, 0), Quaternion.identity);
            careTile.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>());
            careTile.GetComponent<RectTransform>().localScale = new Vector2(imageScale, imageScale);
            careTile.GetComponent<RectTransform>().anchoredPosition = tile.tilePosition;
            tiles.Add(tile);
        }
        Debug.Log("PlayerPos1:" + mapData.playerPosition);
        player = Instantiate(playerPrefab, mapData.playerPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
        player.GetComponent<RectTransform>().anchoredPosition = mapData.playerPosition;
        Debug.Log("PlayerPos1:" + player.transform.position);
        //startPlayerPosition = mapData.playerPosition;
    }
    
    void Start()
    {
        var canvas = this.GetComponent<Canvas>();
        width = canvas.GetComponent<RectTransform>().rect.size.x;
        height = canvas.GetComponent<RectTransform>().rect.size.y;

        //var z = ScriptableObject.CreateInstance<Map>();
        InitializePrefabs();
        if (!File.Exists(mapDataFilePath))
        {
            if(!PlayerPrefs.HasKey("mapLevel"))
                PlayerPrefs.SetInt("mapLevel", 1);
            else
                PlayerPrefs.SetInt("mapLevel", PlayerPrefs.GetInt("mapLevel")+1);
            //InitializePrefabs();
            InitializateGenerationMap();
        }
        else
            GenerateMapFromFile();

    }

    // Update is called once per frame
    void Update()
    {

    }


}
