using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;

public abstract class Item : MonoBehaviour
{
    private string Name;

    private RectTransform rectTransform;

    //лучи
    private List<Collider2D> itemColliders = new List<Collider2D>();
    private List<RaycastHit2D> hits = new List<RaycastHit2D>();
    private List<RaycastStructure> careHits = new List<RaycastStructure>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        var collidersArray = rectTransform.GetComponents<Collider2D>();
        for (int i = 0; i < collidersArray.Count(); i++)
        {
            itemColliders.Add(collidersArray[i]);
        }
    }

    void Update()
    {
        //PointerEventData eventData2;
        //DragAndDrop.OnBeginDrag(eventData2);
    }

    void CreateRaycast(List<Collider2D> itemColliders)
    {
        foreach (var collider in itemColliders)
        {
            hits.Add(Physics2D.Raycast(collider.bounds.center, -Vector2.up));
        }
    }

}
