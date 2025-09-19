using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Контроллер камеры, следующей за персонажем с ограничением границ
/// Поддерживает плавное движение и настраиваемые смещения
/// </summary>
[RequireComponent(typeof(Camera))]
public class TopDownCameraFollowController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField]
    private Transform target;

    [SerializeField]
    private bool useRectTransform = true;

    [Header("Camera Offset")]
    [SerializeField]
    private Vector2 cameraOffset = Vector2.zero;

    [SerializeField]
    private bool smoothFollow = true;

    [SerializeField, Range(0.1f, 5f)]
    private float smoothTime = 0.3f;

    [Header("Movement Boundaries")]
    [SerializeField]
    public Vector2 minBoundary = new Vector2(155f, 58f);

    [SerializeField]
    public Vector2 maxBoundary = new Vector2(1735f, 980f);

    [SerializeField]
    private bool clampCameraPosition = true;

    private Camera controlledCamera;
    private RectTransform targetRectTransform;
    private Vector3 currentVelocity;
    private Vector3 targetPosition;

    private const float Z_POSITION = -10f; // Стандартная Z-позиция для 2D камеры

    private void Awake()
    {
        InitializeComponents();
        ValidateBoundaries();
    }

    private void InitializeComponents()
    {
        controlledCamera = GetComponent<Camera>();

        if (target == null)
        {
            Debug.LogError("Target is not assigned in TopDownCameraFollowController!");
            enabled = false;
            return;
        }

        if (useRectTransform)
        {
            targetRectTransform = target.GetComponent<RectTransform>();
            if (targetRectTransform == null)
            {
                Debug.LogWarning("useRectTransform is enabled but target doesn't have RectTransform! Switching to regular Transform.");
                useRectTransform = false;
            }
        }
    }

    private void ValidateBoundaries()
    {
        if (minBoundary.x >= maxBoundary.x)
        {
            Debug.LogWarning("minBoundary.x >= maxBoundary.x! Adjusting boundaries.");
            maxBoundary.x = minBoundary.x + 100f;
        }

        if (minBoundary.y >= maxBoundary.y)
        {
            Debug.LogWarning("minBoundary.y >= maxBoundary.y! Adjusting boundaries.");
            maxBoundary.y = minBoundary.y + 100f;
        }
    }

    private void Start()
    {
        InitializeCameraPosition();
    }

    private void InitializeCameraPosition()
    {
        targetPosition = CalculateTargetPosition();

        if (!smoothFollow)
        {
            transform.position = targetPosition;
        }
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        targetPosition = CalculateTargetPosition();

        if (smoothFollow)
        {
            ApplySmoothFollow();
        }
        else
        {
            ApplyImmediateFollow();
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        Vector2 targetScreenPosition = GetTargetScreenPosition();
        Vector2 clampedPosition = clampCameraPosition ?
            ClampPosition(targetScreenPosition) : targetScreenPosition;

        return new Vector3(
            clampedPosition.x + cameraOffset.x,
            clampedPosition.y + cameraOffset.y,
            Z_POSITION
        );
    }

    private Vector2 GetTargetScreenPosition()
    {
        if (useRectTransform && targetRectTransform != null)
        {
            return targetRectTransform.anchoredPosition;
        }
        else
        {
            return new Vector2(target.position.x, target.position.y);
        }
    }

    private Vector2 ClampPosition(Vector2 position)
    {
        return new Vector2(
            Mathf.Clamp(position.x, minBoundary.x, maxBoundary.x),
            Mathf.Clamp(position.y, minBoundary.y, maxBoundary.y)
        );
    }

    private void ApplySmoothFollow()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothTime
        );
    }

    private void ApplyImmediateFollow()
    {
        transform.position = targetPosition;
    }

    /// <summary>
    /// Устанавливает нового target для камеры
    /// </summary>
    public void SetTarget(Transform newTarget, bool resetImmediately = true)
    {
        target = newTarget;

        if (useRectTransform)
        {
            targetRectTransform = newTarget.GetComponent<RectTransform>();
        }

        if (resetImmediately)
        {
            InitializeCameraPosition();
        }
    }

    /// <summary>
    /// Устанавливает смещение камеры
    /// </summary>
    public void SetCameraOffset(Vector2 offset, bool applyImmediately = true)
    {
        cameraOffset = offset;

        if (applyImmediately)
        {
            targetPosition = CalculateTargetPosition();
            ApplyImmediateFollow();
        }
    }

    /// <summary>
    /// Устанавливает границы движения камеры
    /// </summary>
    public void SetBoundaries(Vector2 newMinBoundary, Vector2 newMaxBoundary)
    {
        minBoundary = newMinBoundary;
        maxBoundary = newMaxBoundary;
        ValidateBoundaries();
    }

    /// <summary>
    /// Включает/выключает ограничение границ
    /// </summary>
    public void SetClampEnabled(bool enabled)
    {
        clampCameraPosition = enabled;
    }

    /// <summary>
    /// Включает/выключает плавное следование
    /// </summary>
    public void SetSmoothFollowEnabled(bool enabled)
    {
        smoothFollow = enabled;
        currentVelocity = Vector3.zero; // Сбрасываем velocity при изменении режима
    }

    /// <summary>
    /// Возвращает true если target находится в пределах границ
    /// </summary>
    public bool IsTargetWithinBounds()
    {
        Vector2 targetPos = GetTargetScreenPosition();
        return targetPos.x >= minBoundary.x && targetPos.x <= maxBoundary.x &&
               targetPos.y >= minBoundary.y && targetPos.y <= maxBoundary.y;
    }

    /// <summary>
    /// Телепортирует камеру к текущей позиции target immediately
    /// </summary>
    public void SnapToTarget()
    {
        targetPosition = CalculateTargetPosition();
        ApplyImmediateFollow();
        currentVelocity = Vector3.zero;
    }

    // Для визуальной отладки границ в редакторе
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!clampCameraPosition) return;

        // Рисуем границы камеры
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(
            (minBoundary.x + maxBoundary.x) / 2,
            (minBoundary.y + maxBoundary.y) / 2,
            Z_POSITION
        );

        Vector3 size = new Vector3(
            maxBoundary.x - minBoundary.x,
            maxBoundary.y - minBoundary.y,
            0.1f
        );

        Gizmos.DrawWireCube(center, size);
    }

    private void OnValidate()
    {
        ValidateBoundaries();
    }
#endif
}