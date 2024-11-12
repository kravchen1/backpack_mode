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
            if (vector2.x < minX && vector2.y < minY) //����� � �����
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, -300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(-535, -300, -10800);
            }
            else if (vector2.x < minX && vector2.y > maxY) //����� � ������
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, 300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(-535, 300, -10800);
            }
            else if (vector2.x > maxX && vector2.y < minY) //������ � �����
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, -300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(615, -300, -10800);
            }
            else if (vector2.x > maxX && vector2.y > maxY) //������ � ������
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(615, 300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(615, 300, -10800);
            }
            else if (vector2.x < minX) //�����
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(-535, vector2.y - 400, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(-535, vector2.y - 400, -10800);
            }
            else if (vector2.y < minY) //�����
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, -300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(vector2.x - 885, -300, -10800);
            }
            else if (vector2.y > maxY) //������
            {
                StartCoroutine(MoveObject(gameObject.transform.localPosition, new Vector3(vector2.x - 885, 300, -10800), durationMove));
                //gameObject.transform.localPosition = new Vector3(vector2.x - 885, 300, -10800);
            }
            else if (vector2.x > maxX) //������
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
        
    }
    // �������� ��� �������� �����������
    private IEnumerator MoveObject(Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0; // �����, ��������� � ������ ��������

        while (elapsedTime < duration)
        {
            // ��������� ������� �������
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
            Debug.Log(elapsedTime);
            elapsedTime += Time.deltaTime; // ����������� �����
            yield return null; // ���� ���������� �����
        }

        // ���������, ��� ������ ����� �� ������� �������
        transform.localPosition = targetPosition;
    }
}
