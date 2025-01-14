using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class MoveCamera : MonoBehaviour
{
    private GameObject player;
    private float minX = 450, minY = 250, maxX = 1550, maxY = 750;
    public float durationMove = 1f;
    private Coroutine currentCoroutine;
    private RectTransform playerRectTransform;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRectTransform = player.GetComponent<RectTransform>();
    }

    public void MoveCameraMethod(Vector2 vector2, bool needSlowMove = true)
    {
        if (needSlowMove)
        {
            if (vector2.x < minX && vector2.y < minY) //����� � �����
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, -300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(-535, -300, -10800);
            }
            else if (vector2.x < minX && vector2.y > maxY) //����� � ������
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, 300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(-535, 300, -10800);
            }
            else if (vector2.x > maxX && vector2.y < minY) //������ � �����
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, -300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(615, -300, -10800);
            }
            else if (vector2.x > maxX && vector2.y > maxY) //������ � ������
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, 300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(615, 300, -10800);
            }
            else if (vector2.x < minX) //�����
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, vector2.y - 400, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(-535, vector2.y - 400, -10800);
            }
            else if (vector2.y < minY) //�����
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, -300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(vector2.x - 885, -300, -10800);
            }
            else if (vector2.y > maxY) //������
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, 300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(vector2.x - 885, 300, -10800);
            }
            else if (vector2.x > maxX) //������
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, vector2.y - 400, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(615, vector2.y - 400, -10800);
            }
            else
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, vector2.y - 400, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(vector2.x - 885, vector2.y - 400, -10800);
            }
        }
        else
        {
            if (vector2.x < minX && vector2.y < minY) //����� � �����
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(-535, -300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(-535, -300, -10800);
            }
            else if (vector2.x < minX && vector2.y > maxY) //����� � ������
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(-535, 300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(-535, 300, -10800);
            }
            else if (vector2.x > maxX && vector2.y < minY) //������ � �����
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(615, -300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(615, -300, -10800);
            }
            else if (vector2.x > maxX && vector2.y > maxY) //������ � ������
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(615, 300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(615, 300, -10800);
            }
            else if (vector2.x < minX) //�����
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(-535, vector2.y - 400, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(-535, vector2.y - 400, -10800);
            }
            else if (vector2.y < minY) //�����
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, -300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(vector2.x - 885, -300, -10800);
            }
            else if (vector2.y > maxY) //������
            {
                //MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, 300, -10800), durationMove);
                gameObject.transform.localPosition = new Vector3(vector2.x - 885, 300, -10800);
            }
            else if (vector2.x > maxX) //������
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
        //if(player.transform.localPosition.x >= gameObject.transform.localPosition.x + 100)
        //{
        if (playerRectTransform.anchoredPosition.x > 155 && playerRectTransform.anchoredPosition.x < 1735)
        {
            if (playerRectTransform.anchoredPosition.y > 60 && playerRectTransform.anchoredPosition.y < 980)
            {
                gameObject.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y, gameObject.transform.localPosition.z);
            }
            else
                gameObject.transform.localPosition = new Vector3(player.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);

        }
        else if (playerRectTransform.anchoredPosition.y > 58 && playerRectTransform.anchoredPosition.y < 980)
        {
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, player.transform.localPosition.y, gameObject.transform.localPosition.z);
        }
            //gameObject.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y, gameObject.transform.localPosition.z);
        //}
    }
    // �������� ��� �������� �����������
    private IEnumerator MoveObject(Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0; // �����, ��������� � ������ ��������
        while (elapsedTime < duration)
        {
            // ��������� ������� �������
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime; // ����������� �����
            yield return null; // ���� ���������� �����
        }

        // ���������, ��� ������ ����� �� ������� �������
        transform.localPosition = targetPosition;
    }
}
