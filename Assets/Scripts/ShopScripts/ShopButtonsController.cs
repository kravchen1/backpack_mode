//using Assets.Scripts.ItemScripts;
//using NUnit.Framework.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class ShopButtonsController : MonoBehaviour
//{

//    private CharacterStats stat;
//    private void Awake()
//    {
//        stat = GameObject.FindGameObjectWithTag("Stat").GetComponent<CharacterStats>();
//        PlayerPrefs.SetInt("priceReroll", 1);
//    }

//    public void ButtonReroll()
//    {
//        if (stat.playerCoins - PlayerPrefs.GetInt("priceReroll") >= 0)
//        {
//            var listShopData = GameObject.FindObjectsByType<GenerateShopItems>(FindObjectsSortMode.None);
//            //bool rerolling = false;
//            foreach (var data in listShopData)
//            {
//                if (!data.GetComponent<Price>().lockForItem.activeSelf)
//                {
//                    if (data.shopData.item != null)
//                    {
//                        Destroy(data.shopData.item.gameObject);
//                        data.GenerateRandomItem();
//                        //rerolling = true;
//                    }
//                    else
//                    {
//                        data.GenerateRandomItem();
//                        //rerolling = true;
//                    }
//                }
//            }
//            stat.playerCoins -= PlayerPrefs.GetInt("priceReroll");

//            //if (rerolling)
//            //{
//            //    stat.playerCoins -= PlayerPrefs.GetInt("priceReroll");
//            //    PlayerPrefs.SetInt("countRerollBeforePriceIncrease", PlayerPrefs.GetInt("countRerollBeforePriceIncrease") + 1);

//            //    if (PlayerPrefs.GetInt("countRerollBeforePriceIncrease") == countRerollBeforePriceIncrease)
//            //    {
//            //        PlayerPrefs.SetInt("countRerollBeforePriceIncrease", 0);
//            //        //PlayerPrefs.SetInt("priceReroll", PlayerPrefs.GetInt("priceReroll") + countIncrease);
//            //    }


//            //    textPrice.text = PlayerPrefs.GetInt("priceReroll").ToString();
//            //}


//        }
//    }


//    public void ButtonExitShopItem()
//    {
//        if (!DragManager.isReturnToOrgignalPos)
//        {
//            GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
//            //PlayerPrefs.SetString("ComputerName", System.Environment.MachineName.Replace("-", "_"));
//            //PlayerPrefs.SetInt("IdBackpack", PlayerPrefs.GetInt("IdBackpack") + 1);
//            GameObject.Find("backpack").GetComponent<BackpackData>().SaveNewData(Path.Combine(PlayerPrefs.GetString("savePathTestBackpack"), PlayerPrefs.GetString("ComputerName") + "_" + PlayerPrefs.GetInt("IdBackpack").ToString() + ".json"));

//            GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
//            GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
//            GameObject.Find("Shop").GetComponent<Shop>().SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "shopData.json"));

//            //SceneManager.LoadScene("GenerateMapFortress1");
//            SceneLoader.Instance.LoadScene("GenerateMapFortress1");
//        }
//    }

//    public void ButtonExitEatItem()
//    {
//        if (!DragManager.isReturnToOrgignalPos)
//        {
//            GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
//            GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
//            GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
//            GameObject.Find("Shop").GetComponent<Shop>().SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "shopDataEat.json"));

//            //SceneManager.LoadScene("GenerateMapFortress1");
//            SceneLoader.Instance.LoadScene("GenerateMapFortress1");
//        }
//    }

//    public void ButtonExitCave1()
//    {
//        if (!DragManager.isReturnToOrgignalPos)
//        {
//            GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
//            GameObject.FindWithTag("CaveStone").GetComponent<BackpackData>().SaveNewData(Path.Combine(PlayerPrefs.GetString("savePath"), "caveStoneData.json"));
//            GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
//            GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
//            //SceneManager.LoadScene("GenerateMap");
//            SceneLoader.Instance.LoadScene("GenerateMap");
//        }
//    }

//    public void ButtonExitCaveIn1()
//    {
//        if (!DragManager.isReturnToOrgignalPos)
//        {
//            GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
//            GameObject.FindWithTag("Shop").GetComponent<Shop>().SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "shopCave1Data.json"));
//            GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
//            GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
//            //SceneManager.LoadScene("GenerateMap");
//            SceneLoader.Instance.LoadScene(PlayerPrefs.GetString("currentLocation"));
//        }
//    }

//    public void ButtonExitBackpack()
//    {
//        if (!DragManager.isReturnToOrgignalPos)
//        {
//            GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();

//            PlayerPrefs.SetString("ComputerName", System.Environment.MachineName.Replace("-", "_"));
//            PlayerPrefs.SetInt("IdBackpack", PlayerPrefs.GetInt("IdBackpack") + 1);
//            GameObject.Find("backpack").GetComponent<BackpackData>().SaveNewData(Path.Combine(PlayerPrefs.GetString("savePathTestBackpack"), PlayerPrefs.GetString("ComputerName") + "_" + PlayerPrefs.GetInt("IdBackpack").ToString() + ".json"));

//            GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
//            GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
//            //Debug.Log("Unload");
//            //SceneManager.UnloadSceneAsync("BackPack");
//            //SceneManager.LoadScene(PlayerPrefs.GetString("currentLocation"));
//            SceneLoader.Instance.LoadScene(PlayerPrefs.GetString("currentLocation"));
//        }
//    }


//    public void SaveDevBackpacks()
//    {
//        GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
//        GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
//    }
//}