using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class MoveMiniMapCamera : MonoBehaviour
{
    private GameObject player;

    private RectTransform playerRectTransform;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRectTransform = player.GetComponent<RectTransform>();
    }

    
    private void DefaultUpdate()
    {
        gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, gameObject.transform.position.z);
    }
    private void Update()
    {
        DefaultUpdate();
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
