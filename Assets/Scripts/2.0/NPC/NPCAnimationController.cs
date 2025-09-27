// NPCAnimationController.cs
using UnityEngine;
using UnityEngine.AI;

public class NPCAnimationController : MonoBehaviour
{
    [Header("Animation References")]
    public Animator animator;
    public string horizontalAnimParam = "Horizontal";
    public string verticalAnimParam = "Vertical";
    public string speedAnimParam = "Speed";
    public string isMovingAnimParam = "IsMoving";

    [Header("Animation Settings")]
    public float animationSmoothTime = 0.1f;
    public float walkThreshold = 0.1f;

    [Header("Flip Settings")]
    [SerializeField] private Transform graphicsTransform;
    [SerializeField] private bool enableFlip = true;
    [SerializeField] private FlipMode flipMode = FlipMode.ByMovement;
    [SerializeField] private float flipSmoothTime = 0.1f;

    [Header("Forced Look Settings")]
    [SerializeField] private float forcedLookDuration = 3f;

    private bool isForcedLooking = false;
    private float forcedLookTimer = 0f;
    private Vector2 forcedLookDirection;

    private NavMeshAgent navMeshAgent;
    private Vector2 lastNonZeroDirection = new Vector2(0, -1);
    private Vector2 animationVelocity;
    private float currentFlipVelocity;
    private float targetScaleX = 1f;

    public enum FlipMode
    {
        ByMovement,
        ByLastDirection,
        Manual
    }

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (graphicsTransform == null && transform.childCount > 0)
        {
            graphicsTransform = transform.GetChild(0);
        }
    }

    void Update()
    {
        UpdateForcedLook();
        UpdateAnimations();
        HandleFlip();
    }

    private void UpdateForcedLook()
    {
        if (isForcedLooking)
        {
            forcedLookTimer -= Time.deltaTime;
            if (forcedLookTimer <= 0f)
            {
                isForcedLooking = false;
            }
        }
    }

    private void UpdateAnimations()
    {
        if (animator == null || navMeshAgent == null) return;

        // ѕолучаем направление движени€ из velocity NavMeshAgent
        Vector2 movement = new Vector2(navMeshAgent.velocity.x, navMeshAgent.velocity.y).normalized;
        float currentSpeed = navMeshAgent.velocity.magnitude;

        // ≈сли принудительный поворот активен - используем его направление
        Vector2 animationDirection = isForcedLooking ? forcedLookDirection :
                                   (currentSpeed > walkThreshold ? movement : lastNonZeroDirection);

        // ќбновл€ем lastNonZeroDirection если движемс€
        if (currentSpeed > walkThreshold)
        {
            lastNonZeroDirection = movement;
        }


        // ѕлавно интерполируем параметры анимации
        animator.SetFloat(horizontalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(horizontalAnimParam), animationDirection.x,
                            ref animationVelocity.x, animationSmoothTime));

        animator.SetFloat(verticalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(verticalAnimParam), animationDirection.y,
                            ref animationVelocity.y, animationSmoothTime));

        // ¬ычисл€ем значение скорости дл€ анимации
        float speedValue = 0f;
        if (currentSpeed > walkThreshold)
        {
            // Ќормализуем скорость относительно максимальной скорости агента
            speedValue = Mathf.Clamp01(currentSpeed / navMeshAgent.speed);

            // ћожно добавить множители дл€ разных состо€ний (бег, усталость и т.д.)
            if (TryGetComponent<NPCController>(out var npcController))
            {
                if (npcController.IsHostile())
                {
                    speedValue *= 1.2f; // ”скоренна€ анимаци€ при преследовании
                }
            }
        }

        animator.SetFloat(speedAnimParam, speedValue);

        // ƒополнительный параметр дл€ простой проверки движени€
        if (!string.IsNullOrEmpty(isMovingAnimParam))
        {
            animator.SetBool(isMovingAnimParam, currentSpeed > walkThreshold);
        }
    }

    private void HandleFlip()
    {
        if (!enableFlip || graphicsTransform == null) return;

        Vector2 movement = new Vector2(navMeshAgent.velocity.x, navMeshAgent.velocity.y);
        float currentSpeed = navMeshAgent.velocity.magnitude;

        // ѕриоритет у принудительного поворота
        if (isForcedLooking)
        {
            targetScaleX = Mathf.Sign(forcedLookDirection.x);
        }
        else
        {
            switch (flipMode)
            {
                case FlipMode.ByMovement:
                    UpdateFlipByMovement(movement, currentSpeed);
                    break;
                case FlipMode.ByLastDirection:
                    UpdateFlipByLastDirection(movement, currentSpeed);
                    break;
            }
        }

        ApplySmoothFlip();
    }

    private void UpdateFlipByMovement(Vector2 movement, float currentSpeed)
    {
        if (currentSpeed > walkThreshold && Mathf.Abs(movement.x) > 0.1f)
        {
            targetScaleX = Mathf.Sign(movement.x);
        }
    }

    private void UpdateFlipByLastDirection(Vector2 movement, float currentSpeed)
    {
        if (currentSpeed > walkThreshold)
        {
            lastNonZeroDirection = movement;
        }

        if (Mathf.Abs(lastNonZeroDirection.x) > 0.1f)
        {
            targetScaleX = Mathf.Sign(lastNonZeroDirection.x);
        }
    }

    private void ApplySmoothFlip()
    {
        if (graphicsTransform == null) return;

        if (flipSmoothTime > 0)
        {
            float newScaleX = Mathf.SmoothDamp(
                graphicsTransform.localScale.x,
                targetScaleX,
                ref currentFlipVelocity,
                flipSmoothTime
            );

            graphicsTransform.localScale = new Vector3(newScaleX, graphicsTransform.localScale.y, graphicsTransform.localScale.z);
        }
        else
        {
            graphicsTransform.localScale = new Vector3(targetScaleX, graphicsTransform.localScale.y, graphicsTransform.localScale.z);
        }
    }

    #region Public API

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

    public void SetFlipEnabled(bool enabled)
    {
        enableFlip = enabled;
    }

    public void SetFlipMode(FlipMode mode)
    {
        flipMode = mode;
    }

    public void ForceLookAt(Vector2 direction, float duration = -1f)
    {
        isForcedLooking = true;
        forcedLookDirection = direction.normalized;
        forcedLookTimer = duration > 0 ? duration : forcedLookDuration;

        // —разу примен€ем поворот
        if (Mathf.Abs(forcedLookDirection.x) > 0.1f)
        {
            targetScaleX = Mathf.Sign(forcedLookDirection.x);
            if (graphicsTransform != null)
            {
                graphicsTransform.localScale = new Vector3(targetScaleX, graphicsTransform.localScale.y, graphicsTransform.localScale.z);
            }
        }
    }

    public void CancelForcedLook()
    {
        isForcedLooking = false;
        forcedLookTimer = 0f;
    }

    #endregion
}