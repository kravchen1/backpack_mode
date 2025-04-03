using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : EventParent
{
    public GameObject player;
    private bool isPlayerInTrigger = false;

    public TextMeshPro lvlText;

    public int lvlEnemy = 1;
    public int idSpawner = 0;
    public string enemyJSON = "";

    public int startHP = 100;
    public int stepHPForLevel = 25;

    public float startStamina = 5.0f;
    public float stepStaminaForLevel = 0.5f;

    public List<GameObject> dropItems;
    public List<float> probabilityDropItems;

    private string currentSceneName;
    private GameObject map;

    private GameObject canvasBackpackEnemy;
    private GenerateBackpackOnMap generateBackpackOnMap;
    private bool isDie = false;

    public float moveSpeed = 30f;
    public Animator animator;

    //private static readonly int IsRunning = Animator.StringToHash("Run1");
    //private static readonly int IsIdle = Animator.StringToHash("Idle");
    private List<Vector2> pointsRun = new List<Vector2>();
    private RectTransform rt;

    private string Biom = "1";
    private int idSpawn = 0;
    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        lvlText.text = "lvl. " + lvlEnemy.ToString();
        if(currentSceneName == "GenerateMap")
            map = GameObject.FindGameObjectWithTag("GoMap");
        else
            map = GameObject.FindGameObjectWithTag("Cave");
        if (gameObject.transform.parent.GetComponent<BattleSpawn>() != null)
        {
            idSpawn = gameObject.transform.parent.GetComponent<BattleSpawn>().id;

            pointsRun = gameObject.transform.parent.GetComponent<BattleSpawn>().pointsRun;
        }
        rt = GetComponent<RectTransform>();
    }

    public virtual void Move()
    {
        if (!isDie)
        {
            pointsRun = gameObject.transform.parent.GetComponent<BattleSpawn>().pointsRun;

            rt = GetComponent<RectTransform>();
            StartCoroutine(MoveBetweenPoints());
        }
    }

    public IEnumerator MoveBetweenPoints()
    {
        // Проверяем, есть ли точки для перемещения
        if (pointsRun == null || pointsRun.Count == 0)
        {
            Debug.LogWarning("No points to move!");
            yield break;
        }
        // Проверяем, назначен ли аниматор
        if (animator == null)
        {
            Debug.LogWarning("No animator!");
            yield break;
        }
        // Начинаем с первой точки в списке
        int currentPointIndex = 0;
        rt.anchoredPosition = pointsRun[currentPointIndex];

        while (true)
        {
            // Включаем анимацию покоя
            animator.Play("Idle");

            // Ждем r секунд
            float r = UnityEngine.Random.Range(3f, 7f);
            yield return new WaitForSeconds(r);
            //yield return new WaitForSeconds(0.5f);
            // Переходим к следующей точке
            currentPointIndex = (currentPointIndex + 1) % pointsRun.Count;
            Vector2 targetPoint = pointsRun[currentPointIndex];
            Vector3 theScale = transform.GetChild(0).localScale;
            // Включаем анимацию бега
            animator.Play("Run1");

            if (rt.anchoredPosition.x < targetPoint.x)
            {
                if(theScale.x > 0)
                    theScale.x = -theScale.x;
            }
            else
            {
                theScale.x = Math.Abs(theScale.x);
            }
            transform.GetChild(0).localScale = theScale;

            // Двигаемся к точке
            while (Vector2.Distance(rt.anchoredPosition, targetPoint) > 0.1f)
            {
                rt.anchoredPosition = Vector2.MoveTowards(
                    rt.anchoredPosition,
                    targetPoint,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            // Гарантируем, что мы точно достигли точки
            rt.anchoredPosition = targetPoint;
        }
    }


    protected void OnTriggerEnter2D()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        isPlayerInTrigger = true;
        if (isShowPressE)
        {
            GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume",1f);
            GetComponent<AudioSource>().Play();
            SetActivePressE(isShowPressE);
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInTrigger = false;
        SetActivePressE(false);
    }

    public void ActivateEnemy()
    {
        checkCameraPositionAndSavePlayerPosition(player);
        PlayerPrefs.SetString("currentLocation", SceneManager.GetActiveScene().name);

        if (gameObject.transform.parent.GetComponent<BattleSpawn>() != null)
        {
            BattlesSpawnerData battlesSpawnerData = new BattlesSpawnerData();
            battlesSpawnerData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn" + Biom + ".json"));

            battlesSpawnerData.battlesSpawnerDataClass.battleData.Where(e => e.id == idSpawn).ToList()[0].position = rt.anchoredPosition;

            battlesSpawnerData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn" + Biom + ".json"));
        }

        StartBattle();
    }



    protected void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE)
        {
            ActivateEnemy();
        }
    }




    protected void OnMouseUp()
    {
        //Debug.Log("clickBackpackEnemy");
        if (canvasBackpackEnemy == null)
        {
            canvasBackpackEnemy = GameObject.FindGameObjectWithTag("backpack");
            if (canvasBackpackEnemy != null)
            {
                canvasBackpackEnemy.transform.GetChild(0).gameObject.SetActive(true);
                canvasBackpackEnemy.transform.GetChild(1).gameObject.SetActive(true);
                generateBackpackOnMap = canvasBackpackEnemy.GetComponent<GenerateBackpackOnMap>();
                generateBackpackOnMap.ClearBackpackObjects();
                generateBackpackOnMap.Generate(enemyJSON);
            }
        }
        else
        {
            if (canvasBackpackEnemy != null)
            {
                canvasBackpackEnemy.transform.GetChild(0).gameObject.SetActive(true);
                canvasBackpackEnemy.transform.GetChild(1).gameObject.SetActive(true);
                generateBackpackOnMap.ClearBackpackObjects();
                generateBackpackOnMap.Generate(enemyJSON);
            }
        }
    }



    private void EndDieAnimation()
    {
        if (dropItems.Count > 0 && dropItems.Count == probabilityDropItems.Count)
        {
            for (int i = 0; i < dropItems.Count; i++)
            {
                // Создаём генератор с seed (можно использовать время)
                Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks);
                // Случайный float в [min, max)
                float r = rand.NextFloat(0.0f, 100.0f);

                //Debug.Log("Index: " + i);
                //Debug.Log("dropItems[i]: " + dropItems[i].name);
                if (dropItems[i].GetComponent<DropItem>().item.CompareTag("ItemKeyStone") && dropItems[i].GetComponent<DropItem>().item.GetComponent<CaveStonesKeys>().stoneLevel == PlayerPrefs.GetInt("caveEnemyLvl")+1)
                    r = 0;
                if (r <= (probabilityDropItems[i] + (lvlEnemy-1) * 0.15f))
                {
                    //Debug.Log(dropItems[i].name + "  loot " + r);
                    if (SceneManager.GetActiveScene().name == "Cave")
                    {
                        Instantiate(dropItems[i], gameObject.transform.position + new Vector3(-300, -200, 0), Quaternion.identity, map.GetComponent<RectTransform>().transform);
                    }
                    else
                        Instantiate(dropItems[i], gameObject.transform.position, Quaternion.identity, map.GetComponent<RectTransform>().transform);
                }
            }
        }
        Destroy(gameObject.transform.parent.gameObject);
        FindFirstObjectByType<Player>().InitializedGPSTracker();
    }
    public void Die()
    {
        isDie = true;
        Invoke("EndDieAnimation", 1f);
    }

    public void JSONBackpackInitialized()
    {
        enemyJSON = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-81.04045104980469,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":39.6099853515625,\"y\":-161.37620544433595,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        if (gameObject.tag == "EnemyCave1")
        {
            enemyJSON = GlobalMap1Enemy(); //Cave1Enemy();
        }
        else if (gameObject.tag == "EnemyGlobalMap1")
        {
            enemyJSON = GlobalMap1Enemy(); //enemyJSON = switch lvl mob
        }
        else if (gameObject.tag == "BossCave1")
        {
            if(gameObject.name == "Dragon(Clone)")
                enemyJSON = Cave1BossDragon(); //todo
        }
        else if (gameObject.tag == "BossGlobalMap1")
        {
            enemyJSON = GlobalMap1Enemy(); //todo
            //BossGlobalMap1Enemy();
        }
        else if (gameObject.tag == "MiniBossGlobalMap1")
        {
            enemyJSON = GlobalMap1Enemy(); //todo
            //MiniBossGlobalMap1Enemy();
        }
    }
    public void StartBattle()
    {
        if (!isDie)
        {
            PlayerPrefs.SetString("enemyName", gameObject.name);
            PlayerPrefs.SetInt("enemyLvl", lvlEnemy);

            PlayerPrefs.SetInt("enemyHP", startHP + ((lvlEnemy - 1) * stepHPForLevel));
            PlayerPrefs.SetFloat("enemyStamina", startStamina + ((lvlEnemy - 1) * stepStaminaForLevel));
            if (currentSceneName == "GenerateMap")
                PlayerPrefs.SetInt("enemyIdSpawner", idSpawner);



            PlayerPrefs.SetString("enemyBackpackJSON", enemyJSON);



            //SceneManager.LoadScene("BackPackBattle");
            SceneLoader.Instance.LoadScene("BackPackBattle");
        }
    }



    string GlobalMap1EnemyLevel1()//+20250401
    {

        System.Random sysRandom = new System.Random();
        int r = sysRandom.Next(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-3.84002685546875,\"y\":39.2510986328125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-3.84002685546875,\"y\":39.2510986328125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                
                break;
            case 1:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.83990478515625,\"y\":-0.7047271728515625,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":79.8399658203125,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                
                break;
            case 2:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":33.659942626953128,\"y\":-1.50469970703125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

                break;
            case 3:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";

                break;
            case 4:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":80.63998413085938,\"y\":41.53111267089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-3.84002685546875,\"y\":39.2510986328125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 5:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-3.84014892578125,\"y\":39.2510986328125,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63995361328125,\"y\":-0.70477294921875,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                
                break;
            case 6:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":36.540008544921878,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                
                break;
            case 7:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-4.0400390625,\"y\":39.0311279296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                
                break;
            case 8:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":33.0599365234375,\"y\":79.83111572265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}}]}";

                break;
            case 9:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-3.84002685546875,\"y\":43.03111267089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":80.63998413085938,\"y\":39.2510986328125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";

                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel2()//+20250401
    {
        System.Random sysRandom = new System.Random();
        int r = sysRandom.Next(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-3.6400146484375,\"y\":42.731109619140628,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Sling\",\"position\":{\"x\":118.1400146484375,\"y\":78.83111572265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
        switch (r)
        {
            case 0:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-3.84002685546875,\"y\":43.03111267089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":124.08999633789063,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                
                break;
            case 1:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-3.6400146484375,\"y\":42.731109619140628,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Sling\",\"position\":{\"x\":118.1400146484375,\"y\":78.83111572265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                
                break;
            case 2:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":-4.6400146484375,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":118.30999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":123.739990234375,\"y\":80.43109130859375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
 
                break;
            case 3:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":41.53111267089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":123.739990234375,\"y\":80.43109130859375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                
                break;
            case 4:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":80.63998413085938,\"y\":121.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slingshot\",\"position\":{\"x\":35.42999267578125,\"y\":-0.638885498046875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                
                break;
            case 5:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":38.759979248046878,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":33.0599365234375,\"y\":160.16693115234376,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                
                break;
            case 6:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":38.759979248046878,\"y\":119.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                
                break;
            case 7:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Club\",\"position\":{\"x\":85.33999633789063,\"y\":-2.7689056396484377,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-3.84002685546875,\"y\":43.03111267089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}}]}";
                
                break;
            case 8:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Club\",\"position\":{\"x\":85.33999633789063,\"y\":-2.7689056396484377,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                
                break;
            case 9:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Halberd\",\"position\":{\"x\":41.39996337890625,\"y\":40.54693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":80.63998413085938,\"y\":-38.80470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel3()//+20250401
    {
        System.Random sysRandom = new System.Random();
        int r = sysRandom.Next(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":80.63998413085938,\"y\":-38.80470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":292.20001220703127,\"y\":200.30270385742188,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-3.84002685546875,\"y\":119.5869140625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
        switch (r)
        {
            case 0:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":80.63998413085938,\"y\":-38.80470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":292.20001220703127,\"y\":200.30270385742188,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-3.84002685546875,\"y\":119.5869140625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 1:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-173.5999755859375,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-44.8699951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-44.8699951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":32.760009765625,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":80.63998413085938,\"y\":123.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 2:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ChainWhip\",\"position\":{\"x\":-91.09002685546875,\"y\":38.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-172.79998779296876,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":32.760009765625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":43.03111267089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":33.8299560546875,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 3:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":38.759979248046878,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":120.56692504882813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-89.1199951171875,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-173.5999755859375,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 4:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.785430908203128,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88626861572266,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-0.704803466796875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPot\",\"position\":{\"x\":37.15997314453125,\"y\":118.56692504882813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":38.759979248046878,\"y\":-121.00047302246094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":33.0599365234375,\"y\":240.50274658203126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 5:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88626861572266,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":-3.84002685546875,\"y\":-83.50469970703125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":204.52001953125,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":249.5999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":33.659942626953128,\"y\":78.83111572265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 6:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88599395751953,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.785736083984378,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":198.45733642578126,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-3.84002685546875,\"y\":159.96710205078126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-3.84002685546875,\"y\":240.30294799804688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Katana\",\"position\":{\"x\":86.0399169921875,\"y\":162.11297607421876,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-3.84002685546875,\"y\":79.63131713867188,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":78.67779541015625,\"y\":-82.21380615234375,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":42.077606201171878,\"y\":-166.6938018798828,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-1.65826416015625,\"y\":-82.2138671875,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":32.760009765625,\"y\":-0.7045745849609375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 7:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":80.44003295898438,\"y\":119.3668212890625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-4.0400390625,\"y\":119.3668212890625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":163.15789794921876,\"y\":-1.87774658203125,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1201171875,\"y\":-81.04059600830078,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":82.8216552734375,\"y\":-1.8779296875,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04059600830078,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.8399658203125,\"y\":-81.04059600830078,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.8399658203125,\"y\":-0.704803466796875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.3199462890625,\"y\":-81.04059600830078,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.3199462890625,\"y\":-0.704803466796875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 8:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":163.15789794921876,\"y\":78.45816040039063,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1201171875,\"y\":-0.704681396484375,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-0.70458984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.70458984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.70458984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":38.759979248046878,\"y\":-121.04048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 9:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Halberd\",\"position\":{\"x\":33.780029296875,\"y\":34.39109802246094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-51.41998291015625,\"y\":-0.50469970703125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-4.6400146484375,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":79.8399658203125,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel4()//+20250401
    {
        System.Random sysRandom = new System.Random();
        int r = sysRandom.Next(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Halberd\",\"position\":{\"x\":-127.55999755859375,\"y\":40.54693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-51.41998291015625,\"y\":-0.50469970703125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-37.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":120.56692504882813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}}]}";
        switch (r)
        {
            case 0:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Halberd\",\"position\":{\"x\":-127.55999755859375,\"y\":40.54693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-51.41998291015625,\"y\":-0.50469970703125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-37.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":120.56692504882813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}}]}";
                break;
            case 1:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-37.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":120.56692504882813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-130.70001220703126,\"y\":79.69692993164063,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-3.84002685546875,\"y\":43.03111267089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-173.5999755859375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-50.64996337890625,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 2:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-37.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-3.84002685546875,\"y\":-37.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":32.760009765625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Crossbow\",\"position\":{\"x\":-129.3499755859375,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-173.5999755859375,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":-129.699951171875,\"y\":160.76690673828126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dagger\",\"position\":{\"x\":-88.32000732421875,\"y\":36.18109130859375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 3:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":-130.199951171875,\"y\":-121.00059509277344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-172.79998779296876,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":-131.79998779296876,\"y\":118.56692504882813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-132.20001220703126,\"y\":240.10272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-3.6400146484375,\"y\":-37.60469055175781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 4:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.27215576171877,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":292.20001220703127,\"y\":200.30282592773438,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Club\",\"position\":{\"x\":-83.6199951171875,\"y\":-83.10469055175781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071067094802856}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":-119.14048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 5:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slingshot\",\"position\":{\"x\":-3.9100341796875,\"y\":-120.31048583984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slingshot\",\"position\":{\"x\":82.239990234375,\"y\":37.63111877441406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071067094802856}},{\"name\":\"Sling\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-51.719970703125,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-88.32000732421875,\"y\":-40.10469055175781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Sling\",\"position\":{\"x\":123.739990234375,\"y\":160.76690673828126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 6:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Katana\",\"position\":{\"x\":170.52001953125,\"y\":1.44110107421875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":118.1400146484375,\"y\":159.16693115234376,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":79.8399658203125,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":32.760009765625,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-3.84002685546875,\"y\":40.23109436035156,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-88.32000732421875,\"y\":43.03111267089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 7:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Halberd\",\"position\":{\"x\":119.8800048828125,\"y\":37.86692810058594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":-45.719970703125,\"y\":-121.04048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-89.1199951171875,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-4.6400146484375,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":80.44003295898438,\"y\":119.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 8:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":-45.719970703125,\"y\":39.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":124.08999633789063,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":124.08999633789063,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":121.239990234375,\"y\":-81.24049377441406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-4.6400146484375,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-89.1199951171875,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 9:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79208374023438,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-4.6400146484375,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":79.8399658203125,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":249.39996337890626,\"y\":-41.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":164.92001342773438,\"y\":-41.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.8399658203125,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.8399658203125,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.3199462890625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.3199462890625,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-172.79998779296876,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-172.79998779296876,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel5()//+20250401
    {
        System.Random sysRandom = new System.Random();
        int r = sysRandom.Next(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79208374023438,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":33.659942626953128,\"y\":-162.1763153076172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":249.39996337890626,\"y\":-41.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":249.39996337890626,\"y\":119.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":118.739990234375,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Halberd\",\"position\":{\"x\":41.39996337890625,\"y\":120.88275146484375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":164.91998291015626,\"y\":39.0311279296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79208374023438,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":33.659942626953128,\"y\":-162.1763153076172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":249.39996337890626,\"y\":-41.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":249.39996337890626,\"y\":119.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":118.739990234375,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Halberd\",\"position\":{\"x\":41.39996337890625,\"y\":120.88275146484375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":164.91998291015626,\"y\":39.0311279296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 1:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPot\",\"position\":{\"x\":38.759979248046878,\"y\":-121.00047302246094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":37.15997314453125,\"y\":198.90274047851563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":206.02001953125,\"y\":38.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":202.6199951171875,\"y\":-81.84050750732422,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":202.6199951171875,\"y\":159.16693115234376,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 2:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-87.52001953125,\"y\":197.20272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-3.03997802734375,\"y\":197.20272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"Crossbow\",\"position\":{\"x\":124.0899658203125,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":124.0899658203125,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-88.32000732421875,\"y\":-37.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":32.760009765625,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-51.72003173828125,\"y\":-161.37632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":32.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Kettlebell\",\"position\":{\"x\":165.1199951171875,\"y\":-38.80470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 3:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.785736083984378,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.45733642578126,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":292.199951171875,\"y\":-40.70448303222656,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":-299.15997314453127,\"y\":-40.70451354980469,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Halberd\",\"position\":{\"x\":125.8800048828125,\"y\":120.88290405273438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-44.8699951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-173.0,\"y\":39.0311279296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-51.719970703125,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 4:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79208374023438,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.8321533203125,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.8321533203125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-172.7999267578125,\"y\":-38.80470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-88.32000732421875,\"y\":-38.80470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-173.59991455078126,\"y\":-198.87632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-89.1199951171875,\"y\":-198.87632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":38.260040283203128,\"y\":79.69692993164063,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":249.60003662109376,\"y\":203.70272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":165.1199951171875,\"y\":203.70272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":80.63998413085938,\"y\":203.70272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.8399658203125,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.8399658203125,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 5:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.8321533203125,\"y\":198.45724487304688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88599395751953,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79217529296876,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.785797119140628,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":201.72003173828126,\"y\":159.96701049804688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":201.72003173828126,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":80.63998413085938,\"y\":-0.70458984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ChainWhip\",\"position\":{\"x\":35.6600341796875,\"y\":-79.67457580566406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":35.560028076171878,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":159.96701049804688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.8399658203125,\"y\":159.96701049804688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":240.30282592773438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-173.0,\"y\":119.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-89.1199951171875,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 6:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60784912109376,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Broom\",\"position\":{\"x\":-179.09002685546876,\"y\":4.6952056884765629,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":-38.80479431152344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Broom\",\"position\":{\"x\":-179.09002685546876,\"y\":-75.64059448242188,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04059600830078,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":-0.704803466796875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":249.5999755859375,\"y\":-37.30479431152344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":80.63998413085938,\"y\":-37.30479431152344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":334.0799560546875,\"y\":-0.704803466796875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":334.0799560546875,\"y\":-81.04059600830078,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":79.63101196289063,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":79.63101196289063,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":159.96682739257813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.96682739257813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 7:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60784912109376,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"AutomaticCrossbow\",\"position\":{\"x\":-170.73004150390626,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-219.780029296875,\"y\":78.83111572265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":292.199951171875,\"y\":-40.70469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":123.239990234375,\"y\":-40.70469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-4.6400146484375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-219.780029296875,\"y\":159.16693115234376,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 8:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60797119140626,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"AutomaticCrossbow\",\"position\":{\"x\":82.7099609375,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-130.79998779296876,\"y\":0.231109619140625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-257.280029296875,\"y\":-37.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-172.79998779296876,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-88.32000732421875,\"y\":123.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-172.79998779296876,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":80.63998413085938,\"y\":123.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":165.1199951171875,\"y\":123.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 9:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60797119140626,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-257.48004150390627,\"y\":-41.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-173.0,\"y\":-41.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ChainWhip\",\"position\":{\"x\":-6.61004638671875,\"y\":-41.50469970703125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-173.5999755859375,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-89.1199951171875,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":117.54010009765625,\"y\":160.16693115234376,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071067094802856}}]}";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel6()//+20250402
    {
        System.Random sysRandom = new System.Random();
        int r = sysRandom.Next(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":-131.5899658203125,\"y\":-123.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPot\",\"position\":{\"x\":206.02001953125,\"y\":-122.04048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPot\",\"position\":{\"x\":37.15997314453125,\"y\":38.23109436035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":249.5999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-89.1199951171875,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":-173.5999755859375,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":-131.5899658203125,\"y\":-123.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPot\",\"position\":{\"x\":206.02001953125,\"y\":-122.04048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPot\",\"position\":{\"x\":37.15997314453125,\"y\":38.23109436035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":249.5999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-89.1199951171875,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":-173.5999755859375,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 1:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-173.59991455078126,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ChainWhip\",\"position\":{\"x\":-6.61004638671875,\"y\":38.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":36.760009765625,\"y\":-81.24049377441406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Katana\",\"position\":{\"x\":158.82998657226563,\"y\":-155.976318359375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":205.719970703125,\"y\":-0.9046478271484375,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":201.72003173828126,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":201.72003173828126,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 2:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MudWorm\",\"position\":{\"x\":162.93997192382813,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":-3.84002685546875,\"y\":-123.01533508300781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":41.53111267089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":39.6099853515625,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":248.79998779296876,\"y\":202.802734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":207.72003173828126,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":124.08999633789063,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":162.93997192382813,\"y\":74.93109130859375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}}]}";
                break;
            case 3:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaShield\",\"position\":{\"x\":124.43997192382813,\"y\":-39.50469970703125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":207.0948486328125,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaRing1\",\"position\":{\"x\":80.63998413085938,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":-119.14048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":38.259979248046878,\"y\":160.03274536132813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-4.6400146484375,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":117.239990234375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":201.72003173828126,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":249.5999755859375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MudWorm\",\"position\":{\"x\":249.5999755859375,\"y\":-2.7689056396484377,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 4:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireBody1\",\"position\":{\"x\":124.43997192382813,\"y\":40.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":208.56997680664063,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireDagger1\",\"position\":{\"x\":38.134796142578128,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"BlackClaw\",\"position\":{\"x\":207.72003173828126,\"y\":-121.00047302246094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-3.84002685546875,\"y\":39.2510986328125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":33.659942626953128,\"y\":239.50274658203126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":202.6199951171875,\"y\":239.50274658203126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireDagger1\",\"position\":{\"x\":249.5999755859375,\"y\":37.65625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireBody1\",\"position\":{\"x\":39.9599609375,\"y\":-119.84048461914063,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 5:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-44.87005615234375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":124.0899658203125,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-50.820068359375,\"y\":239.50274658203126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":-47.72003173828125,\"y\":-161.57630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Kettlebell\",\"position\":{\"x\":118.739990234375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":117.54000854492188,\"y\":-161.1763153076172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":-47.72003173828125,\"y\":79.43109130859375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":117.54000854492188,\"y\":79.83111572265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-88.32000732421875,\"y\":-41.08470153808594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaShield\",\"position\":{\"x\":39.9599609375,\"y\":-39.50469970703125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":165.11996459960938,\"y\":-44.154693603515628,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 6:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-129.35003662109376,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-135.300048828125,\"y\":239.50274658203126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Kettlebell\",\"position\":{\"x\":34.260009765625,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Nunchucks\",\"position\":{\"x\":38.259979248046878,\"y\":-80.97470092773438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-88.52008056640625,\"y\":-121.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-172.79998779296876,\"y\":-37.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":34.260009765625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dinosaur\",\"position\":{\"x\":79.84002685546875,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-136.20001220703126,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 7:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-88.52001953125,\"y\":39.0311279296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slingshot\",\"position\":{\"x\":82.239990234375,\"y\":-123.04049682617188,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Sling\",\"position\":{\"x\":248.79998779296876,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-173.5999755859375,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-259.242431640625,\"y\":201.02923583984376,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-339.57830810546877,\"y\":201.02926635742188,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":36.760009765625,\"y\":-0.904693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":36.760009765625,\"y\":79.43109130859375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071067094802856}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":165.32000732421876,\"y\":42.731109619140628,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Sling\",\"position\":{\"x\":248.79998779296876,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 8:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79205322265626,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":33.0599365234375,\"y\":79.83111572265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"AutomaticCrossbow\",\"position\":{\"x\":82.7099609375,\"y\":-121.00047302246094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":36.760009765625,\"y\":-0.904693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-88.32000732421875,\"y\":-121.42048645019531,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Kettlebell\",\"position\":{\"x\":247.6375732421875,\"y\":201.02920532226563,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":167.3017578125,\"y\":201.02923583984376,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":290.199951171875,\"y\":-0.904693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":286.5,\"y\":79.83111572265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-89.1199951171875,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 9:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.3121337890625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64794921875,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64794921875,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.3121337890625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Broom\",\"position\":{\"x\":-10.1300048828125,\"y\":4.6951446533203129,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":79.63092041015625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":-3.8399658203125,\"y\":-81.04071044921875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MagicWand\",\"position\":{\"x\":-88.52001953125,\"y\":-121.64067077636719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MagicWand\",\"position\":{\"x\":164.91998291015626,\"y\":-121.64067077636719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":334.08001708984377,\"y\":-123.01556396484375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":249.5999755859375,\"y\":-123.01556396484375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":249.5999755859375,\"y\":79.63092041015625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":334.08001708984377,\"y\":37.65606689453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":80.63998413085938,\"y\":79.63092041015625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":-3.8399658203125,\"y\":79.63092041015625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04071044921875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":207.0948486328125,\"y\":-0.7048797607421875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":-3.8399658203125,\"y\":-161.37649536132813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37649536132813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":-88.320068359375,\"y\":79.63092041015625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel7()//+20250403
    {
        System.Random sysRandom = new System.Random();
        int r = sysRandom.Next(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12152099609375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.12152099609375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.12152099609375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":334.0799560546875,\"y\":-38.80458068847656,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20457458496094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":247.42001342773438,\"y\":74.93121337890625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":-0.70458984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-45.719970703125,\"y\":-40.66456604003906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MudWorm\",\"position\":{\"x\":247.42001342773438,\"y\":159.96701049804688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04039764404297,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-37.30458068847656,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlackClaw\",\"position\":{\"x\":38.759979248046878,\"y\":120.00698852539063,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":159.96701049804688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-172.79998779296876,\"y\":-81.04039764404297,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":-0.70458984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-257.280029296875,\"y\":-38.80458068847656,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12152099609375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.12152099609375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.12152099609375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":334.0799560546875,\"y\":-38.80458068847656,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20457458496094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":247.42001342773438,\"y\":74.93121337890625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":-0.70458984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-45.719970703125,\"y\":-40.66456604003906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MudWorm\",\"position\":{\"x\":247.42001342773438,\"y\":159.96701049804688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04039764404297,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-37.30458068847656,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlackClaw\",\"position\":{\"x\":38.759979248046878,\"y\":120.00698852539063,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":159.96701049804688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-172.79998779296876,\"y\":-81.04039764404297,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":-0.70458984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-257.280029296875,\"y\":-38.80458068847656,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 1:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.12152099609375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":247.42001342773438,\"y\":155.26690673828126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-3.84002685546875,\"y\":119.5869140625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"BlackClaw\",\"position\":{\"x\":292.199951171875,\"y\":39.67109680175781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireDagger1\",\"position\":{\"x\":-88.32000732421875,\"y\":-42.73469543457031,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"FireFlask1\",\"position\":{\"x\":291.5748291015625,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireDagger1\",\"position\":{\"x\":38.18994140625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Katana\",\"position\":{\"x\":74.3499755859375,\"y\":-75.6404800415039,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-172.79998779296876,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":-257.48004150390627,\"y\":-41.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-172.79998779296876,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 2:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79205322265626,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79205322265626,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":249.4000244140625,\"y\":39.0311279296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":249.5999755859375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":165.11993408203126,\"y\":37.65625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Halberd\",\"position\":{\"x\":-127.55999755859375,\"y\":40.54693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":-3.84002685546875,\"y\":-203.35116577148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.11993408203126,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":39.6099853515625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Broom\",\"position\":{\"x\":-9.24005126953125,\"y\":-2.7388916015625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":80.63998413085938,\"y\":-42.67955017089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":-88.32000732421875,\"y\":-203.35116577148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-88.32000732421875,\"y\":-41.08470153808594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 3:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-45.219970703125,\"y\":-240.91209411621095,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":249.39996337890626,\"y\":119.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBody1\",\"position\":{\"x\":-44.52001953125,\"y\":40.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":249.39996337890626,\"y\":-41.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaShield\",\"position\":{\"x\":124.43997192382813,\"y\":40.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":38.134796142578128,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":-130.82513427734376,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-129.3499755859375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":-172.79998779296876,\"y\":37.65625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":124.08999633789063,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-44.8699951171875,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 4:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-45.219970703125,\"y\":-240.91209411621095,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Crossbow\",\"position\":{\"x\":-129.3499755859375,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":249.5999755859375,\"y\":-42.67955017089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":208.56997680664063,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-50.82000732421875,\"y\":-162.1763153076172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":121.239990234375,\"y\":-241.91209411621095,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":164.91998291015626,\"y\":-121.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":80.44003295898438,\"y\":-121.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":117.53997802734375,\"y\":-0.50469970703125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":208.56997680664063,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":-3.84002685546875,\"y\":-42.67955017089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":80.63998413085938,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-172.79998779296876,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 5:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":198.45724487304688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.45724487304688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78562927246094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60784912109376,\"y\":37.78562927246094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":37.78562927246094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88599395751953,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78562927246094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":38.759979248046878,\"y\":-121.00038146972656,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":-131.59002685546876,\"y\":37.03120422363281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":206.02001953125,\"y\":38.63121032714844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-0.70458984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":-0.70458984375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":37.15997314453125,\"y\":198.90283203125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MudWorm\",\"position\":{\"x\":334.0799865722656,\"y\":77.56698608398438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":249.39996337890626,\"y\":199.70281982421876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":334.08001708984377,\"y\":240.30279541015626,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":-257.280029296875,\"y\":37.656341552734378,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 6:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78535461425781,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78535461425781,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79205322265626,\"y\":37.78535461425781,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79205322265626,\"y\":-122.88626861572266,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88626861572266,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-122.88626861572266,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":-46.28997802734375,\"y\":-161.37640380859376,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaBody1\",\"position\":{\"x\":39.9599609375,\"y\":-39.504791259765628,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaShield\",\"position\":{\"x\":208.92001342773438,\"y\":-39.504791259765628,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":38.134796142578128,\"y\":79.63101196289063,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaShield\",\"position\":{\"x\":-129.0,\"y\":-39.50469970703125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":122.6148681640625,\"y\":-161.37640380859376,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"AutomaticCrossbow\",\"position\":{\"x\":206.01998901367188,\"y\":156.00274658203126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":-130.82513427734376,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":-172.79998779296876,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 7:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":37.78535461425781,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78535461425781,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-122.88626861572266,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":-122.88626861572266,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":334.0799560546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":334.0799560546875,\"y\":-123.01533508300781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":249.5999755859375,\"y\":-123.01533508300781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":249.60003662109376,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":-257.27996826171877,\"y\":-123.01577758789063,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":-172.8001708984375,\"y\":-123.01576232910156,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":334.0799560546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MagicWand\",\"position\":{\"x\":333.8800048828125,\"y\":199.70272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.1199951171875,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":80.63998413085938,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MagicWand\",\"position\":{\"x\":249.39996337890626,\"y\":199.70272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":80.63998413085938,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 8:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83203125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83203125,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83203125,\"y\":-122.88626861572266,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":-89.10003662109375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":80.63995361328125,\"y\":202.20272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Katana\",\"position\":{\"x\":86.04000854492188,\"y\":1.44110107421875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-51.41998291015625,\"y\":79.83111572265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Spear\",\"position\":{\"x\":-89.10003662109375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-136.20001220703126,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-172.800048828125,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-47.72003173828125,\"y\":-161.57630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-136.20001220703126,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-172.800048828125,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"WitchPot\",\"position\":{\"x\":-300.550048828125,\"y\":-43.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":80.63995361328125,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 9:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.27215576171877,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":-122.88626861572266,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":121.239990234375,\"y\":79.43109130859375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-88.3199462890625,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-48.32000732421875,\"y\":160.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-47.72003173828125,\"y\":-161.57630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-3.84002685546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":123.239990234375,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":-3.84002685546875,\"y\":-42.67955017089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-88.32000732421875,\"y\":-41.08470153808594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":249.5999755859375,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.280029296875,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":122.6148681640625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":117.239990234375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel8()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":205.7921142578125,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":79.631103515625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":292.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-4.6400146484375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.280029296875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-89.1199951171875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":123.239990234375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-173.5999755859375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-130.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-299.15997314453127,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":247.42001342773438,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-258.08001708984377,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-259.46002197265627,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-342.5599365234375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":121.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel9()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":205.7921142578125,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":79.631103515625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":292.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-4.6400146484375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.280029296875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-89.1199951171875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":123.239990234375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-173.5999755859375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-130.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-299.15997314453127,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":247.42001342773438,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-258.08001708984377,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-259.46002197265627,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-342.5599365234375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":121.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel10()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":205.7921142578125,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":79.631103515625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":292.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-4.6400146484375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.280029296875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-89.1199951171875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":123.239990234375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-173.5999755859375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-130.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-299.15997314453127,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":247.42001342773438,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-258.08001708984377,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-259.46002197265627,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-342.5599365234375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":121.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel11()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":205.7921142578125,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":79.631103515625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":292.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-4.6400146484375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.280029296875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-89.1199951171875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":123.239990234375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-173.5999755859375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-130.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-299.15997314453127,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":247.42001342773438,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-258.08001708984377,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-259.46002197265627,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-342.5599365234375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":121.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel12()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":205.7921142578125,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":79.631103515625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":292.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-4.6400146484375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.280029296875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-89.1199951171875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":123.239990234375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-173.5999755859375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-130.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-299.15997314453127,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":247.42001342773438,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-258.08001708984377,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-259.46002197265627,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-342.5599365234375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":121.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel13()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":205.7921142578125,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":79.631103515625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":292.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-4.6400146484375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.280029296875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-89.1199951171875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":123.239990234375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-173.5999755859375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-130.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-299.15997314453127,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":247.42001342773438,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-258.08001708984377,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-259.46002197265627,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-342.5599365234375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":121.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel14()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":205.7921142578125,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":79.631103515625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":292.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-4.6400146484375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.280029296875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-89.1199951171875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":123.239990234375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-173.5999755859375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-130.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-299.15997314453127,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":247.42001342773438,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-258.08001708984377,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-259.46002197265627,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-342.5599365234375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":121.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }

    string GlobalMap1EnemyLevel15()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":205.7921142578125,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":79.631103515625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":334.0799865722656,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":292.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-4.6400146484375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.280029296875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-89.1199951171875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":123.239990234375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-173.5999755859375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-130.199951171875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nibbler\",\"position\":{\"x\":-299.15997314453127,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":247.42001342773438,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-258.08001708984377,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-259.46002197265627,\"y\":235.60272216796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-342.5599365234375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-3.84002685546875,\"y\":121.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }



    string GlobalMap1Enemy()
    {
        switch (lvlEnemy)
        {
            case 1:
                return GlobalMap1EnemyLevel1();
            case 2:
                return GlobalMap1EnemyLevel2();
            case 3:
                return GlobalMap1EnemyLevel3();
            case 4:
                return GlobalMap1EnemyLevel4();
            case 5:
                return GlobalMap1EnemyLevel5();
            case 6:
                return GlobalMap1EnemyLevel6();
            case 7:
                return GlobalMap1EnemyLevel7();
            case 8:
                return GlobalMap1EnemyLevel8();
            case 9:
                return GlobalMap1EnemyLevel9();
            case 10:
                return GlobalMap1EnemyLevel10();
            case 11:
                return GlobalMap1EnemyLevel11();
            case 12:
                return GlobalMap1EnemyLevel12();
            case 13:
                return GlobalMap1EnemyLevel13();
            case 14:
                return GlobalMap1EnemyLevel14();
            case 15:
                return GlobalMap1EnemyLevel15();
        }
        return "";
    }




    string Cave1EnemyLevel1()
    {
        int r = UnityEngine.Random.Range(0, 10);
        string result = "";
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel2()
    {
        int r = UnityEngine.Random.Range(0, 10);
        string result = "";
        switch (r)
        {
            case 0:
                result = "";

                break;
            case 1:
                result = "";

                break;
            case 2:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79208374023438,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-3.84002685546875,\"y\":-41.08488464355469,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 3:
                result = "";
                
                break;
            case 4:
                result = "";
                
                break;
            case 5:
                result = "";
                
                break;
            case 6:
                result = "";
                
                break;
            case 7:
                result = "";
                
                break;
            case 8:
                result = "";
                    
                break;
            case 9:
                result = "";
                
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel3()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";
        
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel4()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";
        
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel5()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";
        
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel6()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";
        
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel7()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel8()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel9()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";


        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel10()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel11()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel12()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }//+

    string Cave1EnemyLevel13()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }

    string Cave1EnemyLevel14()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }

    string Cave1EnemyLevel15()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }



    string Cave1Enemy()
    {
        switch (lvlEnemy)
        {
            case 1:
                return Cave1EnemyLevel1();
            case 2:
                return Cave1EnemyLevel2();
            case 3:
                return Cave1EnemyLevel3();
            case 4:
                return Cave1EnemyLevel4();
            case 5:
                return Cave1EnemyLevel5();
            case 6:
                return Cave1EnemyLevel6();
            case 7:
                return Cave1EnemyLevel7();
            case 8:
                return Cave1EnemyLevel8();
            case 9:
                return Cave1EnemyLevel9();
            case 10:
                return Cave1EnemyLevel10();
            case 11:
                return Cave1EnemyLevel11();
            case 12:
                return Cave1EnemyLevel12();
            case 13:
                return Cave1EnemyLevel13();
            case 14:
                return Cave1EnemyLevel14();
            case 15:
                return Cave1EnemyLevel15();
        }
        return "";
    }



    string Cave1BossDragonLevel1()
    {
        int r = -1;
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-214.6800537109375,\"y\":39.67127990722656,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-81.04045104980469,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":39.6099853515625,\"y\":-161.37620544433595,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 1:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Hammer\",\"position\":{\"x\":-256.760009765625,\"y\":-80.97470092773438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-88.32000732421875,\"y\":-44.154693603515628,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 2:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-341.760009765625,\"y\":-204.82630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slingshot\",\"position\":{\"x\":-299.65997314453127,\"y\":-80.97470092773438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 3:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-341.760009765625,\"y\":-204.82630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dagger\",\"position\":{\"x\":-257.280029296875,\"y\":-121.42048645019531,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dagger\",\"position\":{\"x\":-341.760009765625,\"y\":-41.08470153808594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 4:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":-342.5599365234375,\"y\":-198.87632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":-342.5599365234375,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":-258.08001708984377,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 5:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":-173.58001708984376,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":-173.58001708984376,\"y\":-241.7121124267578,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 6:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":-173.58001708984376,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":-340.96002197265627,\"y\":-204.47630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"Dagger\",\"position\":{\"x\":-213.83001708984376,\"y\":-241.7121124267578,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 7:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":-342.5599365234375,\"y\":-198.87632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"Slingshot\",\"position\":{\"x\":-215.17999267578126,\"y\":-161.31048583984376,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}}]}";
                break;
            case 8:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.8321533203125,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.55009460449219,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-44.8699951171875,\"y\":-81.04029846191406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":-4.62005615234375,\"y\":-0.7044830322265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 9:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720947265625,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31216430664063,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2721862792969,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.3121337890625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Hammer\",\"position\":{\"x\":161.34002685546876,\"y\":-82.57488250732422,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel2()
    {
        int r = -1;
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-89.1199951171875,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-130.2000732421875,\"y\":39.67127990722656,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-4.6400146484375,\"y\":122.46722412109375,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":79.8399658203125,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":164.32000732421876,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 1:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.785430908203128,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.785430908203128,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":79.8399658203125,\"y\":42.131011962890628,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":164.32000732421876,\"y\":42.131011962890628,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-3.84002685546875,\"y\":39.25099182128906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Crossbow\",\"position\":{\"x\":-88.32000732421875,\"y\":39.25099182128906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 2:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79208374023438,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-3.84002685546875,\"y\":-41.08488464355469,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":164.32000732421876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":248.79998779296876,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 3:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22225952148438,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79208374023438,\"y\":-203.22225952148438,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":79.8399658203125,\"y\":-198.8765106201172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":207.72003173828126,\"y\":-201.3765106201172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-3.84002685546875,\"y\":-201.7565155029297,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 4:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":198.45724487304688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":198.45724487304688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":-258.08013916015627,\"y\":202.80300903320313,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-341.760009765625,\"y\":199.9229736328125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":-173.60003662109376,\"y\":202.80319213867188,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 5:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":37.785430908203128,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.785430908203128,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":-4.6400146484375,\"y\":42.13140869140625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slingshot\",\"position\":{\"x\":-130.70001220703126,\"y\":-0.638885498046875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":40.23109436035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}}]}";
                break;
            case 6:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":79.84002685546875,\"y\":122.46728515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":165.12005615234376,\"y\":120.56698608398438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.3199462890625,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.8399658203125,\"y\":79.63119506835938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
                break;
            case 7:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31210327148438,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.27215576171877,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":334.0799865722656,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Hammer\",\"position\":{\"x\":165.6400146484375,\"y\":-0.638885498046875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":79.84002685546875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 8:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60784912109376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.96701049804688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":240.30279541015626,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"LeatherBoots\",\"position\":{\"x\":-219.780029296875,\"y\":78.83111572265625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":-173.0,\"y\":199.70272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MagicWand\",\"position\":{\"x\":-257.48004150390627,\"y\":199.70272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
            case 9:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60784912109376,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Spear\",\"position\":{\"x\":-257.280029296875,\"y\":77.16693115234375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Sling\",\"position\":{\"x\":-173.5999755859375,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WoodenArmor\",\"position\":{\"x\":-130.199951171875,\"y\":200.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
                break;
        }
        return result;
    }       
    string Cave1BossDragonLevel3()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.785614013671878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-4.63995361328125,\"y\":-38.20457458496094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.7200927734375,\"y\":120.00717163085938,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":79.84005737304688,\"y\":42.13117980957031,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel4()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-4.63995361328125,\"y\":-38.20457458496094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.7200927734375,\"y\":120.00717163085938,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":162.43997192382813,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel5()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12142944335938,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55015563964844,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-4.63995361328125,\"y\":-38.20457458496094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.7200927734375,\"y\":120.00717163085938,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":162.43997192382813,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":79.8399658203125,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel6()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-122.88617706298828,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-122.88617706298828,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.785430908203128,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.785430908203128,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-4.63995361328125,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.7200927734375,\"y\":39.67127990722656,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":162.43997192382813,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-89.1199951171875,\"y\":-118.54060363769531,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sTail\",\"position\":{\"x\":162.43997192382813,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel7()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-122.88617706298828,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-122.88617706298828,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.785430908203128,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.785430908203128,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-4.63995361328125,\"y\":-118.54048156738281,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.7200927734375,\"y\":39.67127990722656,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":162.43997192382813,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-89.1199951171875,\"y\":-118.54060363769531,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sTail\",\"position\":{\"x\":162.43997192382813,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-3.84002685546875,\"y\":199.9227294921875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel8()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22195434570313,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-4.63995361328125,\"y\":-198.8763885498047,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.7200927734375,\"y\":-40.66462707519531,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":162.43997192382813,\"y\":39.6710205078125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-89.1199951171875,\"y\":-198.8765106201172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sTail\",\"position\":{\"x\":162.43997192382813,\"y\":-121.00059509277344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-3.84002685546875,\"y\":119.58682250976563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-88.32000732421875,\"y\":119.58682250976563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel9()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":249.5999755859375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22195434570313,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-214.17999267578126,\"y\":-80.24048614501953,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.719970703125,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":77.9599609375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-89.1199951171875,\"y\":-198.8765106201172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sTail\",\"position\":{\"x\":77.9599609375,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":334.0799865722656,\"y\":119.5869140625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-88.32000732421875,\"y\":119.58682250976563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":162.43997192382813,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel10()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":249.5999755859375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22195434570313,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-214.17999267578126,\"y\":-80.24048614501953,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.719970703125,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":77.9599609375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-89.1199951171875,\"y\":-198.8765106201172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sTail\",\"position\":{\"x\":77.9599609375,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-213.83001708984376,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-88.32000732421875,\"y\":119.58682250976563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":162.43997192382813,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":249.5999755859375,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":334.08001708984377,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel11()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":249.5999755859375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22195434570313,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-214.17999267578126,\"y\":-80.24048614501953,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.719970703125,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":77.9599609375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-89.1199951171875,\"y\":-198.8765106201172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sTail\",\"position\":{\"x\":77.9599609375,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-213.83001708984376,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-88.32000732421875,\"y\":119.58682250976563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":162.43997192382813,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":249.5999755859375,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":334.08001708984377,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel12()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":249.5999755859375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22195434570313,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-214.17999267578126,\"y\":-80.24048614501953,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.719970703125,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":77.9599609375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-89.1199951171875,\"y\":-198.8765106201172,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sTail\",\"position\":{\"x\":77.9599609375,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-213.83001708984376,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-88.32000732421875,\"y\":119.58682250976563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":162.43997192382813,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":249.5999755859375,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":334.08001708984377,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":289.0,\"y\":-161.37632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-214.67999267578126,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel13()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":249.5999755859375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22195434570313,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-45.719970703125,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":77.9599609375,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-87.51995849609375,\"y\":116.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Dragon`sTail\",\"position\":{\"x\":77.9599609375,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-257.280029296875,\"y\":-41.08470153808594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-172.79998779296876,\"y\":-41.08470153808594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":162.43997192382813,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":249.5999755859375,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":334.08001708984377,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":289.0,\"y\":-161.37632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-214.67999267578126,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireBody1\",\"position\":{\"x\":-213.48004150390626,\"y\":-200.17633056640626,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-88.32000732421875,\"y\":-204.82630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel14()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-42.55027770996094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":165.11996459960938,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79205322265626,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83203125,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79205322265626,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83203125,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83203125,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22195434570313,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-130.20001220703126,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":-6.52008056640625,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-172.0,\"y\":116.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Dragon`sTail\",\"position\":{\"x\":-6.52008056640625,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-341.76007080078127,\"y\":-41.08470153808594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-257.27996826171877,\"y\":-41.08470153808594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":77.9599609375,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":165.11993408203126,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":249.5999755859375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":204.52001953125,\"y\":-161.37632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-299.1600341796875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireBody1\",\"position\":{\"x\":-297.96002197265627,\"y\":-200.17633056640626,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-172.79998779296876,\"y\":-204.82630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"BlackClaw\",\"position\":{\"x\":292.199951171875,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }
    string Cave1BossDragonLevel15()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-42.55027770996094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":165.11996459960938,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79205322265626,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83203125,\"y\":118.12124633789063,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.79205322265626,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83203125,\"y\":-203.22207641601563,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83203125,\"y\":-42.55046081542969,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.55036926269531,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22195434570313,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-130.20001220703126,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":-6.52008056640625,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dragon`sLeg\",\"position\":{\"x\":-172.0,\"y\":116.86691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Dragon`sTail\",\"position\":{\"x\":-6.52008056640625,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-341.76007080078127,\"y\":-41.08470153808594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-257.27996826171877,\"y\":-41.08470153808594,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sWing\",\"position\":{\"x\":77.9599609375,\"y\":-40.66468811035156,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":165.11993408203126,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":204.52001953125,\"y\":-161.37632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dragon`sHead\",\"position\":{\"x\":-299.1600341796875,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireBody1\",\"position\":{\"x\":-297.96002197265627,\"y\":-200.17633056640626,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-172.79998779296876,\"y\":-204.82630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"BlackClaw\",\"position\":{\"x\":292.199951171875,\"y\":39.67109680175781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":291.5748291015625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireFlask1\",\"position\":{\"x\":291.5748291015625,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":286.199951171875,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
        switch (r)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "";
                break;
            case 2:
                result = "";
                break;
            case 3:
                result = "";
                break;
            case 4:
                result = "";
                break;
            case 5:
                result = "";
                break;
            case 6:
                result = "";
                break;
            case 7:
                result = "";
                break;
            case 8:
                result = "";
                break;
            case 9:
                result = "";
                break;
        }
        return result;
    }



    string Cave1BossDragon()
    {
        switch (lvlEnemy)
        {
            case 1:
                return Cave1BossDragonLevel1();
            case 2:    
                return Cave1BossDragonLevel2();
            case 3:    
                return Cave1BossDragonLevel3();
            case 4:    
                return Cave1BossDragonLevel4();
            case 5:    
                return Cave1BossDragonLevel5();
            case 6:    
                return Cave1BossDragonLevel6();
            case 7:    
                return Cave1BossDragonLevel7();
            case 8:    
                return Cave1BossDragonLevel8();
            case 9:    
                return Cave1BossDragonLevel9();
            case 10:   
                return Cave1BossDragonLevel10();
            case 11:   
                return Cave1BossDragonLevel11();
            case 12:   
                return Cave1BossDragonLevel12();
            case 13:   
                return Cave1BossDragonLevel13();
            case 14:   
                return Cave1BossDragonLevel14();
            case 15:   
                return Cave1BossDragonLevel15();
        }
        return "";
    }
}