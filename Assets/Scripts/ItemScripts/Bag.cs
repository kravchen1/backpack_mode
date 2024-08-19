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
    private List<ObjectInCells> objectsInCells = new List<ObjectInCells>();

    void OnTriggerEnter2D(Collider2D col)
    {
        if(!col.gameObject.name.Contains("Image"))
            Debug.Log(col.gameObject.name);
    }

    //var allCellsList = backpack.GetComponentsInChildren<Cell>().ToList();
    public void StayParentForChild()
    {
        var cellList = gameObject.GetComponentsInChildren<Cell>().ToList();
        foreach (var cell in cellList.Where(e => e.nestedObject != null))
        {
            cell.nestedObject.transform.SetParent(gameObject.transform);
            objectsInCells.Add(new ObjectInCells(cell.nestedObject.GetComponent<Item>()));
        }
    }

    public new void TapFirst()
    {
        if (firstTap)
        {
            firstTap = false;
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            backpack = GameObject.Find("backpack");
        }
    }
    public new void TapRotate()
    {
        needToRotate = true;
        if (needToDynamic)
        {
            needToRotateToStartRotation = true;
        }
        needToDynamic = false;
    }

    public void TapShowBackPack()
    {
        for (int i = 0; i < backpack.transform.childCount; i++)
        {
            if (backpack.transform.GetChild(i).gameObject.name.Contains("Image"))
            {
                backpack.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        StayParentForChild();
       
        TapFirst();
        TapRotate();
        TapShowBackPack();
    }


    public void CreateRaycast()
    {
        foreach (var collider in itemColliders)
        {
            hits.Add(Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, 128));
        }
    }
    public override void CreateCareRayñast()
    {
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (careHits.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
                {
                    careHits.Add(new RaycastStructure(hit));//îáúåêòû
                    bagTransform = hit.transform.parent.transform;
                }
            }
        }
        foreach (var hit in hitsForBackpack)
        {
            if (hit.collider != null)
            {
                if (careHitsForBackpack.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
                {
                    careHitsForBackpack.Add(new RaycastStructure(hit));//îáúåêòû
                }
            }
        }
    }
    public override void RaycastEvent()
    {
        hits.Clear();
        hitsForBackpack.Clear();

        foreach(var objectInCell in objectsInCells)
        {
            objectInCell.gameObject.hits = objectInCell.gameObject.CreateRaycast(256);
            objectInCell.gameObject.hitsForBackpack = objectInCell.gameObject.CreateRaycast(128);
            objectInCell.gameObject.ClearCareRaycast();
            objectInCell.gameObject.CreateCareRayñast();
            //Item:
            //hitsForBackpack = CreateRaycast(128);
            //hits = CreateRaycast(256);
        }

        CreateRaycast();

        ClearCareRaycast();
        CreateCareRayñast();
    }
    public void ChangeColorMyCells()
    {
        if (careHits.Count() == colliderCount)
        {
            foreach (var collider in itemColliders)
            {
                collider.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
        else
            foreach (var collider in itemColliders)
            {
                collider.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        RaycastEvent();
        ChangeColorMyCells();
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
    public void ChangeColorToDefault()
    {
        foreach (var collider in itemColliders)
        {
            collider.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    public void DisableBackpackCells()
    {
        for (int i = 0; i < backpack.transform.childCount; i++)
        {
            if (backpack.transform.GetChild(i).gameObject.name.Contains("Image"))
            {
                backpack.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
    public void ClearParentForChild()
    {
        var cellList = gameObject.GetComponentsInChildren<Cell>();
        foreach (var cell in cellList)
        {
            if (cell.nestedObject != null)
            {
                cell.nestedObject.transform.SetParent(backpack.transform);
            }
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        ChangeColorToDefault();
        needToRotate = false;
        CorrectEndPoint();
        DisableBackpackCells();
        ClearParentForChild();
        careHits.Clear();



        var cellList = gameObject.GetComponentsInChildren<Cell>().ToList();
        foreach (var cell in cellList.Where(e => e.nestedObject != null))
        {
            cell.nestedObject = null ;
            //objectsInCells.Add(new ObjectInCells(cell.nestedObject.GetComponent<Item>()));
        }
        
        foreach (var objectInCell in objectsInCells)
        {
            //objectInCell.gameObject.OnEndDrag(eventData);
            if (!objectInCell.gameObject.CorrectEndPoint())
            {
                objectInCell.gameObject.transform.SetParent(backpack.transform);
                objectInCell.gameObject.needToDynamic = true;
            }
        }
    }
}