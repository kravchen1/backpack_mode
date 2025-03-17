using System.Collections.Generic;
using UnityEngine;

public class BackpackController : MonoBehaviour
{
    //7*9 = 63 €чейки
    public GameObject cell1_1GO;
    public GameObject cell1_2GO;
    public GameObject cell1_3GO;
    public GameObject cell1_4GO;
    public GameObject cell1_5GO;
    public GameObject cell1_6GO;
    public GameObject cell1_7GO;
    public GameObject cell1_8GO;
    public GameObject cell1_9GO;
    public GameObject cell2_1GO;
    public GameObject cell2_2GO;
    public GameObject cell2_3GO;
    public GameObject cell2_4GO;
    public GameObject cell2_5GO;
    public GameObject cell2_6GO;
    public GameObject cell2_7GO;
    public GameObject cell2_8GO;
    public GameObject cell2_9GO;
    public GameObject cell3_1GO;
    public GameObject cell3_2GO;
    public GameObject cell3_3GO;
    public GameObject cell3_4GO;
    public GameObject cell3_5GO;
    public GameObject cell3_6GO;
    public GameObject cell3_7GO;
    public GameObject cell3_8GO;
    public GameObject cell3_9GO;
    public GameObject cell4_1GO;
    public GameObject cell4_2GO;
    public GameObject cell4_3GO;
    public GameObject cell4_4GO;
    public GameObject cell4_5GO;
    public GameObject cell4_6GO;
    public GameObject cell4_7GO;
    public GameObject cell4_8GO;
    public GameObject cell4_9GO;
    public GameObject cell5_1GO;
    public GameObject cell5_2GO;
    public GameObject cell5_3GO;
    public GameObject cell5_4GO;
    public GameObject cell5_5GO;
    public GameObject cell5_6GO;
    public GameObject cell5_7GO;
    public GameObject cell5_8GO;
    public GameObject cell5_9GO;
    public GameObject cell6_1GO;
    public GameObject cell6_2GO;
    public GameObject cell6_3GO;
    public GameObject cell6_4GO;
    public GameObject cell6_5GO;
    public GameObject cell6_6GO;
    public GameObject cell6_7GO;
    public GameObject cell6_8GO;
    public GameObject cell6_9GO;
    public GameObject cell7_1GO;
    public GameObject cell7_2GO;
    public GameObject cell7_3GO;
    public GameObject cell7_4GO;
    public GameObject cell7_5GO;
    public GameObject cell7_6GO;
    public GameObject cell7_7GO;
    public GameObject cell7_8GO;
    public GameObject cell7_9GO;


    private Cell cell1_1;
    private Cell cell1_2;
    private Cell cell1_3;
    private Cell cell1_4;
    private Cell cell1_5;
    private Cell cell1_6;
    private Cell cell1_7;
    private Cell cell1_8;
    private Cell cell1_9;
    private Cell cell2_1;
    private Cell cell2_2;
    private Cell cell2_3;
    private Cell cell2_4;
    private Cell cell2_5;
    private Cell cell2_6;
    private Cell cell2_7;
    private Cell cell2_8;
    private Cell cell2_9;
    private Cell cell3_1;
    private Cell cell3_2;
    private Cell cell3_3;
    private Cell cell3_4;
    private Cell cell3_5;
    private Cell cell3_6;
    private Cell cell3_7;
    private Cell cell3_8;
    private Cell cell3_9;
    private Cell cell4_1;
    private Cell cell4_2;
    private Cell cell4_3;
    private Cell cell4_4;
    private Cell cell4_5;
    private Cell cell4_6;
    private Cell cell4_7;
    private Cell cell4_8;
    private Cell cell4_9;
    private Cell cell5_1;
    private Cell cell5_2;
    private Cell cell5_3;
    private Cell cell5_4;
    private Cell cell5_5;
    private Cell cell5_6;
    private Cell cell5_7;
    private Cell cell5_8;
    private Cell cell5_9;
    private Cell cell6_1;
    private Cell cell6_2;
    private Cell cell6_3;
    private Cell cell6_4;
    private Cell cell6_5;
    private Cell cell6_6;
    private Cell cell6_7;
    private Cell cell6_8;
    private Cell cell6_9;
    private Cell cell7_1;
    private Cell cell7_2;
    private Cell cell7_3;
    private Cell cell7_4;
    private Cell cell7_5;
    private Cell cell7_6;
    private Cell cell7_7;
    private Cell cell7_8;
    private Cell cell7_9;

    private List<Cell> cells = new List<Cell>();

