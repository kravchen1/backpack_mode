///*
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine.Audio;
//using UnityEditor.Rendering;

//public class DragAndDrop : Item, IBeginDragHandler, IEndDragHandler, IDragHandler 
//{
    
   


//    //private List<Transform> cellList = new List<Transform>();

//    private List<Vector2> vectorList = new List<Vector2>();
//   // private List<Collider2D> colliders = new List<Collider2D>();
//    private Vector3 offset;
//    private Transform bagTransform;
    

//    private void Awake()
//    {
//        rectTransform = GetComponent<RectTransform>();
//        //image = GetComponent<Image>();
//        canvas = GetComponentInParent<Canvas>();
//        //canvasGroup = GetComponent<CanvasGroup>();
//        //imageColor = image.color;
//    }

//    /*
//     // If no registred hitobject => Entering
//                if( hitObject == null )
//                {
//                    go.SendMessage ("OnHitEnter"); 
//                }
//                // If hit object is the same as the registered one => Stay
//                else if( hitObject.GetInstanceID() == go.GetInstanceID() )
//                {
//                    hitObject.SendMessage( "OnHitStay" );
//                }
//                // If new object hit => Exit last + Enter new
//                else
//                {
//                    hitObject.SendMessage( "OnHitExit" );
//                    go.SendMessage ("OnHitEnter");
//                }

//                hitting = true ;
//                hitObject = go ;
    
//    void Update()
//    {
//    }

    
 

 

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        var cellTransform = collision.GetComponent<RectTransform>();
//        bagTransform = cellTransform.parent.transform;
//        if (vectorList.Where(e => e == (Vector2)cellTransform.localPosition).Count() == 0 && vectorList.Count()<4)
//            vectorList.Add((Vector2)cellTransform.localPosition);
//        Debug.Log("GlobalPosition:" + (Vector2)cellTransform.position);
//        Debug.Log(collision.name + " Trigger " + this.name);
//        Debug.Log("OnTriggerEnterVectorListCount:" + vectorList.Count.ToString());
//       // collision.GetComponent<Image>().color = Color.red;
//        //this.GetComponent<Image>().color = new Color32(255, 255, 225, 100);
//        //PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
//        //// globaltest.text = PlayerPrefs.GetInt("level").ToString();
//        //PlayerPrefs.Save();
//    }

//    private void OnTriggerExit2D(Collider2D collision)
//    {
//        var cellTransform = collision.GetComponent<RectTransform>();
//        //Debug.Log("OnTriggerExitVectorListCount:" + vectorList.Count.ToString());
//        if(vectorList.Count() == 4)
//            vectorList.Remove(cellTransform.localPosition);
//        Debug.Log(collision.name + " AnTrigger " + this.name);
//        Debug.Log("OnTriggerExitVectorListCount:" + vectorList.Count.ToString());
//       // collision.GetComponent<Image>().color = imageColor;
//        //this.GetComponent<Image>().color = color;
//    }
//}*/
