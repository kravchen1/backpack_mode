using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : EventParent
{
    private GameObject player;
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
    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        lvlText.text = "lvl. " + lvlEnemy.ToString();
        if(currentSceneName == "GenerateMap")
            map = GameObject.FindGameObjectWithTag("GoMap");
        else
            map = GameObject.FindGameObjectWithTag("Cave");
    }
    private void OnTriggerEnter2D()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        isPlayerInTrigger = true;
        if (isShowPressE)
        {
            GetComponent<AudioSource>().Play();
            SetActivePressE(isShowPressE);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInTrigger = false;
        SetActivePressE(false);
    }

    public void ActivateEnemy()
    {
        PlayerPrefs.SetFloat("PostionMapX", player.GetComponent<RectTransform>().anchoredPosition.x);
        PlayerPrefs.SetFloat("PostionMapY", player.GetComponent<RectTransform>().anchoredPosition.y);
        PlayerPrefs.SetString("currentLocation", SceneManager.GetActiveScene().name);

        StartBattle();
    }



    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE)
        {
            ActivateEnemy();
        }
    }



    //private void OnMouseEnter()
    //{
    //    if (PlayerPrefs.GetInt("clickEnemy") == 0)
    //    {
    //        if (canvasBackpackEnemy == null)
    //        {
    //            canvasBackpackEnemy = GameObject.FindGameObjectWithTag("backpack");
    //            if (canvasBackpackEnemy != null)
    //            {
    //                generateBackpackOnMap = canvasBackpackEnemy.GetComponent<GenerateBackpackOnMap>();
    //                generateBackpackOnMap.Generate(enemyJSON);
    //            }
    //        }
    //        else
    //        {
    //            if (canvasBackpackEnemy != null)
    //            {
    //                generateBackpackOnMap.Generate(enemyJSON);
    //            }
    //        }
    //    }
    //}

    //private void OnMouseExit()
    //{
    //    if (PlayerPrefs.GetInt("clickEnemy") == 0)
    //        if (canvasBackpackEnemy != null)
    //        {
    //            generateBackpackOnMap.ClearBackpackObjects();
    //        }
    //}

    private bool click = false;
    public void OnMouseUp()
    {
        click = !click;
        //if(PlayerPrefs.GetInt("clickEnemy") == 0)
        if(click)
        {
            PlayerPrefs.SetInt("clickEnemy", 1);
            
            if (canvasBackpackEnemy == null)
            {
                canvasBackpackEnemy = GameObject.FindGameObjectWithTag("backpack");
                if (canvasBackpackEnemy != null)
                {
                    generateBackpackOnMap = canvasBackpackEnemy.GetComponent<GenerateBackpackOnMap>();
                    generateBackpackOnMap.ClearBackpackObjects();
                    generateBackpackOnMap.Generate(enemyJSON);
                }
            }
            else
            {
                if (canvasBackpackEnemy != null)
                {
                    generateBackpackOnMap.ClearBackpackObjects();
                    generateBackpackOnMap.Generate(enemyJSON);
                }
            }
        }
        else
        {
            PlayerPrefs.SetInt("clickEnemy", 0);
            generateBackpackOnMap.ClearBackpackObjects();
        }
    }



    private void EndDieAnimation()
    {
        if (dropItems.Count > 0 && dropItems.Count == probabilityDropItems.Count)
        {
            for (int i = 0; i < dropItems.Count; i++)
            {
                float r = Random.Range(1.0f, 100.0f);

                if (dropItems[i].GetComponent<DropItem>().item.CompareTag("KeyStonesItems") && dropItems[i].GetComponent<DropItem>().item.GetComponent<CaveStonesKeys>().stoneLevel == PlayerPrefs.GetInt("caveEnemyLvl")+1)
                    r = 0;
                if (r <= probabilityDropItems[i]) 
                {
                    Debug.Log(dropItems[i].name + "  loot " + r);
                    if(SceneManager.GetActiveScene().name == "Cave")
                        Instantiate(dropItems[i], gameObject.transform.position + new Vector3(-300,-200,0), Quaternion.identity, map.GetComponent<RectTransform>().transform);
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
        Invoke("EndDieAnimation", 1f);
    }

    public void JSONBackpackInitialized()
    {
        enemyJSON = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-81.04045104980469,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":39.6099853515625,\"y\":-161.37620544433595,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

        if (gameObject.tag == "EnemyCave1")
        {
            enemyJSON = Cave1Enemy();
        }
        else if (gameObject.tag == "EnemyGlobalMap1")
        {
            enemyJSON = GlobalMap1Enemy(); //enemyJSON = switch lvl mob
        }
        else if (gameObject.tag == "BossCave1")
        {
            enemyJSON = GlobalMap1Enemy(); //todo
            //BossCave1Enemy();
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
        PlayerPrefs.SetString("enemyName", gameObject.name);
        PlayerPrefs.SetInt("enemyLvl", lvlEnemy);

        PlayerPrefs.SetInt("enemyHP", startHP + ((lvlEnemy - 1) * stepHPForLevel));
        PlayerPrefs.SetFloat("enemyStamina", startStamina + ((lvlEnemy - 1) * stepStaminaForLevel));
        if(currentSceneName == "GenerateMap")
            PlayerPrefs.SetInt("enemyIdSpawner", idSpawner);

        

        PlayerPrefs.SetString("enemyBackpackJSON", enemyJSON);



        //SceneManager.LoadScene("BackPackBattle");
        SceneLoader.Instance.LoadScene("BackPackBattle");
    }




    string GlobalMap1EnemyLevel1()
    {
        int r = Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-81.04045104980469,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":39.6099853515625,\"y\":-161.37620544433595,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
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

    string GlobalMap1EnemyLevel2()
    {
        int r = Random.Range(0, 10);
        string result = "";
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

    string GlobalMap1EnemyLevel3()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":37.785552978515628,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slingshot\",\"position\":{\"x\":251.19998168945313,\"y\":198.802734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-4.0400390625,\"y\":39.0311279296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":117.239990234375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RevolverRing\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":117.239990234375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Sling\",\"position\":{\"x\":333.67999267578127,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Sling\",\"position\":{\"x\":249.20001220703126,\"y\":42.131103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
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

    string GlobalMap1EnemyLevel4()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":247.92001342773438,\"y\":-241.7121124267578,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Spear\",\"position\":{\"x\":248.81997680664063,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":247.92001342773438,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Spear\",\"position\":{\"x\":248.81997680664063,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-341.760009765625,\"y\":-197.97630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-172.79998779296876,\"y\":-197.97630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-304.86004638671877,\"y\":-80.84049987792969,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"Crossbow\",\"position\":{\"x\":-88.32000732421875,\"y\":-204.82630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Crossbow\",\"position\":{\"x\":-298.30999755859377,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
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

    string GlobalMap1EnemyLevel5()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":-299.15997314453127,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-341.760009765625,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-257.280029296875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-173.0,\"y\":-201.97630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-88.52001953125,\"y\":-201.97630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-129.3499755859375,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-129.3499755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-298.30999755859377,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
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

    string GlobalMap1EnemyLevel6()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":208.56997680664063,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":334.08001708984377,\"y\":-203.35116577148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlueCrystal\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":162.93997192382813,\"y\":-166.07630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":333.67999267578127,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":80.64013671875,\"y\":123.36691284179688,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":159.96697998046876,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":79.63113403320313,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":249.60009765625,\"y\":123.36691284179688,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-3.84002685546875,\"y\":-201.7563018798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Sling\",\"position\":{\"x\":118.1400146484375,\"y\":-242.1121063232422,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":249.5999755859375,\"y\":-241.7121124267578,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
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

    string GlobalMap1EnemyLevel7()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-42.55027770996094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":165.1199951171875,\"y\":-42.55024719238281,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":-90.50006103515625,\"y\":-5.404693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AutomaticCrossbow\",\"position\":{\"x\":-170.73004150390626,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBody1\",\"position\":{\"x\":124.43994140625,\"y\":121.16693115234375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-90.50006103515625,\"y\":-85.74049377441406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":80.63995361328125,\"y\":-38.80470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":165.11993408203126,\"y\":-38.80470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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

    string GlobalMap1EnemyLevel8()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-42.55027770996094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":165.1199951171875,\"y\":-42.55023193359375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaBody1\",\"position\":{\"x\":124.43994140625,\"y\":121.16693115234375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-171.20001220703126,\"y\":118.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-257.280029296875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-136.199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-257.48004150390627,\"y\":-41.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-342.1600341796875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-136.199951171875,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-3.84014892578125,\"y\":-41.084686279296878,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":80.639892578125,\"y\":-0.7046966552734375,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":124.08993530273438,\"y\":-81.04046630859375,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-42.55027770996094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":165.1199951171875,\"y\":-42.55023193359375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":117.239990234375,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-259.46002197265627,\"y\":-5.404693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":-259.46002197265627,\"y\":-85.74049377441406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlackClaw\",\"position\":{\"x\":-299.15997314453127,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":-5.404693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":74.93109130859375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MudWorm\",\"position\":{\"x\":78.46002197265625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlackClaw\",\"position\":{\"x\":-214.67999267578126,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-51.719970703125,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-42.55027770996094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":165.1199951171875,\"y\":-42.55023193359375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaBody1\",\"position\":{\"x\":39.9599609375,\"y\":40.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":165.1199951171875,\"y\":-42.67955017089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":-257.280029296875,\"y\":-203.35116577148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MagicWand\",\"position\":{\"x\":-341.96002197265627,\"y\":-201.97630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-257.35003662109377,\"y\":-39.974700927734378,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-341.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-257.67999267578127,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlueCrystal\",\"position\":{\"x\":-172.79998779296876,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":-88.32000732421875,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":-172.79998779296876,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":122.6148681640625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaGloves1\",\"position\":{\"x\":-46.34515380859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-129.3499755859375,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-134.699951171875,\"y\":-241.7120819091797,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04047393798828,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireGloves1\",\"position\":{\"x\":124.08999633789063,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":249.5999755859375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-213.83001708984376,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":-257.280029296875,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":-172.79998779296876,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":118.739990234375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-257.280029296875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":118.739990234375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":249.20001220703126,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":121.01998901367188,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireFlask1\",\"position\":{\"x\":-88.32000732421875,\"y\":37.65625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":334.0799865722656,\"y\":-3.1688995361328127,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":249.39996337890626,\"y\":119.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":-3.84002685546875,\"y\":37.65625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":122.6148681640625,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":333.8800048828125,\"y\":199.70272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Broom\",\"position\":{\"x\":-90.42999267578125,\"y\":234.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04046630859375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Hammer\",\"position\":{\"x\":-172.280029296875,\"y\":160.03274536132813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Hammer\",\"position\":{\"x\":-84.11004638671875,\"y\":-5.368896484375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-257.67999267578127,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Hammer\",\"position\":{\"x\":0.3699951171875,\"y\":155.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Hammer\",\"position\":{\"x\":163.83999633789063,\"y\":80.56692504882813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Hammer\",\"position\":{\"x\":250.1199951171875,\"y\":160.03274536132813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-50.82000732421875,\"y\":-161.7762908935547,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":80.63998413085938,\"y\":-120.44047546386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":165.1199951171875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RedCrystal\",\"position\":{\"x\":334.0799560546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RedCrystal\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-88.32000732421875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-257.280029296875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlueCrystal\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":338.7799987792969,\"y\":-2.7689056396484377,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RedCrystal\",\"position\":{\"x\":-257.280029296875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RedCrystal\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-88.32000732421875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-257.280029296875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":-257.280029296875,\"y\":-123.01533508300781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":-172.79998779296876,\"y\":-123.01533508300781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-171.20001220703126,\"y\":118.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Nibbler\",\"position\":{\"x\":38.759979248046878,\"y\":200.34274291992188,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireBody1\",\"position\":{\"x\":208.92001342773438,\"y\":40.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireBody1\",\"position\":{\"x\":39.9599609375,\"y\":40.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireDagger1\",\"position\":{\"x\":-88.32000732421875,\"y\":37.60111999511719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-172.79998779296876,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":124.08999633789063,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":118.1400146484375,\"y\":-161.7762908935547,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":334.0799560546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-219.780029296875,\"y\":239.90274047851563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Broom\",\"position\":{\"x\":247.489990234375,\"y\":234.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":334.0799865722656,\"y\":39.2510986328125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireGloves1\",\"position\":{\"x\":208.56997680664063,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-44.8699951171875,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-171.20001220703126,\"y\":198.802734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":-174.98004150390626,\"y\":-166.07630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBody1\",\"position\":{\"x\":293.39996337890627,\"y\":201.50274658203126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.67999267578127,\"y\":-198.87632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":334.0799560546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AutomaticCrossbow\",\"position\":{\"x\":-170.73004150390626,\"y\":39.67109680175781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":334.08001708984377,\"y\":37.65625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlackClaw\",\"position\":{\"x\":123.239990234375,\"y\":-121.00047302246094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":-3.84002685546875,\"y\":-123.01533508300781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":249.5999755859375,\"y\":-121.42048645019531,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-257.280029296875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-4.0400390625,\"y\":39.0311279296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":-174.98004150390626,\"y\":-85.74049377441406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RedCrystal\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-4.239990234375,\"y\":202.802734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MudWorm\",\"position\":{\"x\":80.63998413085938,\"y\":157.90274047851563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaGloves1\",\"position\":{\"x\":122.6148681640625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaRing1\",\"position\":{\"x\":249.5999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":37.785552978515628,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":121.31207275390625,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":32.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-136.199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-172.79998779296876,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-88.32000732421875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-3.84002685546875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
        int r = Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":247.92001342773438,\"y\":-241.7121124267578,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Spear\",\"position\":{\"x\":248.81997680664063,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":247.92001342773438,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Spear\",\"position\":{\"x\":248.81997680664063,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-341.760009765625,\"y\":-197.97630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-172.79998779296876,\"y\":-197.97630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-304.86004638671877,\"y\":-80.84049987792969,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071067094802856,\"w\":0.7071068286895752}},{\"name\":\"Crossbow\",\"position\":{\"x\":-88.32000732421875,\"y\":-204.82630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Crossbow\",\"position\":{\"x\":-298.30999755859377,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
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
    }//+

    string Cave1EnemyLevel2()
    {
        int r = Random.Range(0, 10);
        string result = "";
        switch (r)
        {
            case 0:
                result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPot\",\"position\":{\"x\":-299.15997314453127,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-341.760009765625,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-257.280029296875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-173.0,\"y\":-201.97630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-88.52001953125,\"y\":-201.97630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-129.3499755859375,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-129.3499755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":-298.30999755859377,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
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
    }//+

    string Cave1EnemyLevel3()
    {
        int r = -1;// Random.Range(0, 10);
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":334.0799865722656,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":205.7921142578125,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":208.56997680664063,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":334.08001708984377,\"y\":-203.35116577148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlueCrystal\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":162.93997192382813,\"y\":-166.07630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":333.67999267578127,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":80.64013671875,\"y\":123.36691284179688,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":159.96697998046876,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":165.1199951171875,\"y\":79.63113403320313,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":249.60009765625,\"y\":123.36691284179688,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":-3.84002685546875,\"y\":-201.7563018798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Sling\",\"position\":{\"x\":118.1400146484375,\"y\":-242.1121063232422,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":249.5999755859375,\"y\":-241.7121124267578,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-42.55027770996094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":165.1199951171875,\"y\":-42.55024719238281,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":-90.50006103515625,\"y\":-5.404693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AutomaticCrossbow\",\"position\":{\"x\":-170.73004150390626,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBody1\",\"position\":{\"x\":124.43994140625,\"y\":121.16693115234375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-90.50006103515625,\"y\":-85.74049377441406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":80.63995361328125,\"y\":-38.80470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":165.11993408203126,\"y\":-38.80470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-42.55027770996094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":165.1199951171875,\"y\":-42.55023193359375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaBody1\",\"position\":{\"x\":124.43994140625,\"y\":121.16693115234375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-171.20001220703126,\"y\":118.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-257.280029296875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":-136.199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"RabbitPaw\",\"position\":{\"x\":-257.48004150390627,\"y\":-41.30470275878906,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":-342.1600341796875,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-136.199951171875,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-3.84014892578125,\"y\":-41.084686279296878,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":80.639892578125,\"y\":-0.7046966552734375,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":124.08993530273438,\"y\":-81.04046630859375,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}}]}";
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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-42.55027770996094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":165.1199951171875,\"y\":-42.55023193359375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":117.239990234375,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-259.46002197265627,\"y\":-5.404693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":-259.46002197265627,\"y\":-85.74049377441406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlackClaw\",\"position\":{\"x\":-299.15997314453127,\"y\":-201.33628845214845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":-5.404693603515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":-6.02001953125,\"y\":74.93109130859375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MudWorm\",\"position\":{\"x\":78.46002197265625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlackClaw\",\"position\":{\"x\":-214.67999267578126,\"y\":120.00692749023438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-51.719970703125,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";
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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.12786865234376,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-301.08795166015627,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31204223632813,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-132.1279296875,\"y\":-42.55027770996094,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":165.1199951171875,\"y\":-42.55023193359375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaBody1\",\"position\":{\"x\":39.9599609375,\"y\":40.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":165.1199951171875,\"y\":-42.67955017089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":-257.280029296875,\"y\":-203.35116577148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MagicWand\",\"position\":{\"x\":-341.96002197265627,\"y\":-201.97630310058595,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-257.35003662109377,\"y\":-39.974700927734378,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-341.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"GauntletBoots\",\"position\":{\"x\":-257.67999267578127,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlueCrystal\",\"position\":{\"x\":-172.79998779296876,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":-88.32000732421875,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomOyster\",\"position\":{\"x\":-172.79998779296876,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":122.6148681640625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaGloves1\",\"position\":{\"x\":-46.34515380859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-129.3499755859375,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":-134.699951171875,\"y\":-241.7120819091797,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":8.429370268459025e-8,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04047393798828,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireGloves1\",\"position\":{\"x\":124.08999633789063,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":80.63998413085938,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":249.5999755859375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-213.83001708984376,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":-257.280029296875,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":-172.79998779296876,\"y\":117.9920654296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":118.739990234375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":-257.280029296875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Kettlebell\",\"position\":{\"x\":118.739990234375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":249.20001220703126,\"y\":-38.20469665527344,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":121.01998901367188,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"Magnifire\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"FireFlask1\",\"position\":{\"x\":-88.32000732421875,\"y\":37.65625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Spear\",\"position\":{\"x\":334.0799865722656,\"y\":-3.1688995361328127,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":249.39996337890626,\"y\":119.36691284179688,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":-3.84002685546875,\"y\":37.65625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBoots1\",\"position\":{\"x\":122.6148681640625,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MagicWand\",\"position\":{\"x\":333.8800048828125,\"y\":199.70272827148438,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Broom\",\"position\":{\"x\":-90.42999267578125,\"y\":234.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04046630859375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Hammer\",\"position\":{\"x\":-172.280029296875,\"y\":160.03274536132813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Hammer\",\"position\":{\"x\":-84.11004638671875,\"y\":-5.368896484375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-257.67999267578127,\"y\":122.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Hammer\",\"position\":{\"x\":0.3699951171875,\"y\":155.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Hammer\",\"position\":{\"x\":163.83999633789063,\"y\":80.56692504882813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Hammer\",\"position\":{\"x\":250.1199951171875,\"y\":160.03274536132813,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-50.82000732421875,\"y\":-161.7762908935547,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":80.63998413085938,\"y\":-120.44047546386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"MushroomWhite\",\"position\":{\"x\":165.1199951171875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RedCrystal\",\"position\":{\"x\":334.0799560546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RedCrystal\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-88.32000732421875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-257.280029296875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlueCrystal\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"GlamorousToad\",\"position\":{\"x\":338.7799987792969,\"y\":-2.7689056396484377,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RedCrystal\",\"position\":{\"x\":-257.280029296875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RedCrystal\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RevolverRing\",\"position\":{\"x\":-88.32000732421875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"w\":4.371138828673793e-8}},{\"name\":\"ManaRing1\",\"position\":{\"x\":-257.280029296875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":-257.280029296875,\"y\":-123.01533508300781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireFlask1\",\"position\":{\"x\":-172.79998779296876,\"y\":-123.01533508300781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-171.20001220703126,\"y\":118.4669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Nibbler\",\"position\":{\"x\":38.759979248046878,\"y\":200.34274291992188,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireBody1\",\"position\":{\"x\":208.92001342773438,\"y\":40.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireBody1\",\"position\":{\"x\":39.9599609375,\"y\":40.83110046386719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireDagger1\",\"position\":{\"x\":-88.32000732421875,\"y\":37.60111999511719,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":-172.79998779296876,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-3.84002685546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":124.08999633789063,\"y\":-81.04047393798828,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"HiddenDagger\",\"position\":{\"x\":118.1400146484375,\"y\":-161.7762908935547,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":334.0799560546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-219.780029296875,\"y\":239.90274047851563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Broom\",\"position\":{\"x\":247.489990234375,\"y\":234.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":334.0799865722656,\"y\":39.2510986328125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799865722656,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireGloves1\",\"position\":{\"x\":208.56997680664063,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":-44.8699951171875,\"y\":-161.3762969970703,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":1.0,\"w\":0.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":-0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Nunchucks\",\"position\":{\"x\":-171.20001220703126,\"y\":198.802734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":-174.98004150390626,\"y\":-166.07630920410157,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaBody1\",\"position\":{\"x\":293.39996337890627,\"y\":201.50274658203126,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":333.67999267578127,\"y\":-198.87632751464845,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Magnifire\",\"position\":{\"x\":334.0799560546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomChanterelle\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AutomaticCrossbow\",\"position\":{\"x\":-170.73004150390626,\"y\":39.67109680175781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaHelmet\",\"position\":{\"x\":334.08001708984377,\"y\":37.65625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"BlackClaw\",\"position\":{\"x\":123.239990234375,\"y\":-121.00047302246094,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaFlask1\",\"position\":{\"x\":-3.84002685546875,\"y\":-123.01533508300781,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaAmulet1\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"FireHelmet1\",\"position\":{\"x\":249.5999755859375,\"y\":-121.42048645019531,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Magnifire\",\"position\":{\"x\":-257.280029296875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"WitchPotSmall\",\"position\":{\"x\":-4.0400390625,\"y\":39.0311279296875,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MarbleMoth\",\"position\":{\"x\":-174.98004150390626,\"y\":-85.74049377441406,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"RedCrystal\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dinosaur\",\"position\":{\"x\":-4.239990234375,\"y\":202.802734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MudWorm\",\"position\":{\"x\":80.63998413085938,\"y\":157.90274047851563,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaGloves1\",\"position\":{\"x\":122.6148681640625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"ManaRing1\",\"position\":{\"x\":249.5999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"ManaRing1\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":37.785552978515628,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":121.31207275390625,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":32.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-136.199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-172.79998779296876,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-88.32000732421875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-3.84002685546875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":37.785552978515628,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":121.31207275390625,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":32.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-136.199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-172.79998779296876,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-88.32000732421875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-3.84002685546875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":37.785552978515628,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":121.31207275390625,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":32.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-136.199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-172.79998779296876,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-88.32000732421875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-3.84002685546875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
        string result = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag1x1Resistance\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":37.785552978515628,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-341.760009765625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-216.60791015625,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":-47.64788818359375,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-216.60791015625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-42.550262451171878,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":-47.64788818359375,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":198.4571533203125,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":37.78553771972656,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":121.31207275390625,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":121.31207275390625,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":240.302734375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":118.121337890625,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-0.704681396484375,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bag2x1Stamina\",\"position\":{\"x\":290.2720642089844,\"y\":-81.04045104980469,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"bagCommon2x2\",\"position\":{\"x\":290.2720642089844,\"y\":-203.22189331054688,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":32.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":249.5999755859375,\"y\":240.302734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-88.32000732421875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-172.79998779296876,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-257.280029296875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-172.79998779296876,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-88.32000732421875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":80.63998413085938,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":79.631103515625,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":249.5999755859375,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":-3.84002685546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-81.04048919677735,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":334.0799560546875,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":165.1199951171875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-241.71209716796876,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"AngryFluff\",\"position\":{\"x\":80.63998413085938,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-341.760009765625,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":-3.84002685546875,\"y\":159.9669189453125,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Slug\",\"position\":{\"x\":165.1199951171875,\"y\":-161.37631225585938,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-136.199951171875,\"y\":-0.7046966552734375,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.7071068286895752,\"w\":0.7071068286895752}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-172.79998779296876,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-88.32000732421875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":-3.84002685546875,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"MushroomRussula\",\"position\":{\"x\":80.63998413085938,\"y\":-117.64048767089844,\"z\":-1.999786376953125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";

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
}