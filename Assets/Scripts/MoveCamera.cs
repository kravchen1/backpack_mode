using System.Collections;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Player player;
    private float minX = 450, minY = 250, maxX = 1550, maxY = 750;
    public float durationMove = 1f;



    public void MoveCameraMethod(Vector2 vector2, bool needSlowMove = true)
    {
        if (needSlowMove)
        {
            if (vector2.x < minX && vector2.y < minY) //слева и снизу
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, -300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(-535, -300, -10800);
            }
            else if (vector2.x < minX && vector2.y > maxY) //слева и сверху
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, 300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(-535, 300, -10800);
            }
            else if (vector2.x > maxX && vector2.y < minY) //справа и снизу
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, -300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(615, -300, -10800);
            }
            else if (vector2.x > maxX && vector2.y > maxY) //справа и сверху
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, 300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(615, 300, -10800);
            }
            else if (vector2.x < minX) //слева
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, vector2.y - 400, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(-535, vector2.y - 400, -10800);
            }
            else if (vector2.y < minY) //снизу
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, -300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(vector2.x - 885, -300, -10800);
            }
            else if (vector2.y > maxY) //сверху
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, 300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(vector2.x - 885, 300, -10800);
            }
            else if (vector2.x > maxX) //справа
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, vector2.y - 400, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(615, vector2.y - 400, -10800);
            }
            else
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, vector2.y - 400, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(vector2.x - 885, vector2.y - 400, -10800);
            }
        }
        else
        {
            if (vector2.x < minX && vector2.y < minY) //слева и снизу
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(-535, -300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(-535, -300, -10800);
            }
            else if (vector2.x < minX && vector2.y > maxY) //слева и сверху
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(-535, 300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(-535, 300, -10800);
            }
            else if (vector2.x > maxX && vector2.y < minY) //справа и снизу
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(615, -300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(615, -300, -10800);
            }
            else if (vector2.x > maxX && vector2.y > maxY) //справа и сверху
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(615, 300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(615, 300, -10800);
            }
            else if (vector2.x < minX) //слева
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(-535, vector2.y - 400, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(-535, vector2.y - 400, -10800);
            }
            else if (vector2.y < minY) //снизу
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, -300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(vector2.x - 885, -300, -10800);
            }
            else if (vector2.y > maxY) //сверху
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, 300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(vector2.x - 885, 300, -10800);
            }
            else if (vector2.x > maxX) //справа
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(615, vector2.y - 400, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(615, vector2.y - 400, -10800);
            }
            else
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, vector2.y - 400, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(vector2.x - 885, vector2.y - 400, -10800);
            }
        }
    }
    private void Update()
    {
        
    }
    // Корутина для плавного перемещения
    private IEnumerator MoveObject(Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0; // Время, прошедшее с начала перехода

        while (elapsedTime < duration)
        {
            // Вычисляем текущую позицию
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
            Debug.Log(elapsedTime);
            elapsedTime += Time.deltaTime; // Увеличиваем время
            yield return null; // Ждем следующего кадра
        }

        // Убедитесь, что объект точно на целевой позиции
        transform.localPosition = targetPosition;
    }
}
