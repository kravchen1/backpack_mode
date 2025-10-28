using UnityEngine;

public class RobBlock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Добавить проверку тега, если нужно
        if (RobManager.Instance != null)
        {
            RobManager.Instance.Catch();
        }
    }
}