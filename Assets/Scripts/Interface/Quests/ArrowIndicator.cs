using UnityEngine;
using UnityEngine.UI;

public class ArrowIndicator : MonoBehaviour
{
    public Transform target; // Целевой объект, на который указывает стрелка
    public Transform player; // Игрок (или камера)
    public RectTransform arrowRectTransform; // RectTransform стрелки
    public float edgeBuffer = 50f; // Отступ от края экрана

    public Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (target == null || player == null || arrowRectTransform == null)
            return;

        // Получаем позицию целевого объекта на экране
        Vector3 targetScreenPosition = mainCamera.WorldToScreenPoint(target.position);

        // Проверяем, находится ли объект на экране
        bool isOffScreen = targetScreenPosition.x <= 0 || targetScreenPosition.x >= Screen.width ||
                           targetScreenPosition.y <= 0 || targetScreenPosition.y >= Screen.height;

        if (isOffScreen)
        {
            // Показываем стрелку
            arrowRectTransform.gameObject.SetActive(true);

            // Вычисляем направление к объекту
            Vector3 direction = (target.position - player.position).normalized;

            // Вычисляем угол поворота стрелки
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);



            // Позиционируем стрелку на краю экрана
            //Vector3 screenCenter = new Vector3(mainCamera.rect.width / 2, mainCamera.rect.height / 2, 0);
            //Vector3 screenBounds = screenCenter + new Vector3(edgeBuffer, edgeBuffer, 0);

            //targetScreenPosition.x = Mathf.Clamp(targetScreenPosition.x, edgeBuffer, Screen.width - edgeBuffer);
            //targetScreenPosition.y = Mathf.Clamp(targetScreenPosition.y, edgeBuffer, Screen.height - edgeBuffer);

            //arrowRectTransform.position = targetScreenPosition;
        }
        else
        {
            // Скрываем стрелку, если объект на экране
            arrowRectTransform.gameObject.SetActive(false);
        }
    }
}