using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class MoveCamera : MonoBehaviour
{
    private GameObject player;
    public float minX = 155, maxX = 1735, minY = 58, maxY = 980;
    public float minScroolZ = 20f, maxScrollZ = 100f;
    public float countScroll = 1f;
    public float needCamSdvigX = 0;
    public float needCamSdvigY = 0;
    //public float durationMove = 1f;
    // private Coroutine currentCoroutine;
    private RectTransform playerRectTransform;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRectTransform = player.GetComponent<RectTransform>();
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("IgnoreMouse"));
        Camera.main.orthographicSize = PlayerPrefs.GetFloat("orthographicSize" + SceneManager.GetActiveScene().name, 100f);
    }

    
    private void DefaultUpdate()
    {
        if (playerRectTransform.anchoredPosition.x > minX && playerRectTransform.anchoredPosition.x < maxX)
        {
            if (playerRectTransform.anchoredPosition.y > minY && playerRectTransform.anchoredPosition.y < maxY)
            {
                gameObject.transform.localPosition = new Vector3(player.transform.localPosition.x + needCamSdvigX, player.transform.localPosition.y + needCamSdvigY, gameObject.transform.localPosition.z);
            }
            else
                gameObject.transform.localPosition = new Vector3(player.transform.localPosition.x + needCamSdvigX, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);

        }
        else if (playerRectTransform.anchoredPosition.y > minY && playerRectTransform.anchoredPosition.y < maxY)
        {
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, player.transform.localPosition.y + needCamSdvigY, gameObject.transform.localPosition.z);
        }
    }
    private void Update()
    {
        DefaultUpdate();

        // Получаем значение прокрутки колёсика мыши
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        if (scrollData < 0)
        {
            //up
            if (Camera.main.orthographicSize < maxScrollZ)
            {
                Camera.main.orthographicSize += countScroll;
                PlayerPrefs.SetFloat("orthographicSize" + SceneManager.GetActiveScene().name, Camera.main.orthographicSize);
            }
        }
        else if (scrollData > 0)
        {
            //down
            if(Camera.main.orthographicSize > minScroolZ)
            {
                Camera.main.orthographicSize -= countScroll;
                PlayerPrefs.SetFloat("orthographicSize" + SceneManager.GetActiveScene().name, Camera.main.orthographicSize);
            }
            
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