    private void Awake()
    {
        cell1_1 = cell1_1GO.GetComponent<Cell>();
        cell1_2 = cell1_2GO.GetComponent<Cell>();
        cell1_3 = cell1_3GO.GetComponent<Cell>();
        cell1_4 = cell1_4GO.GetComponent<Cell>();
        cell1_5 = cell1_5GO.GetComponent<Cell>();
        cell1_6 = cell1_6GO.GetComponent<Cell>();
        cell1_7 = cell1_7GO.GetComponent<Cell>();
        cell1_8 = cell1_8GO.GetComponent<Cell>();
        cell1_9 = cell1_9GO.GetComponent<Cell>();
        cell2_1 = cell2_1GO.GetComponent<Cell>();
        cell2_2 = cell2_2GO.GetComponent<Cell>();
        cell2_3 = cell2_3GO.GetComponent<Cell>();
        cell2_4 = cell2_4GO.GetComponent<Cell>();
        cell2_5 = cell2_5GO.GetComponent<Cell>();
        cell2_6 = cell2_6GO.GetComponent<Cell>();
        cell2_7 = cell2_7GO.GetComponent<Cell>();
        cell2_8 = cell2_8GO.GetComponent<Cell>();
        cell2_9 = cell2_9GO.GetComponent<Cell>();
        cell3_1 = cell3_1GO.GetComponent<Cell>();
        cell3_2 = cell3_2GO.GetComponent<Cell>();
        cell3_3 = cell3_3GO.GetComponent<Cell>();
        cell3_4 = cell3_4GO.GetComponent<Cell>();
        cell3_5 = cell3_5GO.GetComponent<Cell>();
        cell3_6 = cell3_6GO.GetComponent<Cell>();
        cell3_7 = cell3_7GO.GetComponent<Cell>();
        cell3_8 = cell3_8GO.GetComponent<Cell>();
        cell3_9 = cell3_9GO.GetComponent<Cell>();
        cell4_1 = cell4_1GO.GetComponent<Cell>();
        cell4_2 = cell4_2GO.GetComponent<Cell>();
        cell4_3 = cell4_3GO.GetComponent<Cell>();
        cell4_4 = cell4_4GO.GetComponent<Cell>();
        cell4_5 = cell4_5GO.GetComponent<Cell>();
        cell4_6 = cell4_6GO.GetComponent<Cell>();
        cell4_7 = cell4_7GO.GetComponent<Cell>();
        cell4_8 = cell4_8GO.GetComponent<Cell>();
        cell4_9 = cell4_9GO.GetComponent<Cell>();
        cell5_1 = cell5_1GO.GetComponent<Cell>();
        cell5_2 = cell5_2GO.GetComponent<Cell>();
        cell5_3 = cell5_3GO.GetComponent<Cell>();
        cell5_4 = cell5_4GO.GetComponent<Cell>();
        cell5_5 = cell5_5GO.GetComponent<Cell>();
        cell5_6 = cell5_6GO.GetComponent<Cell>();
        cell5_7 = cell5_7GO.GetComponent<Cell>();
        cell5_8 = cell5_8GO.GetComponent<Cell>();
        cell5_9 = cell5_9GO.GetComponent<Cell>();
        cell6_1 = cell6_1GO.GetComponent<Cell>();
        cell6_2 = cell6_2GO.GetComponent<Cell>();
        cell6_3 = cell6_3GO.GetComponent<Cell>();
        cell6_4 = cell6_4GO.GetComponent<Cell>();
        cell6_5 = cell6_5GO.GetComponent<Cell>();
        cell6_6 = cell6_6GO.GetComponent<Cell>();
        cell6_7 = cell6_7GO.GetComponent<Cell>();
        cell6_8 = cell6_8GO.GetComponent<Cell>();
        cell6_9 = cell6_9GO.GetComponent<Cell>();
        cell7_1 = cell7_1GO.GetComponent<Cell>();
        cell7_2 = cell7_2GO.GetComponent<Cell>();
        cell7_3 = cell7_3GO.GetComponent<Cell>();
        cell7_4 = cell7_4GO.GetComponent<Cell>();
        cell7_5 = cell7_5GO.GetComponent<Cell>();
        cell7_6 = cell7_6GO.GetComponent<Cell>();
        cell7_7 = cell7_7GO.GetComponent<Cell>();
        cell7_8 = cell7_8GO.GetComponent<Cell>();
        cell7_9 = cell7_9GO.GetComponent<Cell>();



        cells.Add(cell1_1);
        cells.Add(cell1_2);
        cells.Add(cell1_3);
        cells.Add(cell1_4);
        cells.Add(cell1_5);
        cells.Add(cell1_6);
        cells.Add(cell1_7);
        cells.Add(cell1_8);
        cells.Add(cell1_9);
        cells.Add(cell2_1);
        cells.Add(cell2_2);
        cells.Add(cell2_3);
        cells.Add(cell2_4);
        cells.Add(cell2_5);
        cells.Add(cell2_6);
        cells.Add(cell2_7);
        cells.Add(cell2_8);
        cells.Add(cell2_9);
        cells.Add(cell3_1);
        cells.Add(cell3_2);
        cells.Add(cell3_3);
        cells.Add(cell3_4);
        cells.Add(cell3_5);
        cells.Add(cell3_6);
        cells.Add(cell3_7);
        cells.Add(cell3_8);
        cells.Add(cell3_9);
        cells.Add(cell4_1);
        cells.Add(cell4_2);
        cells.Add(cell4_3);
        cells.Add(cell4_4);
        cells.Add(cell4_5);
        cells.Add(cell4_6);
        cells.Add(cell4_7);
        cells.Add(cell4_8);
        cells.Add(cell4_9);
        cells.Add(cell5_1);
        cells.Add(cell5_2);
        cells.Add(cell5_3);
        cells.Add(cell5_4);
        cells.Add(cell5_5);
        cells.Add(cell5_6);
        cells.Add(cell5_7);
        cells.Add(cell5_8);
        cells.Add(cell5_9);
        cells.Add(cell6_1);
        cells.Add(cell6_2);
        cells.Add(cell6_3);
        cells.Add(cell6_4);
        cells.Add(cell6_5);
        cells.Add(cell6_6);
        cells.Add(cell6_7);
        cells.Add(cell6_8);
        cells.Add(cell6_9);
        cells.Add(cell7_1);
        cells.Add(cell7_2);
        cells.Add(cell7_3);
        cells.Add(cell7_4);
        cells.Add(cell7_5);
        cells.Add(cell7_6);
        cells.Add(cell7_7);
        cells.Add(cell7_8);
        cells.Add(cell7_9);
    }

