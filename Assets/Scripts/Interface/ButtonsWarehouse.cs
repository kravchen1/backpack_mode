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
    public void ToggleWarehouse(int numberWarehouse)
    {
        switch (numberWarehouse)
        {
            case 1:
                warehouse1.SetActive(true);
                warehouse2.SetActive(false);
                warehouse3.SetActive(false);
                warehouse4.SetActive(false);
                warehouse5.SetActive(false);
                break;
            case 2:
                warehouse1.SetActive(false);
                warehouse2.SetActive(true);
                warehouse3.SetActive(false);
                warehouse4.SetActive(false);
                warehouse5.SetActive(false);
                openWarehouse2 = true;
                break;
            case 3:
                warehouse1.SetActive(false);
                warehouse2.SetActive(false);
                warehouse3.SetActive(true);
                warehouse4.SetActive(false);
                warehouse5.SetActive(false);
                openWarehouse3 = true;
                break;
            case 4:
                warehouse1.SetActive(false);
                warehouse2.SetActive(false);
                warehouse3.SetActive(false);
                warehouse4.SetActive(true);
                warehouse5.SetActive(false);
                openWarehouse4 = true;
                break;
            case 5:
                warehouse1.SetActive(false);
                warehouse2.SetActive(false);
                warehouse3.SetActive(false);
                warehouse4.SetActive(false);
                warehouse5.SetActive(true);
                openWarehouse5 = true;
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

        SceneManager.LoadScene("GenerateMapFortress1");
    }

}
