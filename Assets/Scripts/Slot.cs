using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var anchor = eventData.pointerDrag.transform.parent.transform;
        var otherItemTransform = eventData.pointerDrag.transform;
        var cellList = new List<Transform>();
        var vectorList = new List<Vector3>();
        //var test = eventData.pointerDrag.transform.localPosition - eventData.pointerDrag.transform.GetChild(0).localPosition;
        for (int i = 0; i < eventData.pointerDrag.transform.childCount; i++)
        {
            cellList.Add(eventData.pointerDrag.transform.GetChild(i));
        }
        var test = eventData.pointerDrag.transform.position - cellList[0].position;
        foreach (var cell in cellList)
        {
            var collider = cell.GetComponent<Collider2D>();
            List<Collider2D> result = new List<Collider2D>();
            Physics2D.OverlapCollider(collider, result);
            vectorList.Add(result[0].transform.position);
        }
        for(int i = 0; i < cellList.Count; i++)
        {
            cellList[i].position = vectorList[i];
        }
        eventData.pointerDrag.transform.position = cellList[0].position + test;
    }
}
