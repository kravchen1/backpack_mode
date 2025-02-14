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





    //// �������� ��� �������� �����������
    //private IEnumerator MoveObject(Vector3 startPosition, Vector3 targetPosition, float duration)
    //{
    //    float elapsedTime = 0; // �����, ��������� � ������ ��������
    //    while (elapsedTime < duration)
    //    {
    //        // ��������� ������� �������
    //        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
    //        elapsedTime += Time.deltaTime; // ����������� �����
    //        yield return null; // ���� ���������� �����
    //    }

    //    // ���������, ��� ������ ����� �� ������� �������
    //    transform.localPosition = targetPosition;
    //}
}
