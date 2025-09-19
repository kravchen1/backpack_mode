using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseMoveSpeed = 15f;

    [Header("Animation References")]
    public Animator animator;
    public string horizontalAnimParam = "Horizontal";
    public string verticalAnimParam = "Vertical";
    public string speedAnimParam = "Speed";

    [Header("Animation Settings")]
    public float animationSmoothTime = 0.1f;
    public float walkThreshold = 0.1f;

    [Header("Flip Settings")]
    [SerializeField] private Transform graphicsTransform; // Ссылка на дочерний объект с графикой
    [SerializeField] private bool enableFlip = true;
    [SerializeField] private FlipMode flipMode = FlipMode.ByMovement;
    [SerializeField] private float flipSmoothTime = 0.1f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastNonZeroDirection = new Vector2(0, -1);
    private Vector2 animationVelocity;
    private float currentFlipVelocity;
    private float targetScaleX = 1f;
    [HideInInspector] public float moveSpeed = 15f;//change in DynamicCameraSize

    public enum FlipMode
    {
        ByMovement,     // Поворот по движению
        ByLastDirection, // Поворот по последнему направлению
        Manual          // Ручное управление
    }

    void Start()
    {
        moveSpeed = baseMoveSpeed;
        InitializeComponents();
        InitializeGraphics();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (rb == null)
            Debug.LogError("Rigidbody2D component is required!");

        // Автопоиск graphics transform если не назначен
        if (graphicsTransform == null && transform.childCount > 0)
        {
            graphicsTransform = transform.GetChild(0);
            Debug.Log($"Auto-assigned graphics transform: {graphicsTransform.name}");
        }
    }

    private void InitializeGraphics()
    {
        if (graphicsTransform != null)
        {
            targetScaleX = graphicsTransform.localScale.x;
        }
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        HandleFlip();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = movement * moveSpeed;
        }
    }

    private void HandleFlip()
    {
        if (!enableFlip || graphicsTransform == null) return;

        switch (flipMode)
        {
            case FlipMode.ByMovement:
                UpdateFlipByMovement();
                break;

            case FlipMode.ByLastDirection:
                UpdateFlipByLastDirection();
                break;

            case FlipMode.Manual:
                // Ручное управление - ничего не делаем
                break;
        }

        ApplySmoothFlip();
    }

    private void UpdateFlipByMovement()
    {
        if (movement.magnitude > walkThreshold)
        {
            // Поворачиваем только при значительном движении по X
            if (Mathf.Abs(movement.x) > 0.1f)
            {
                targetScaleX = Mathf.Sign(movement.x);
            }
        }
    }

    private void UpdateFlipByLastDirection()
    {
        if (movement.magnitude > walkThreshold)
        {
            lastNonZeroDirection = movement;
        }

        // Поворачиваем based on последнего направления
        if (Mathf.Abs(lastNonZeroDirection.x) > 0.1f)
        {
            targetScaleX = Mathf.Sign(lastNonZeroDirection.x);
        }
    }

    private void ApplySmoothFlip()
    {
        if (flipSmoothTime > 0)
        {
            float currentScaleX = graphicsTransform.localScale.x;
            float newScaleX = Mathf.SmoothDamp(
                currentScaleX,
                targetScaleX,
                ref currentFlipVelocity,
                flipSmoothTime
            );

            graphicsTransform.localScale = new Vector3(
                newScaleX,
                graphicsTransform.localScale.y,
                graphicsTransform.localScale.z
            );
        }
        else
        {
            // Мгновенный поворот
            graphicsTransform.localScale = new Vector3(
                targetScaleX,
                graphicsTransform.localScale.y,
                graphicsTransform.localScale.z
            );
        }
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        // Сохраняем последнее ненулевое направление движения
        if (movement.magnitude > walkThreshold)
        {
            lastNonZeroDirection = movement;
        }

        // Для анимации используем последнее ненулевое направление
        Vector2 animationDirection = movement.magnitude > walkThreshold ? movement : lastNonZeroDirection;

        // Плавное изменение параметров направления
        animator.SetFloat(horizontalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(horizontalAnimParam), animationDirection.x,
                            ref animationVelocity.x, animationSmoothTime));

        animator.SetFloat(verticalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(verticalAnimParam), animationDirection.y,
                            ref animationVelocity.y, animationSmoothTime));

        // Управляем переходом между Blend Tree
        float speedValue = movement.magnitude > walkThreshold ? 1f : 0f;
        animator.SetFloat(speedAnimParam, speedValue);
    }

    #region Public API - Flip Control

    /// <summary>
    /// Устанавливает направление поворота вручную
    /// </summary>
    public void SetFlipDirection(float direction, bool immediate = false)
    {
        if (!enableFlip) return;

        targetScaleX = Mathf.Sign(direction);

        if (immediate && graphicsTransform != null)
        {
            graphicsTransform.localScale = new Vector3(
                targetScaleX,
                graphicsTransform.localScale.y,
                graphicsTransform.localScale.z
            );
            currentFlipVelocity = 0f;
        }
    }

    /// <summary>
    /// Включает/выключает систему поворотов
    /// </summary>
    public void SetFlipEnabled(bool enabled)
    {
        enableFlip = enabled;
    }

    /// <summary>
    /// Устанавливает режим поворота
    /// </summary>
    public void SetFlipMode(FlipMode mode)
    {
        flipMode = mode;
    }

    /// <summary>
    /// Возвращает текущее направление поворота (1 = right, -1 = left)
    /// </summary>
    public float GetFlipDirection()
    {
        return targetScaleX;
    }

    /// <summary>
    /// Немедленно поворачивает графику в указанном направлении
    /// </summary>
    public void FlipImmediately(bool faceRight)
    {
        if (graphicsTransform == null) return;

        targetScaleX = faceRight ? 1f : -1f;
        graphicsTransform.localScale = new Vector3(
            targetScaleX,
            graphicsTransform.localScale.y,
            graphicsTransform.localScale.z
        );
        currentFlipVelocity = 0f;
    }

    #endregion

    #region Public API - Movement

    public void Move(Vector2 direction)
    {
        movement = direction.normalized;
    }

    public void Stop()
    {
        movement = Vector2.zero;
        if (rb != null) rb.velocity = Vector2.zero;
    }

    public Vector2 GetMovementDirection() => movement;
    public Vector2 GetFacingDirection() => lastNonZeroDirection;

    #endregion

    // Для отладки
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Movement: {movement}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Facing: {lastNonZeroDirection}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Flip Direction: {targetScaleX}");
    }
}