// LeafInteraction.cs
using UnityEngine;

public class LeafInteraction : MonoBehaviour
{
    public float force = 5f;
    public float radius = 2f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] leaves = Physics2D.OverlapCircleAll(mousePos, radius);

            foreach (Collider2D leaf in leaves)
            {
                Rigidbody2D rb = leaf.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (leaf.transform.position - (Vector3)mousePos).normalized;
                    rb.AddForce(direction * force, ForceMode2D.Impulse);

                    // Добавляем небольшой вращательный момент
                    rb.AddTorque(Random.Range(-1f, 1f) * force);
                }
            }
        }
    }

    // Для визуализации радиуса взаимодействия в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}