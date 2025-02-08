using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class MoveCamera : MonoBehaviour
{
    private GameObject player;
    public float minX = 155, maxX = 1735, minY = 58, maxY = 980;
    //public float durationMove = 1f;
   // private Coroutine currentCoroutine;
    private RectTransform playerRectTransform;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRectTransform = player.GetComponent<RectTransform>();
    }

    //public void MoveCameraMethod(Vector2 vector2, bool needSlowMove = true)
    //{
    //    if (needSlowMove)
    //    {
    //        if (vector2.x < minX && vector2.y < minY) //слева и снизу
    //        {
    //            if (currentCoroutine != null)
    //            {
    //                StopCoroutine(currentCoroutine);
    //            }
    //            currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, -300, -10800), durationMove));
    //            //gameObject.transform.localPosition = new Vector3(-535, -300, -10800);
    //        }
    //        else if (vector2.x < minX && vector2.y > maxY) //слева и сверху
    //        {
    //            if (currentCoroutine != null)
    //            {
    //                StopCoroutine(currentCoroutine);
    //            }
    //            currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, 300, -10800), durationMove));
    //            //gameObject.transform.localPosition = new Vector3(-535, 300, -10800);
    //        }
    //        else if (vector2.x > maxX && vector2.y < minY) //справа и снизу
    //        {
    //            if (currentCoroutine != null)
    //            {
    //                StopCoroutine(currentCoroutine);
    //            }
    //            currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, -300, -10800), durationMove));
    //            //gameObject.transform.localPosition = new Vector3(615, -300, -10800);
    //        }
    //        else if (vector2.x > maxX && vector2.y > maxY) //справа и сверху
    //        {
    //            if (currentCoroutine != null)
    //            {
    //                StopCoroutine(currentCoroutine);
    //            }
    //            currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, 300, -10800), durationMove));
    //            //gameObject.transform.localPosition = new Vector3(615, 300, -10800);
    //        }
    //        else if (vector2.x < minX) //слева
    //        {
    //            if (currentCoroutine != null)
    //            {
    //                StopCoroutine(currentCoroutine);
    //            }
    //            currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, vector2.y - 400, -10800), durationMove));
    //            //gameObject.transform.localPosition = new Vector3(-535, vector2.y - 400, -10800);
    //        }
    //        else if (vector2.y < minY) //снизу
    //        {
    //            if (currentCoroutine != null)
    //            {
    //                StopCoroutine(currentCoroutine);
    //            }
    //            currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, -300, -10800), durationMove));
    //            //gameObject.transform.localPosition = new Vector3(vector2.x - 885, -300, -10800);
    //        }
    //        else if (vector2.y > maxY) //сверху
    //        {
    //            if (currentCoroutine != null)
    //            {
    //                StopCoroutine(currentCoroutine);
    //            }
    //            currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, 300, -10800), durationMove));
    //            //gameObject.transform.localPosition = new Vector3(vector2.x - 885, 300, -10800);
    //        }
    //        else if (vector2.x > maxX) //справа
    //        {
    //            if (currentCoroutine != null)
    //            {
    //                StopCoroutine(currentCoroutine);
    //            }
    //            currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, vector2.y - 400, -10800), durationMove));
    //            //gameObject.transform.localPosition = new Vector3(615, vector2.y - 400, -10800);
    //        }
    //        else
    //        {
    //            if (currentCoroutine != null)
    //            {
    //                StopCoroutine(currentCoroutine);
    //            }
    //            currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, vector2.y - 400, -10800), durationMove));
    //            //gameObject.transform.localPosition = new Vector3(vector2.x - 885, vector2.y - 400, -10800);
    //        }
    //    }
    //    else
    //    {
    //        if (vector2.x < minX && vector2.y < minY) //слева и снизу
    //        {
    //            //MoveObject(gameObject.transform.localPosition, new Vector3(-535, -300, -10800), durationMove);
    //            gameObject.transform.localPosition = new Vector3(-535, -300, -10800);
    //        }
    //        else if (vector2.x < minX && vector2.y > maxY) //слева и сверху
    //        {
    //            //MoveObject(gameObject.transform.localPosition, new Vector3(-535, 300, -10800), durationMove);
    //            gameObject.transform.localPosition = new Vector3(-535, 300, -10800);
    //        }
    //        else if (vector2.x > maxX && vector2.y < minY) //справа и снизу
    //        {
    //            //MoveObject(gameObject.transform.localPosition, new Vector3(615, -300, -10800), durationMove);
    //            gameObject.transform.localPosition = new Vector3(615, -300, -10800);
    //        }
    //        else if (vector2.x > maxX && vector2.y > maxY) //справа и сверху
    //        {
    //            //MoveObject(gameObject.transform.localPosition, new Vector3(615, 300, -10800), durationMove);
    //            gameObject.transform.localPosition = new Vector3(615, 300, -10800);
    //        }
    //        else if (vector2.x < minX) //слева
    //        {
    //            //MoveObject(gameObject.transform.localPosition, new Vector3(-535, vector2.y - 400, -10800), durationMove);
    //            gameObject.transform.localPosition = new Vector3(-535, vector2.y - 400, -10800);
    //        }
    //        else if (vector2.y < minY) //снизу
    //        {
    //            //MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, -300, -10800), durationMove);
    //            gameObject.transform.localPosition = new Vector3(vector2.x - 885, -300, -10800);
    //        }
    //        else if (vector2.y > maxY) //сверху
    //        {
    //            //MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, 300, -10800), durationMove);
    //            gameObject.transform.localPosition = new Vector3(vector2.x - 885, 300, -10800);
    //        }
    //        else if (vector2.x > maxX) //справа
    //        {
    //            //MoveObject(gameObject.transform.localPosition, new Vector3(615, vector2.y - 400, -10800), durationMove);
    //            gameObject.transform.localPosition = new Vector3(615, vector2.y - 400, -10800);
    //        }
    //        else
    //        {
    //            //MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, vector2.y - 400, -10800), durationMove);
    //            gameObject.transform.localPosition = new Vector3(vector2.x - 885, vector2.y - 400, -10800);
    //        }
    //    }
    //}
    private void Update()
    {
        if (playerRectTransform.anchoredPosition.x > minX && playerRectTransform.anchoredPosition.x < maxX)
        {
            if (playerRectTransform.anchoredPosition.y > minY && playerRectTransform.anchoredPosition.y < maxY)
            {
                gameObject.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y, gameObject.transform.localPosition.z);
            }
            else
                gameObject.transform.localPosition = new Vector3(player.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);

        }
        else if (playerRectTransform.anchoredPosition.y > minY && playerRectTransform.anchoredPosition.y < maxY)
        {
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, player.transform.localPosition.y, gameObject.transform.localPosition.z);
        }
    }





    //// Корутина для плавного перемещения
    //private IEnumerator MoveObject(Vector3 startPosition, Vector3 targetPosition, float duration)
    //{
    //    float elapsedTime = 0; // Время, прошедшее с начала перехода
    //    while (elapsedTime < duration)
    //    {
    //        // Вычисляем текущую позицию
    //        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
    //        elapsedTime += Time.deltaTime; // Увеличиваем время
    //        yield return null; // Ждем следующего кадра
    //    }

    //    // Убедитесь, что объект точно на целевой позиции
    //    transform.localPosition = targetPosition;
    //}
}
