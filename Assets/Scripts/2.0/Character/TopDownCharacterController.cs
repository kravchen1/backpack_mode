using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseMoveSpeed = 15f;

    [Header("Sprint Settings")]
    [SerializeField] private float _sprintSpeedMultiplier = 1.5f;
    [SerializeField] private float _staminaCostPerSecond = 10f;
    [SerializeField] private float _minStaminaToSprint = 15f;

    [Header("Stamina Regen Settings")]
    [SerializeField] private float _regenDelayAfterSprint = 1.5f;

    [Header("Animation References")]
    public Animator animator;
    public string horizontalAnimParam = "Horizontal";
    public string verticalAnimParam = "Vertical";
    public string speedAnimParam = "Speed";
    public string isSprintingAnimParam = "IsSprinting";
    public string isExhaustedAnimParam = "IsExhausted";

    [Header("Animation Settings")]
    public float animationSmoothTime = 0.1f;
    public float walkThreshold = 0.1f;

    [Header("Flip Settings")]
    [SerializeField] private Transform graphicsTransform;
    [SerializeField] private bool enableFlip = true;
    [SerializeField] private FlipMode flipMode = FlipMode.ByMovement;
    [SerializeField] private float flipSmoothTime = 0.1f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastNonZeroDirection = new Vector2(0, -1);
    private Vector2 animationVelocity;
    private float currentFlipVelocity;
    private float targetScaleX = 1f;

    // Система спринта и стамины
    private bool _isSprinting = false;
    private bool _canSprint = true;
    private bool _isExhausted = false;
    private float _currentMoveSpeed;
    private float _sprintSpeed;
    private float _sprintEndTime;

    public enum FlipMode
    {
        ByMovement,
        ByLastDirection,
        Manual
    }

    void Start()
    {
        InitializeComponents();
        InitializeGraphics();
        InitializeSpeedSystem();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (rb == null)
            Debug.LogError("Rigidbody2D component is required!");

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

    private void InitializeSpeedSystem()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.Stats.OnMoveSpeedChanged += OnMoveSpeedChanged;
            PlayerDataManager.Instance.Stats.OnStaminaChanged += OnStaminaChanged;

            _currentMoveSpeed = PlayerDataManager.Instance.Stats.CurrentMoveSpeed;
            _sprintSpeed = _currentMoveSpeed * _sprintSpeedMultiplier;

            UpdateExhaustionState();
        }
        else
        {
            _currentMoveSpeed = baseMoveSpeed;
            _sprintSpeed = baseMoveSpeed * _sprintSpeedMultiplier;
            Debug.LogWarning("PlayerDataManager not found. Using base move speed.");
        }
    }

    private void OnMoveSpeedChanged(float newSpeed)
    {
        _currentMoveSpeed = newSpeed;
        _sprintSpeed = newSpeed * _sprintSpeedMultiplier;
    }

    private void OnStaminaChanged(float current, float max)
    {
        UpdateExhaustionState();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        HandleSprintInput();
        HandleFlip();
        UpdateAnimations();
        UpdateStaminaRegen();
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            float currentSpeed = _isSprinting ? _sprintSpeed : _currentMoveSpeed;
            rb.velocity = movement * currentSpeed;
        }
    }

    private void HandleSprintInput()
    {
        if (PlayerDataManager.Instance == null) return;

        bool sprintKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool hasEnoughStamina = PlayerDataManager.Instance.Stats.CurrentStamina >= _minStaminaToSprint;
        bool isMoving = movement.magnitude > walkThreshold;

        if (_isExhausted)
        {
            if (_isSprinting)
            {
                StopSprint();
            }
            return;
        }

        if (sprintKeyPressed && isMoving && hasEnoughStamina && _canSprint)
        {
            if (!_isSprinting)
            {
                StartSprint();
            }
            SpendStaminaForSprint();
        }
        else
        {
            if (_isSprinting)
            {
                StopSprint();
            }
        }

        if (_isSprinting && PlayerDataManager.Instance.Stats.CurrentStamina <= 0)
        {
            StopSprint();
            TriggerExhaustion();
        }
    }

    private void StartSprint()
    {
        _isSprinting = true;

        PlayerDataManager.Instance.Stats.StopStaminaRegen();

        //if (animator != null && !string.IsNullOrEmpty(isSprintingAnimParam))
        //{
        //    animator.SetBool(isSprintingAnimParam, true);
        //}
    }

    private void StopSprint()
    {
        if (!_isSprinting) return;

        _isSprinting = false;
        _sprintEndTime = Time.time;

        PlayerDataManager.Instance.Stats.StartStaminaRegen();

        //if (animator != null && !string.IsNullOrEmpty(isSprintingAnimParam))
        //{
        //    animator.SetBool(isSprintingAnimParam, false);
        //}
    }

    private void SpendStaminaForSprint()
    {
        if (PlayerDataManager.Instance == null) return;

        float staminaCost = _staminaCostPerSecond * Time.deltaTime;
        PlayerDataManager.Instance.Stats.CurrentStamina -= staminaCost;
    }

    private void UpdateStaminaRegen()
    {
        if (PlayerDataManager.Instance == null) return;

        if (!_isSprinting && Time.time - _sprintEndTime > _regenDelayAfterSprint)
        {
            PlayerDataManager.Instance.Stats.UpdateStaminaRegen(Time.deltaTime);
        }
    }

    private void UpdateExhaustionState()
    {
        if (PlayerDataManager.Instance == null) return;

        bool wasExhausted = _isExhausted;
        _isExhausted = PlayerDataManager.Instance.Stats.CurrentStamina <= 0;

        if (_isExhausted && !wasExhausted)
        {
            TriggerExhaustion();
        }
        else if (!_isExhausted && wasExhausted)
        {
            ClearExhaustion();
        }

        if (animator != null && !string.IsNullOrEmpty(isExhaustedAnimParam))
        {
            animator.SetBool(isExhaustedAnimParam, _isExhausted);
        }
    }

    private void TriggerExhaustion()
    {
        _isExhausted = true;

        if (_isSprinting)
        {
            StopSprint();
        }

        _canSprint = false;

        Debug.Log("Exhaustion triggered! Sprint disabled temporarily.");

        Invoke(nameof(ClearExhaustion), 3f);
    }

    private void ClearExhaustion()
    {
        _isExhausted = false;
        _canSprint = true;

        if (animator != null && !string.IsNullOrEmpty(isExhaustedAnimParam))
        {
            animator.SetBool(isExhaustedAnimParam, false);
        }

        Debug.Log("Exhaustion cleared. Sprint enabled.");
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
        }

        ApplySmoothFlip();
    }

    private void UpdateFlipByMovement()
    {
        if (movement.magnitude > walkThreshold && Mathf.Abs(movement.x) > 0.1f)
        {
            targetScaleX = Mathf.Sign(movement.x);
        }
    }

    private void UpdateFlipByLastDirection()
    {
        if (movement.magnitude > walkThreshold)
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

    private void UpdateAnimations()
    {
        if (animator == null) return;

        if (movement.magnitude > walkThreshold)
        {
            lastNonZeroDirection = movement;
        }

        Vector2 animationDirection = movement.magnitude > walkThreshold ? movement : lastNonZeroDirection;

        animator.SetFloat(horizontalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(horizontalAnimParam), animationDirection.x,
                            ref animationVelocity.x, animationSmoothTime));

        animator.SetFloat(verticalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(verticalAnimParam), animationDirection.y,
                            ref animationVelocity.y, animationSmoothTime));

        float speedValue = 0f;
        if (movement.magnitude > walkThreshold)
        {
            if (_isExhausted)
                speedValue = 0.5f;
            else if (_isSprinting)
                speedValue = 2f;
            else
                speedValue = 1f;
        }

        animator.SetFloat(speedAnimParam, speedValue);
    }

    #region Public API - Sprint & Stamina Control

    public void SetSprintEnabled(bool enabled)
    {
        _canSprint = enabled;
        if (!enabled && _isSprinting)
        {
            StopSprint();
        }
    }

    public bool IsSprinting() => _isSprinting;
    public bool IsExhausted() => _isExhausted;
    public float GetCurrentSpeed() => _isSprinting ? _sprintSpeed : _currentMoveSpeed;

    public void ForceStaminaRegen(int amount)
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.Stats.CurrentStamina =
                Mathf.Min(PlayerDataManager.Instance.Stats.MaxStamina,
                         PlayerDataManager.Instance.Stats.CurrentStamina + amount);

            if (PlayerDataManager.Instance.Stats.CurrentStamina > 0 && _isExhausted)
            {
                ClearExhaustion();
            }
        }
    }

    #endregion

    #region Public API - Flip Control

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

    public float GetFlipDirection()
    {
        return targetScaleX;
    }

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

    private void OnDestroy()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.Stats.OnMoveSpeedChanged -= OnMoveSpeedChanged;
            PlayerDataManager.Instance.Stats.OnStaminaChanged -= OnStaminaChanged;
        }
    }

    // Для отладки
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Movement: {movement}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Speed: {GetCurrentSpeed():F1}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Sprinting: {_isSprinting}");
        GUI.Label(new Rect(10, 70, 300, 20), $"Exhausted: {_isExhausted}");
        if (PlayerDataManager.Instance != null)
        {
            GUI.Label(new Rect(10, 90, 300, 20), $"Stamina: {PlayerDataManager.Instance.Stats.CurrentStamina}");
        }
    }

    #region Editor Methods
#if UNITY_EDITOR
    [ContextMenu("Test Sprint")]
    private void TestSprint()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.Stats.CurrentStamina = 100;
            _canSprint = true;
            _isExhausted = false;
            Debug.Log("Sprint test initialized");
        }
    }

    [ContextMenu("Test Exhaustion")]
    private void TestExhaustion()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.Stats.CurrentStamina = 0;
            Debug.Log("Exhaustion test triggered");
        }
    }
#endif
    #endregion
}