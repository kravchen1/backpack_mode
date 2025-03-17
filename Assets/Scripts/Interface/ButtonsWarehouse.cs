using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsWarehouse : MonoBehaviour
{
    public GameObject warehouse1;
    public GameObject warehouse2;
    public GameObject warehouse3;
    public GameObject warehouse4;
    public GameObject warehouse5;

    public GameObject backpack;
    public GameObject stats;
    public GameObject storage;

    private bool openWarehouse1 = true;
    private bool openWarehouse2 = false;
    private bool openWarehouse3 = false;
    private bool openWarehouse4 = false;
    private bool openWarehouse5 = false;

    public GameObject button1, button2, button3, button4, button5;

    //void ReloadColliders(GameObject warehouse)
    //{
    //    for (int i = 0; i < warehouse.transform.childCount; i++)
    //    {
    //        warehouse.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
    //        warehouse.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
    //    }
    //}

    public void ToggleWarehouse(int numberWarehouse)
    {
        switch (numberWarehouse)
        {
            case 1:
                warehouse1.SetActive(true);
                button1.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);
                warehouse2.SetActive(false);
                button2.GetComponent<Image>().color = Color.white;
                warehouse3.SetActive(false);
                button3.GetComponent<Image>().color = Color.white;
                warehouse4.SetActive(false);
                button4.GetComponent<Image>().color = Color.white;
                warehouse5.SetActive(false);
                button5.GetComponent<Image>().color = Color.white;
                //ReloadColliders(warehouse1);
                break;
            case 2:
                warehouse1.SetActive(false);
                button1.GetComponent<Image>().color = Color.white;
                warehouse2.SetActive(true);
                button2.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);
                warehouse3.SetActive(false);
                button3.GetComponent<Image>().color = Color.white;
                warehouse4.SetActive(false);
                button4.GetComponent<Image>().color = Color.white;
                warehouse5.SetActive(false);
                button5.GetComponent<Image>().color = Color.white;
                openWarehouse2 = true;
                //ReloadColliders(warehouse2);
                break;
            case 3:
                warehouse1.SetActive(false);
                button1.GetComponent<Image>().color = Color.white;
                warehouse2.SetActive(false);
                button2.GetComponent<Image>().color = Color.white;
                warehouse3.SetActive(true);
                button3.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);
                warehouse4.SetActive(false);
                button4.GetComponent<Image>().color = Color.white;
                warehouse5.SetActive(false);
                button5.GetComponent<Image>().color = Color.white;
                openWarehouse3 = true;
                //ReloadColliders(warehouse3);
                break;
            case 4:
                warehouse1.SetActive(false);
                button1.GetComponent<Image>().color = Color.white;
                warehouse2.SetActive(false);
                button2.GetComponent<Image>().color = Color.white;
                warehouse3.SetActive(false);
                button3.GetComponent<Image>().color = Color.white;
                warehouse4.SetActive(true);
                button4.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);
                warehouse5.SetActive(false);
                button5.GetComponent<Image>().color = Color.white;
                openWarehouse4 = true;
                //ReloadColliders(warehouse4);
                break;
            case 5:
                warehouse1.SetActive(false);
                button1.GetComponent<Image>().color = Color.white;
                warehouse2.SetActive(false);
                button2.GetComponent<Image>().color = Color.white;
                warehouse3.SetActive(false);
                button3.GetComponent<Image>().color = Color.white;
                warehouse4.SetActive(false);
                button4.GetComponent<Image>().color = Color.white;
                warehouse5.SetActive(true);
                button5.GetComponent<Image>().color = new Color(0.676827f, 0.8396226f, 0.1544589f);
                openWarehouse5 = true;
                //ReloadColliders(warehouse5);
                break;
        }
        
    }

    public void ExitScene()
    {
        if(openWarehouse1)
            warehouse1.GetComponent<BackpackData>().SaveNewData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackWarehouse1.json"));
        if (openWarehouse2)
            warehouse2.GetComponent<BackpackData>().SaveNewData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackWarehouse2.json"));
        if (openWarehouse3)
            warehouse3.GetComponent<BackpackData>().SaveNewData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackWarehouse3.json"));
        if (openWarehouse4)
            warehouse4.GetComponent<BackpackData>().SaveNewData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackWarehouse4.json"));
        if (openWarehouse5)
            warehouse5.GetComponent<BackpackData>().SaveNewData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackWarehouse5.json"));


        backpack.GetComponent<BackpackData>().SaveData();
        stats.GetComponent<CharacterStats>().SaveData();
        storage.GetComponent<BackpackData>().SaveData();

        //SceneManager.LoadScene("GenerateMapFortress1");
        SceneLoader.Instance.LoadScene("GenerateMapFortress1");
    }

}
