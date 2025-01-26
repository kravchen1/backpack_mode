using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private LineRenderer lineRenderer; // Массив LineRenderer для каждой линии

    public void DrawLineBetweenDoors(Transform pointA, Transform pointB, Color color) // Второй объект)
    {
       // Создаем новый объект для линии
        GameObject lineObject = new GameObject("Line");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Настраиваем LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 1f; // Ширина линии
        lineRenderer.endWidth = 1f; // Ширина линии
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Устанавливаем материал
        lineRenderer.sortingLayerName = "UI";
        //lineRenderer.sortingOrder = 10;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;  


        // Устанавливаем позиции
        lineRenderer.SetPosition(0, pointA.position);
        lineRenderer.SetPosition(1, pointB.position);
    }
}
