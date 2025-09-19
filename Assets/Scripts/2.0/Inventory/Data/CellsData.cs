using System;
using System.Collections.Generic;
using UnityEngine;

public class CellsData : MonoBehaviour
{
    [Header("Settings")]
    public string settingsKey = "InventoryData";

    [Header("References")]
    public List<Cell> cells = new List<Cell>();
    public Transform itemsParent;

    [Header("Prefabs")]
    public List<ItemNew> itemPrefabs = new List<ItemNew>();

    private DataJsonCellList dataJsonList = new DataJsonCellList();

    public void SaveData()
    {
        dataJsonList.inventoryDataJsonList.Clear();

        foreach (Cell cell in cells)
        {
            if (cell.nestedObject != null)
            {
                var item = cell.nestedObject.GetComponent<ItemNew>();
                if (item != null)
                {
                    dataJsonList.inventoryDataJsonList.Add(new DataCellJson(
                        cell.gameObject.name,
                        item.originalNamePrefab,
                        item.transform.eulerAngles.z
                    ));
                }
            }
        }

        string jsonCellsSave = JsonUtility.ToJson(dataJsonList);
        PlayerPrefs.SetString(settingsKey, jsonCellsSave);
        PlayerPrefs.Save();

        Debug.Log($"Data saved. Items: {dataJsonList.inventoryDataJsonList.Count}");
    }
}
