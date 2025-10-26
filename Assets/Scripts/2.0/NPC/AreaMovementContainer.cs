using UnityEngine;

public class AreaMovementContainer : MonoBehaviour
{
    public Vector3[] GetAreaBounds()
    {
        Vector3[] bounds = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            bounds[i] = transform.GetChild(i).position;
        }
        return bounds;
    }

    public bool IsPointInArea(Vector3 point)
    {
        // Простая проверка точки в полигоне
        Vector3[] bounds = GetAreaBounds();
        if (bounds.Length < 3) return true; // Если область не задана, разрешаем движение

        int intersections = 0;
        for (int i = 0; i < bounds.Length; i++)
        {
            Vector3 current = bounds[i];
            Vector3 next = bounds[(i + 1) % bounds.Length];

            if (((current.y > point.y) != (next.y > point.y)) &&
                (point.x < (next.x - current.x) * (point.y - current.y) / (next.y - current.y) + current.x))
            {
                intersections++;
            }
        }
        return intersections % 2 == 1;
    }

    public Vector3 GetClosestPointInArea(Vector3 targetPosition)
    {
        if (IsPointInArea(targetPosition))
            return targetPosition;

        // Находим ближайшую точку на границе области
        Vector3[] bounds = GetAreaBounds();
        Vector3 closestPoint = bounds[0];
        float closestDistance = Vector3.Distance(targetPosition, closestPoint);

        for (int i = 0; i < bounds.Length; i++)
        {
            Vector3 current = bounds[i];
            Vector3 next = bounds[(i + 1) % bounds.Length];

            Vector3 pointOnEdge = GetClosestPointOnLine(current, next, targetPosition);
            float distance = Vector3.Distance(targetPosition, pointOnEdge);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = pointOnEdge;
            }
        }

        return closestPoint;
    }

    private Vector3 GetClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDirection = (lineEnd - lineStart).normalized;
        float lineLength = Vector3.Distance(lineEnd, lineStart);
        Vector3 pointDirection = point - lineStart;

        float dot = Vector3.Dot(pointDirection, lineDirection);
        dot = Mathf.Clamp(dot, 0f, lineLength);

        return lineStart + lineDirection * dot;
    }

    // Визуализация в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3[] bounds = GetAreaBounds();

        for (int i = 0; i < bounds.Length; i++)
        {
            Gizmos.DrawWireSphere(bounds[i], 0.3f);
            if (i < bounds.Length - 1)
            {
                Gizmos.DrawLine(bounds[i], bounds[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(bounds[i], bounds[0]);
            }
        }

        // Заливка области
        if (bounds.Length >= 3)
        {
            Gizmos.color = new Color(0, 1, 0, 0.1f);
            Vector3 center = Vector3.zero;
            foreach (Vector3 point in bounds) center += point;
            center /= bounds.Length;

            for (int i = 0; i < bounds.Length; i++)
            {
                Gizmos.DrawLine(center, bounds[i]);
            }
        }
    }
}