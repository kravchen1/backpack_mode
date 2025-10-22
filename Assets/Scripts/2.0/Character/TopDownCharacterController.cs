using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    #region Settings


    [Header("Appearance")]
    public int bodyIndex = 2;
    public Color bodyColor = Color.white;
    public int hairIndex = 0;
    public Color hairColor = Color.white;
    public int eyeIndex = 1;
    public Color eyeColor = Color.white;
    public SpriteRenderer head;
    public SpriteRenderer body;
    public SpriteRenderer hair;
    public SpriteRenderer eye;

    [Header("Movement")]
    public float baseMoveSpeed = 1f;

    [Header("Sprint")]
    [SerializeField] private float _sprintSpeedMultiplier = 1.5f;
    [SerializeField] private float _staminaCostPerSecond = 10f;
    [SerializeField] private float _minStaminaToSprint = 15f;
    [SerializeField] private float _regenDelayAfterSprint = 1.5f;

    [Header("Animation")]
    public Animator animator;
    public string horizontalAnimParam = "Horizontal";
    public string verticalAnimParam = "Vertical";
    public float animationSmoothTime = 0.1f;
    public float walkThreshold = 0.1f;

    [Header("Flip")]
    [SerializeField] private Transform graphicsTransform;
    [SerializeField] private bool enableFlip = true;
    [SerializeField] private FlipMode flipMode = FlipMode.ByMovement;
    [SerializeField] private float flipSmoothTime = 0.1f;
    #endregion

    #region Components & State
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastNonZeroDirection = new Vector2(0, -1);
    private Vector2 animationVelocity;
    private float currentFlipVelocity;
    private float targetScaleX = 1f;

    // Sprint system
    private bool _isSprinting = false;
    private bool _canSprint = true;
    private bool _isExhausted = false;
    private float _currentMoveSpeed;
    private float _sprintSpeed;
    private float _sprintEndTime;
    #endregion

    #region Enums
    public enum FlipMode { ByMovement, ByLastDirection, Manual }
    #endregion

    #region Unity Lifecycle
    void Start()
    {
        InitializeComponents();
        InitializeGraphics();
        InitializeSpeedSystem();
    }

    void Update()
    {
        if (BattleManager.Instance != null && !BattleManager.Instance.isBattleActive)
        {
            HandleInput();
            HandleSprintInput();
            HandleFlip();
            UpdateAnimations();
        }
        else
        {
            Stop();
            UpdateAnimations();
        }
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

    private void OnDestroy()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.Stats.OnMoveSpeedChanged -= OnMoveSpeedChanged;
            PlayerDataManager.Instance.Stats.OnStaminaChanged -= OnStaminaChanged;
        }
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) Debug.LogError("Rigidbody2D component is required!");

        if (graphicsTransform == null && transform.childCount > 0)
            graphicsTransform = transform.GetChild(0);
    }

    private void InitializeGraphics()
    {
        if (graphicsTransform != null)
            targetScaleX = graphicsTransform.localScale.x;
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
    #endregion

    #region Input Handling
    private void HandleInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }

    private void HandleSprintInput()
    {
        if (PlayerDataManager.Instance == null || _isExhausted) return;

        bool sprintKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool hasEnoughStamina = PlayerDataManager.Instance.Stats.CurrentStamina >= _minStaminaToSprint;
        bool isMoving = movement.magnitude > walkThreshold;

        if (sprintKeyPressed && isMoving && hasEnoughStamina && _canSprint)
        {
            if (!_isSprinting) StartSprint();
            SpendStaminaForSprint();
        }
        else if (_isSprinting)
        {
            StopSprint();
        }

        if (_isSprinting && PlayerDataManager.Instance.Stats.CurrentStamina <= 0)
        {
            StopSprint();
            TriggerExhaustion();
        }
    }
    #endregion

    #region Sprint & Stamina System
    private void StartSprint()
    {
        _isSprinting = true;
        PlayerDataManager.Instance.Stats.StopStaminaRegen();
    }

    private void StopSprint()
    {
        if (!_isSprinting) return;

        _isSprinting = false;
        _sprintEndTime = Time.time;
        PlayerDataManager.Instance.Stats.StartStaminaRegen();
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
            PlayerDataManager.Instance.Stats.UpdateStaminaRegen(Time.deltaTime);
    }

    private void UpdateExhaustionState()
    {
        if (PlayerDataManager.Instance == null) return;

        bool wasExhausted = _isExhausted;
        _isExhausted = PlayerDataManager.Instance.Stats.CurrentStamina <= 0;

        if (_isExhausted && !wasExhausted) TriggerExhaustion();
        else if (!_isExhausted && wasExhausted) ClearExhaustion();
    }

    private void TriggerExhaustion()
    {
        _isExhausted = true;
        if (_isSprinting) StopSprint();
        _canSprint = false;
        Invoke(nameof(ClearExhaustion), 3f);
    }

    private void ClearExhaustion()
    {
        _isExhausted = false;
        _canSprint = true;
    }

    private void OnMoveSpeedChanged(float newSpeed)
    {
        _currentMoveSpeed = newSpeed;
        _sprintSpeed = newSpeed * _sprintSpeedMultiplier;
    }

    private void OnStaminaChanged(float current, float max) => UpdateExhaustionState();
    #endregion

    #region Flip System
    private void HandleFlip()
    {
        if (!enableFlip || graphicsTransform == null) return;

        switch (flipMode)
        {
            case FlipMode.ByMovement: UpdateFlipByMovement(); break;
            case FlipMode.ByLastDirection: UpdateFlipByLastDirection(); break;
        }

        ApplySmoothFlip();
    }

    private void UpdateFlipByMovement()
    {
        if (movement.magnitude > walkThreshold && Mathf.Abs(movement.x) > 0.1f)
            targetScaleX = Mathf.Sign(movement.x);
    }

    private void UpdateFlipByLastDirection()
    {
        if (movement.magnitude > walkThreshold)
            lastNonZeroDirection = movement;

        if (Mathf.Abs(lastNonZeroDirection.x) > 0.1f)
            targetScaleX = Mathf.Sign(lastNonZeroDirection.x);
    }

    private void ApplySmoothFlip()
    {
        if (graphicsTransform == null) return;

        if (flipSmoothTime > 0)
        {
            float newScaleX = Mathf.SmoothDamp(
                graphicsTransform.localScale.x, targetScaleX,
                ref currentFlipVelocity, flipSmoothTime);
            graphicsTransform.localScale = new Vector3(newScaleX, graphicsTransform.localScale.y, graphicsTransform.localScale.z);
        }
        else
        {
            graphicsTransform.localScale = new Vector3(targetScaleX, graphicsTransform.localScale.y, graphicsTransform.localScale.z);
        }
    }
    #endregion

    #region Animation System
    private void UpdateAnimations()
    {
        if (animator == null) return;

        // Определяем направление для анимаций
        if (movement.magnitude > walkThreshold)
            lastNonZeroDirection = movement;

        Vector2 animationDirection = movement.magnitude > walkThreshold ? movement : lastNonZeroDirection;

        // Устанавливаем параметры аниматора с плавным переходом
        animator.SetFloat(horizontalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(horizontalAnimParam), animationDirection.x,
                            ref animationVelocity.x, animationSmoothTime));

        animator.SetFloat(verticalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(verticalAnimParam), animationDirection.y,
                            ref animationVelocity.y, animationSmoothTime));

        // ОПРЕДЕЛЕНИЕ НАПРАВЛЕНИЯ ДВИЖЕНИЯ:
        // Здесь можно добавить логику для определения конкретного направления

        // ДВИЖЕНИЕ ВВЕРХ (Up)
        if(animationDirection.y > 0 && Mathf.Abs(animationDirection.y) > Mathf.Abs(animationDirection.x))
        {
            //Debug.Log("Двигаемся вверх");
            head.sprite = CharacterAppearanceManager.Instance.GetHeadUp();
            body.sprite = CharacterAppearanceManager.Instance.GetBodyUp(bodyIndex);
            hair.sprite = CharacterAppearanceManager.Instance.GetHairUp(hairIndex);
            eye.gameObject.SetActive(false);
        }

        // ДВИЖЕНИЕ ВНИЗ (Down) 
        if(animationDirection.y < 0 && Mathf.Abs(animationDirection.y) > Mathf.Abs(animationDirection.x))
        {
            //Debug.Log("Двигаемся вниз");
            head.sprite = CharacterAppearanceManager.Instance.GetHeadDown();
            body.sprite = CharacterAppearanceManager.Instance.GetBodyDown(bodyIndex);
            hair.sprite = CharacterAppearanceManager.Instance.GetHairDown(hairIndex);
            eye.gameObject.SetActive(true);
            eye.sprite = CharacterAppearanceManager.Instance.GetEyeDown(eyeIndex);
        }

        // ДВИЖЕНИЕ ВПРАВО (Right) или ВЛЕВО (Left)
        if ((animationDirection.x > 0 && Mathf.Abs(animationDirection.x) > Mathf.Abs(animationDirection.y))
            || animationDirection.x < 0 && Mathf.Abs(animationDirection.x) > Mathf.Abs(animationDirection.y))
        {
            head.sprite = CharacterAppearanceManager.Instance.GetHeadSide();
            body.sprite = CharacterAppearanceManager.Instance.GetBodySide(bodyIndex);
            hair.sprite = CharacterAppearanceManager.Instance.GetHairSide(hairIndex);
            eye.gameObject.SetActive(true);
            eye.sprite = CharacterAppearanceManager.Instance.GetEyeSide(eyeIndex);
        }

        // Определяем скорость анимации в зависимости от состояния
        float speedValue = 0f;
        if (movement.magnitude > walkThreshold)
        {
            if (_isExhausted)
                speedValue = 0.5f;    // Медленная анимация при истощении
            else if (_isSprinting)
                speedValue = 2f;      // Быстрая анимация при спринте
            else
                speedValue = 1f;      // Нормальная скорость анимации
        }

        animator.speed = speedValue;
    }
    #endregion

    #region Public API - Sprint & Stamina
    public void SetSprintEnabled(bool enabled)
    {
        _canSprint = enabled;
        if (!enabled && _isSprinting) StopSprint();
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
                ClearExhaustion();
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
            graphicsTransform.localScale = new Vector3(targetScaleX, graphicsTransform.localScale.y, graphicsTransform.localScale.z);
            currentFlipVelocity = 0f;
        }
    }

    public void SetFlipEnabled(bool enabled) => enableFlip = enabled;
    public void SetFlipMode(FlipMode mode) => flipMode = mode;
    public float GetFlipDirection() => targetScaleX;

    public void FlipImmediately(bool faceRight)
    {
        if (graphicsTransform == null) return;

        targetScaleX = faceRight ? 1f : -1f;
        graphicsTransform.localScale = new Vector3(targetScaleX, graphicsTransform.localScale.y, graphicsTransform.localScale.z);
        currentFlipVelocity = 0f;
    }
    #endregion

    #region Public API - Movement
    public void Move(Vector2 direction) => movement = direction.normalized;
    public void Stop()
    {
        movement = Vector2.zero;
        if (rb != null) rb.velocity = Vector2.zero;
    }
    public Vector2 GetMovementDirection() => movement;
    public Vector2 GetFacingDirection() => lastNonZeroDirection;
    #endregion
}