    private void ClearCells()
    {
        foreach(var cell in cells)
        {
            cell.nestedObject = null;
        }
    }


    private void SwitchPosition(float coutSwritchX, float coutSwritchY)
    {
        ClearCells();
        foreach (var item in gameObject.transform.GetComponentsInChildren<Item>())
        {
            item.DeleteNestedObject(gameObject.transform.parent.tag);
            item.rectTransform.anchoredPosition = new Vector2(item.rectTransform.anchoredPosition.x + coutSwritchX, item.rectTransform.anchoredPosition.y + coutSwritchY);
            Physics2D.SyncTransforms();

            if (item.name.ToUpper().Contains("BAG"))
            {
                item.RaycastEvent();
                item.SetNestedObject();
                gameObject.transform.SetAsFirstSibling();
            }
            item.RaycastEvent();
            item.SetNestedObject();
            item.ClearCareRaycast(false);
            //item.updateColorCells();
        }
    }



    private float countSwitchY = 0f, countSwitchX = 0f;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            countSwitchY = 80.3359f;
            countSwitchX = 0f;
            if ( cell1_1.nestedObject == null &&
                cell1_2.nestedObject == null &&
                cell1_3.nestedObject == null &&
                cell1_4.nestedObject == null &&
                cell1_5.nestedObject == null &&
                cell1_6.nestedObject == null &&
                cell1_7.nestedObject == null &&
                cell1_8.nestedObject == null &&
                cell1_9.nestedObject == null)
            {
                SwitchPosition(countSwitchX, countSwitchY);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            countSwitchY = -80.3359f;
            countSwitchX = 0f;
            if (cell7_1.nestedObject == null &&
                cell7_2.nestedObject == null &&
                cell7_3.nestedObject == null &&
                cell7_4.nestedObject == null &&
                cell7_5.nestedObject == null &&
                cell7_6.nestedObject == null &&
                cell7_7.nestedObject == null &&
                cell7_8.nestedObject == null &&
                cell7_9.nestedObject == null)
            {
                SwitchPosition(countSwitchX, countSwitchY);
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            countSwitchY = 0f;
            countSwitchX = -84.48f;
            if (cell1_1.nestedObject == null &&
                cell2_1.nestedObject == null &&
                cell3_1.nestedObject == null &&
                cell4_1.nestedObject == null &&
                cell5_1.nestedObject == null &&
                cell6_1.nestedObject == null &&
                cell7_1.nestedObject == null)
            {
                SwitchPosition(countSwitchX, countSwitchY);
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            countSwitchY = 0f;
            countSwitchX = 84.48f;
            if (cell1_9.nestedObject == null &&
                cell2_9.nestedObject == null &&
                cell3_9.nestedObject == null &&
                cell4_9.nestedObject == null &&
                cell5_9.nestedObject == null &&
                cell6_9.nestedObject == null &&
                cell7_9.nestedObject == null)
            {
                SwitchPosition(countSwitchX, countSwitchY);
            }
        }
    }
}

