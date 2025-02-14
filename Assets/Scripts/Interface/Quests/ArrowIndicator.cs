using UnityEngine;
using UnityEngine.UI;

public class ArrowIndicator : MonoBehaviour
{
    public Transform target; // ������� ������, �� ������� ��������� �������
    public Transform player; // ����� (��� ������)
    public RectTransform arrowRectTransform; // RectTransform �������
    public float edgeBuffer = 50f; // ������ �� ���� ������

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

        // �������� ������� �������� ������� �� ������
        Vector3 targetScreenPosition = mainCamera.WorldToScreenPoint(target.position);

        // ���������, ��������� �� ������ �� ������
        bool isOffScreen = targetScreenPosition.x <= 0 || targetScreenPosition.x >= Screen.width ||
                           targetScreenPosition.y <= 0 || targetScreenPosition.y >= Screen.height;

        if (isOffScreen)
        {
            // ���������� �������
            arrowRectTransform.gameObject.SetActive(true);

            // ��������� ����������� � �������
            Vector3 direction = (target.position - player.position).normalized;

            // ��������� ���� �������� �������
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);



            // ������������� ������� �� ���� ������
            //Vector3 screenCenter = new Vector3(mainCamera.rect.width / 2, mainCamera.rect.height / 2, 0);
            //Vector3 screenBounds = screenCenter + new Vector3(edgeBuffer, edgeBuffer, 0);

            //targetScreenPosition.x = Mathf.Clamp(targetScreenPosition.x, edgeBuffer, Screen.width - edgeBuffer);
            //targetScreenPosition.y = Mathf.Clamp(targetScreenPosition.y, edgeBuffer, Screen.height - edgeBuffer);

            //arrowRectTransform.position = targetScreenPosition;
        }
        else
        {
            // �������� �������, ���� ������ �� ������
            arrowRectTransform.gameObject.SetActive(false);
        }
    }
}