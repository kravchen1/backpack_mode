using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;
//using System;

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
    private GameObject greenStandart;
    private List<Vector2> tilePositions = new List<Vector2>();
    private List<Vector2> pointInterestPoisitions = new List<Vector2>();
    private List<InterestPointStructure> pointInterestStructure = new List<InterestPointStructure>();
    private carePosition carePosition = new carePosition();

    private float width, height;
    private Vector3 moveDownVector = new Vector3(0f, -25.0f, 0.0f);


    public GameObject bossTile;
    public GameObject startTile;
    public Vector3 startTilePosition;

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
        startTilePosition = new Vector3(x, y, 0); 
        Debug.Log(startTilePosition);
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
        for (int i = 0; i < countRoad; i++)
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

    public void GenerateRandomTile(Vector3 carePosition, List<Vector3> tilePositions)
    {
        bool itInterestPoint = false;
        int randomPlace = Random.Range(1, 3);
        int roadOrInterestPoint = Random.Range(1, 3);


        if (roadOrInterestPoint == 1 || itInterestPoint)
        {
            generateTile(road, carePosition);
            itInterestPoint = false;
        }
        else
        {
            switch (randomPlace)
            {
                case 1:
                    generateTile(shopPoint, carePosition);
                    itInterestPoint = true;
                    break;
                case 2:
                    generateTile(battlePoint, carePosition);
                    itInterestPoint = true;
                    break;
                default:
                    Debug.Log("ты долбоёб?");
                    break;
            }
        }
        tilePositions.Add(carePosition);
    }

    //public void RoadGeneration(Vector3 bossTilePosition, Vector3 startTilePosition)
    //{
    //    var carePosition = bossTilePosition - new Vector3(0, 25f, 0);
    //    bool itInterestPoint = false;
    //    List<Vector3> tilePositions = new List<Vector3>();
    //    tilePositions.Add(bossTilePosition);
    //    tilePositions.Add(startTilePosition);
    //    var movingVector = new Vector3(0, 0, 0);
    //    if (startTilePosition.x == 0)
    //        movingVector = new Vector3(-25f, 0, 0);
    //    else
    //        movingVector = new Vector3(25f, 0, 0);
    //    for (int i = 0; i < 3; i++)
    //    {
    //        switch (i)
    //        {
    //            case 0:
    //                if (startTilePosition.x == bossTilePosition.x)
    //                {
    //                    movingVector = new Vector3(0, -25f, 0);
    //                    while (carePosition.y != startTilePosition.y)
    //                    {
    //                        //Генерация от босса к старту по Y
    //                        GenerateRandomTile(carePosition, tilePositions, movingVector);
    //                    }
    //                }
    //                else
    //                {
    //                    movingVector = new Vector3(0, -25f, 0);
    //                    while (carePosition.y >= startTilePosition.y)
    //                    {
    //                        GenerateRandomTile(carePosition, tilePositions, movingVector);
    //                    }
    //                    //carePosition = bossTilePosition;
    //                    if (startTilePosition.x == 0)
    //                    {
    //                        movingVector = new Vector3(-25f, 0, 0);
    //                        while (carePosition.x >= startTilePosition.x)
    //                        {
    //                            GenerateRandomTile(carePosition, tilePositions, movingVector);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        movingVector = new Vector3(25f, 0, 0);
    //                        while (carePosition.x <= startTilePosition.x)
    //                        {
    //                            GenerateRandomTile(carePosition, tilePositions, movingVector);
    //                        }
    //                    }
    //                }
    //                break;
    //            case 1:
    //                if (startTilePosition.x == bossTilePosition.x)
    //                {
    //                    movingVector = new Vector3(0, -25f, 0);
    //                    while (carePosition.y != startTilePosition.y)
    //                    {
    //                        //Генерация от босса к старту по Y
    //                        GenerateRandomTile(carePosition, tilePositions, movingVector);
    //                    }
    //                }
    //                else
    //                {
    //                    movingVector = new Vector3(0, -25f, 0);
    //                    while (carePosition.y >= startTilePosition.y)
    //                    {
    //                        GenerateRandomTile(carePosition, tilePositions, movingVector);
    //                    }
    //                    carePosition = bossTilePosition;
    //                    if (startTilePosition.x == 0)
    //                    {
    //                        movingVector = new Vector3(-25f, 0, 0);
    //                        while (carePosition.x >= startTilePosition.x)
    //                        {
    //                            GenerateRandomTile(carePosition, tilePositions, movingVector);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        movingVector = new Vector3(25f, 0, 0);
    //                        while (carePosition.x <= startTilePosition.x)
    //                        {
    //                            GenerateRandomTile(carePosition, tilePositions, movingVector);
    //                        }
    //                    }
    //                }
    //                break;
    //        }
    //    }

    //}

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

    public Vector2 Recursiya(Vector2 carePoint)
    {
        Dictionary<int, VectorStructure> movingVectors = new Dictionary<int, VectorStructure>();
        movingVectors.Add(0, new VectorStructure(new Vector2(0, 25f)));
        movingVectors.Add(1, new VectorStructure(new Vector2(0, -25f)));
        movingVectors.Add(2, new VectorStructure(new Vector2(-25f, 0)));
        movingVectors.Add(3, new VectorStructure(new Vector2(25f, 0)));
        foreach(var vector in movingVectors)
        {
            if (tilePositions.Any(e => e == bossTile.GetComponent<RectTransform>().anchoredPosition + vector.Value.movingVector))
            {
                roadToBoss = true;
                return carePoint;
            }
        }
        
        var bossPos = bossTile.GetComponent<RectTransform>().anchoredPosition;
        var newCarePoint = carePoint;
        if ((carePoint + movingVectors[0].movingVector).y >= height || tilePositions.Any(e => e == carePoint + movingVectors[0].movingVector))
            movingVectors[0].canMove = false;
        else
            movingVectors[0].canMove = true;
        if ((carePoint + movingVectors[1].movingVector).y < 0 || tilePositions.Any(e => e == carePoint + movingVectors[1].movingVector))
            movingVectors[1].canMove = false;
        else
            movingVectors[1].canMove = true;
        if ((carePoint + movingVectors[2].movingVector).x < 0 || tilePositions.Any(e => e == carePoint + movingVectors[2].movingVector))
            movingVectors[2].canMove = false;
        else
            movingVectors[2].canMove = true;
        if ((carePoint + movingVectors[3].movingVector).x >= width || tilePositions.Any(e => e == carePoint + movingVectors[3].movingVector))
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
            return carePoint;
        }
        else
        {
            //while(newCarePoint + existVectors[randomVector] * 3 > )
            for (int i = 0; i < 2; i++)
            {
                newCarePoint += existVectors[randomVector];
                if (!tilePositions.Any(e => e == newCarePoint))
                {
                    tilePositions.Add(newCarePoint);
                    generateTile(road, newCarePoint);
                }
                else
                    break;
            }
            int randomPlace = Random.Range(1, 3);
            newCarePoint += existVectors[randomVector];
            if (!tilePositions.Any(e => e == newCarePoint))
            {
                tilePositions.Add(newCarePoint);
                pointInterestPoisitions.Add(newCarePoint);
                switch (randomPlace)
                {
                    case 1:
                        generateTile(shopPoint, newCarePoint);
                        break;
                    case 2:
                        generateTile(battlePoint, newCarePoint);
                        break;
                    default:
                        Debug.Log("ты долбоёб?");
                        break;
                }
            }
            //GenerateRandomTile(newCarePoint, tilePositions);
            return Recursiya(newCarePoint);
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
                if (tilePositions.Any(e => e == bossTile.GetComponent<RectTransform>().anchoredPosition + vector.Value.movingVector))
                {
                    roadToBoss = true;
                }
            }

            var bossPos = bossTile.GetComponent<RectTransform>().anchoredPosition;
            var newCarePoint = carePoint.position;
            if ((carePoint.position + movingVectors[0].movingVector).y >= height || tilePositions.Any(e => e == carePoint.position + movingVectors[0].movingVector))
                movingVectors[0].canMove = false;
            else
                movingVectors[0].canMove = true;
            if ((carePoint.position + movingVectors[1].movingVector).y < 0 || tilePositions.Any(e => e == carePoint.position + movingVectors[1].movingVector))
                movingVectors[1].canMove = false;
            else
                movingVectors[1].canMove = true;
            if ((carePoint.position + movingVectors[2].movingVector).x < 0 || tilePositions.Any(e => e == carePoint.position + movingVectors[2].movingVector))
                movingVectors[2].canMove = false;
            else
                movingVectors[2].canMove = true;
            if ((carePoint.position + movingVectors[3].movingVector).x >= width || tilePositions.Any(e => e == carePoint.position + movingVectors[3].movingVector))
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
                //while(newCarePoint + existVectors[randomVector] * 3 > )
                var randomRoadCount = Random.Range(2, 4);
                //if (randomRoadCount == 0)
                //{
                //    return;
                //}
                for (int i = 0; i < randomRoadCount; i++)
                {
                    newCarePoint += existVectors[randomVector];
                    if (!tilePositions.Any(e => e == newCarePoint) && newCarePoint.x >= 0 && newCarePoint.y >= 0 && newCarePoint.x < width && newCarePoint.y < height - ((height % 25)))
                    {
                        tilePositions.Add(newCarePoint);
                        generateTile(road, newCarePoint);
                    }
                    else
                        break;
                }
                int randomPlace = Random.Range(1, 3);
                newCarePoint += existVectors[randomVector];
                if (!tilePositions.Any(e => e == newCarePoint) && newCarePoint.x >= 0 && newCarePoint.y >= 0 && newCarePoint.x < width && newCarePoint.y < height - ((height % 25)))
                {
                    tilePositions.Add(newCarePoint);
                    //pointInterestPoisitions.Add(newCarePoint);
                    pointInterestStructure.Add(new InterestPointStructure(newCarePoint));
                    switch (randomPlace)
                    {
                        case 1:
                            generateTile(shopPoint, newCarePoint);
                            break;
                        case 2:
                            generateTile(battlePoint, newCarePoint);
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
    void Start()
    {
        var canvas = GetComponent<Canvas>();
        width = canvas.GetComponent<RectTransform>().rect.size.x;
        height = canvas.GetComponent<RectTransform>().rect.size.y;
        var square = 0;
        generateBossTile();
        generateStartTile();

        var bossTilePosition = bossTile.GetComponent<RectTransform>().anchoredPosition;
        var startTilePosition = startTile.GetComponent<RectTransform>().anchoredPosition;

        tilePositions.Add(bossTilePosition);
        tilePositions.Add(startTilePosition);

        var carePoint = startTilePosition;
        //Recursiya(startTilePosition);
        var countInterestPoint = Random.Range(5, 10);
        pointInterestPoisitions.Add(startTilePosition);
        pointInterestStructure.Add(new InterestPointStructure(startTilePosition));
        var index = pointInterestPoisitions.Count() - 1;
        var countCycle = 0;
        //while (!roadToBoss)
        //{
        //    countCycle++;
        //    var countPoints = pointInterestPoisitions.Count();
        //    for (int i = 0; i < countPoints; i++)
        //    {
        //        MapGenerator(pointInterestPoisitions[i]);
        //    }
        //    if (countCycle > 100)
        //    {
        //        Debug.Log("Мы закциклились епта");
        //        break;
        //    }
        //}
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
        for (int i = 0; i < height; i+=25)
        {
            for (int j = 0; j < width; j+=25)
            {
                if (!tilePositions.Any(e => e == new Vector2(j,i)))
                {
                    generateTile(greenStandart, new Vector2(j, i));
                }
            }
        }
        Debug.Log(pointInterestStructure.Where(e => e.canBuild == false).Count());
        Debug.Log(roadToBoss);



        //RoadGeneration(bossTilePosition, startTilePosition);
        //Debug.Log((height%25));

        //fullRoadGeneration();


    }

    // Update is called once per frame
    void Update()
    {
    }


}
