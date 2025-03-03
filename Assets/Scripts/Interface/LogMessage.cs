using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LogMessage : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    //public GameObject nestedObject;
    //private List<GameObject> itemsObj;

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    //ChangeShowStars(true);
    //    Debug.Log(gameObject.name + " вошли");
    //    itemsObj = GameObject.FindGameObjectsWithTag("RareWeapon").ToList();
    //    itemsObj.AddRange(GameObject.FindGameObjectsWithTag("Bag").ToList());
    //    itemsObj.AddRange(GameObject.FindGameObjectsWithTag("StartBag").ToList());

    //    foreach (var item in itemsObj)
    //    {
    //        if (item != nestedObject)
    //            item.GetComponent<SpriteRenderer>().color = new Color(100f, 100f, 100f);
    //    }
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    Debug.Log(gameObject.name + " вышли");
    //    foreach (var item in itemsObj)
    //    {
    //        item.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f);
    //    }
    //}
}