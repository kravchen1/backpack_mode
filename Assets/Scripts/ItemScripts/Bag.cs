using static UnityEditor.Progress;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Bag : Item
{
    private GameObject backpack;
    private List<RaycastStructure> careHitsNow = new List<RaycastStructure>();
    public override void OnBeginDrag(PointerEventData eventData)
    {
        foreach (var collider in itemColliders)
        {
            collider.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        }
        foreach (var go in gameObject.GetComponentsInChildren<Cell>().Where(e => e.nestedObject != null))
        {
            go.nestedObject.transform.SetParent(gameObject.transform);
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
                backpack.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = true;
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
                    Debug.Log(careHit.raycastHit.collider.transform.localPosition.x);
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
                rectTransform.localPosition = offset + colliderPos;
                needToDynamic = false;
                foreach (var careHit in careHits)
                {
                    careHit.raycastHit.collider.GetComponent<UnityEngine.UI.Image>().color = imageColor;
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
            collider.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
        needToRotate = false;
        image.color = imageColor;
        image.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;
        CorrectEndPoint();
        
        for (int i = 0; i < backpack.transform.childCount; i++)
        {
            if (backpack.transform.GetChild(i).gameObject.name.Contains("Image"))
            {
                backpack.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = false;
            }
        }
        foreach (var go in gameObject.GetComponentsInChildren<Cell>().Where(e => e.nestedObject != null))
        {
            go.nestedObject.transform.SetParent(backpack.transform);
        }
        //foreach (var careHit in careHits)
        //{
        //    careHitsNow.Add(careHit);
        //    //careHit.raycastHit.collider.gameObject.SetActive(false);
        //}

        careHits.Clear();

    }
}