using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] private Transform target;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void Update()
    {
        if (target == null || !GetComponent<BaseNPC>().IsAlive) return;

        Vector2 direction = (target.position - transform.position).normalized;
        movement = direction * moveSpeed;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = movement;
        }
    }
}