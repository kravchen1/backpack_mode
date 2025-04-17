using UnityEngine;

public class DestroyWhenInvisible : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // Уничтожаем, если лист полностью ниже видимой области
        if (viewportPos.y < -0.1f) // Небольшой запас
        {
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        Destroy(gameObject);
    }
}