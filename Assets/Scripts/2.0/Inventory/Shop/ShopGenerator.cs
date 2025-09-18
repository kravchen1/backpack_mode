using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopGenerator : MonoBehaviour
{
    [Header("Shop References")]
    public GameObject shopCanvas;
    public List<Cell> shopCells = new List<Cell>();


    [Header("Item Generation")]
    public GameObject[] itemPrefabs;
    public int maxShopItems = 8;

    private List<ItemNew> spawnedItems = new List<ItemNew>();
    private float generationTimer;
    private RectTransform canvasRect;
    private bool Generating = true;

    void Start()
    {
        //GenerateItems();
    }

    public void GenerateItems()
    {
        for(int i = 0; i < shopCells.Count() && Generating; i++)
        {
            if (shopCells[i].nestedObject == null)
            {
                int r = Random.Range(0, itemPrefabs.Count());
                ItemNew spawnedItem = Instantiate(itemPrefabs[r], shopCanvas.transform).GetComponent<ItemNew>();

                bool flag = true;

                for (int j = 0; j < spawnedItem.HeightCell && flag; j++)
                {
                    for (int i2 = i; i2 < i + spawnedItem.WidthCell && flag; i2++)
                    {
                        int index = (j * 10) + i2;
                        if (index < 100)
                        {
                            if (shopCells[index].nestedObject != null)
                            {
                                flag = false;
                            }
                        }
                        else
                        {
                            flag = false;
                            Generating = false;
                        }
                    }
                }
                if (flag)
                {
                    CorrectPosition(i, spawnedItem);
                }
                else
                {
                    Debug.Log("Попытка создать предмет на предмете");
                    Destroy(spawnedItem.gameObject);
                }
            }
        }
        
    }

    public void ClearItems()
    {
        Generating = true;
        foreach (var cell in shopCells)
        {
            cell.nestedObject = null;
        }
        for(int i = 0;i < spawnedItems.Count(); i++)
        {
            if (spawnedItems[i] != null)
            {
                Destroy(spawnedItems[i].gameObject);
            }
        }
    }

    public void CorrectPosition(int cellIndex, ItemNew spawnedItem)
    {
        Bounds cellsBounds = new Bounds(shopCells[cellIndex].transform.position, Vector3.zero);
        List<Cell> cellsTemp = new List<Cell>();
        for (int j = 0; j < spawnedItem.HeightCell; j++)
        {
            for (int i = cellIndex; i < cellIndex + spawnedItem.WidthCell; i++)
            {
                int index = (j * 10) + i;
                if (index < 100)
                {
                    cellsBounds.Encapsulate(shopCells[index].transform.position);
                    cellsTemp.Add(shopCells[index]);
                }
                else
                {
                    Debug.Log("Вышли за границу массива ячеек в магазине");
                    Destroy(spawnedItem.gameObject);
                    Generating = false;
                    break;
                }
            }
        }
        foreach (var cell in cellsTemp)
        {
            cell.nestedObject = spawnedItem.gameObject;
        }


        Bounds itemBounds = new Bounds(spawnedItem.itemColliders[0].bounds.center, Vector3.zero);
        foreach (var collider in spawnedItem.itemColliders)
        {
            itemBounds.Encapsulate(collider.bounds);
        }

        Vector3 centerOffset = cellsBounds.center - itemBounds.center;
        spawnedItem.transform.position += centerOffset;

        spawnedItems.Add(spawnedItem);
    }



}