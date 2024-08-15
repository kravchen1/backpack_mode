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
        if (careHitsNow.Count > 0)
        {
            foreach (var careHit in careHitsNow)
            {
                careHit.raycastHit.collider.gameObject.SetActive(true);
            }
            careHitsNow.Clear();
        }

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

        //image.color. = 0.5f;
        //image.raycastTarget = false;
        //canvasGroup.blocksRaycasts = false;
        //transform.GetChild(0).localPosition;
        for (int i = 0; i < backpack.transform.childCount; i++) 
        {
            if (backpack.transform.GetChild(i).gameObject.name.Contains("Image"))
            {
                backpack.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = true;
            }
        }
    }


    public override void OnEndDrag(PointerEventData eventData)
    {
        needToRotate = false;
        image.color = imageColor;
        image.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;
        //var tre = careHits.AsQueryable().Distinct().Count();
        // var tre2 = careHits.AsQueryable().Distinct();
        CorrectEndPoint();
        
        for (int i = 0; i < backpack.transform.childCount; i++)
        {
            if (backpack.transform.GetChild(i).gameObject.name.Contains("Image"))
            {
                backpack.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = false;
            }
        }

        foreach (var careHit in careHits)
        {
            careHitsNow.Add(careHit);
            careHit.raycastHit.collider.gameObject.SetActive(false);
        }

        careHits.Clear();

    }
}