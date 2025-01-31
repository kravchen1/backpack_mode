//using UnityEngine;

//public class ArrowCollisionDetector : MonoBehaviour
//{
//    private Door currentDoor;
//    private bool itsFirstTime;
//    private void OnTriggerEnter2D(Collider2D collision)
//    {

//        // Проверяем, что стрелка пересекается с дверью 2
//        if (collision.gameObject != null && collision.gameObject != this.transform.parent.gameObject && collision.gameObject.CompareTag("MapDoor")
//            && !itsFirstTime)
//        {
//            currentDoor.nextDoors.Add(collision.gameObject);
//            //nextDoor = collision.gameObject;
//            Debug.Log($"Стрелка пересекает {collision.gameObject}");
//            // Здесь можно выполнить дополнительные действия
//            itsFirstTime = true;
//        }
//    }

//    private void Start()
//    {
//        itsFirstTime = false;
//        currentDoor = this.transform.parent.gameObject.GetComponent<Door>();
//    }
//}