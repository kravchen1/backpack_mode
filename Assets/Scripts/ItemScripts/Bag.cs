using static UnityEditor.Progress;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.U2D;

public class Bag : Item
{
    private GameObject backpack;
    private List<RaycastStructure> careHitsNow = new List<RaycastStructure>();

    void OnTriggerEnter2D(Collider2D col)
    {
        if(!col.gameObject.name.Contains("Image"))
            Debug.Log(col.gameObject.name);
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        foreach (var collider in itemColliders)
        {
            collider.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        var cellList = gameObject.GetComponentsInChildren<Cell>().ToList();
        foreach (var cell in cellList.Where(e => e.nestedObject != null))
        {
            cell.nestedObject.transform.SetParent(gameObject.transform);
        }


        var allCellsList = GameObject.Find("backpack").GetComponentsInChildren<Cell>().ToList();


        foreach (var cell in cellList)//GameObject.Find("backpack").GetComponentsInChildren<Cell>())
        {
            foreach (var cell2 in allCellsList.Where(e => e.nestedObject == cell.nestedObject && e.transform.parent != cell.transform.parent))//GameObject.Find("backpack").GetComponentsInChildren<Cell>())
            {
                cell2.nestedObject = null;
            }
        }
        //if (careHitsNow.Count > 0)
        //{
        //    foreach (var careHit in careHitsNow)
        //    {
        //        careHit.raycastHit.collider.gameObject.SetActive(true);
        //    }
        //    careHitsNow.Clear();
        //}

        if (firstTap)
        {
            firstTap = false;
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            backpack = GameObject.Find("backpack");
        }

        needToRotate = true;
        if (needToDynamic)
        {
            needToRotateToStartRotation = true;
        }
        else
        {

        }
        needToDynamic = false;
        for (int i = 0; i < backpack.transform.childCount; i++) 
        {
            if (backpack.transform.GetChild(i).gameObject.name.Contains("Image"))
            {
                backpack.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
    public override bool CorrectEndPoint()
    {
        if (careHits.Count() == colliderCount)
        {
            if (hits.Where(e => e.collider == null).Count() == 0)
            {
                var maxY = careHits[0].raycastHit.collider.transform.localPosition.y;
                Vector2 colliderPos = careHits[0].raycastHit.collider.transform.localPosition;
                //Vector2 colliderPos2 = careHits[0].raycastHit.collider.transform.position;

                for (int i = 1; i < careHits.Count; i++)
                {
                    if (careHits[i].raycastHit.collider.transform.localPosition.y >= maxY)
                    {
                        maxY = careHits[i].raycastHit.collider.transform.localPosition.y;
                    }
                }
                var newListCareHits = careHits.Where(e => e.raycastHit.collider.transform.localPosition.y == maxY).ToList();
                var minX = newListCareHits[0].raycastHit.collider.transform.localPosition.x;
                foreach (var careHit in newListCareHits)
                {
                    //Debug.Log(careHit.raycastHit.collider.transform.localPosition.x);
                    if (careHit.raycastHit.collider.transform.localPosition.y == maxY)
                    {
                        if (careHit.raycastHit.collider.transform.localPosition.x <= minX)
                        {
                            minX = careHit.raycastHit.collider.transform.localPosition.x;
                            colliderPos = careHit.raycastHit.collider.transform.localPosition;
                        }
                    }
                }
                rectTransform.SetParent(bagTransform);
                var offset = new Vector2(itemColliders[0].size.x / 2, -itemColliders[0].size.y / 2);
                //Debug.Log("localPosition: " + rectTransform.localPosition);
                //Debug.Log("position: " + rectTransform.position);
                //Debug.Log("colliderPos: " + colliderPos);
               // Debug.Log("colliderPos2: " + colliderPos2);
                //Debug.Log("offset: " + offset);
                rectTransform.localPosition = offset + colliderPos;
                needToDynamic = false;
                foreach (var careHit in careHits)
                {
                    careHit.raycastHit.collider.GetComponent<SpriteRenderer>().color = imageColor;
                }
            }
            return true;
        }
        else
        {
            needToDynamic = true;
            return false;
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        foreach (var collider in itemColliders)
        {
            collider.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        needToRotate = false;
        image.color = imageColor;
        //image.raycastTarget = true;
        //canvasGroup.blocksRaycasts = true;
        CorrectEndPoint();
        
        for (int i = 0; i < backpack.transform.childCount; i++)
        {
            if (backpack.transform.GetChild(i).gameObject.name.Contains("Image"))
            {
                backpack.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        var cellList = gameObject.GetComponentsInChildren<Cell>();
        foreach (var cell in cellList)
        {
            if(cell.nestedObject != null)
                cell.nestedObject.transform.SetParent(backpack.transform);
        }
        //foreach (var careHit in careHits)
        //{
        //    careHitsNow.Add(careHit);
        //    //careHit.raycastHit.collider.gameObject.SetActive(false);
        //}

        careHits.Clear();

    }
}