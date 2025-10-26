using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    #region Settings
    [Header("Appearance")]
    public int bodyIndex = 2;
    public Color bodyColor = Color.white;
    public Color headColor = Color.white;
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
    [SerializeField] private BaseFlipDirection baseFlipDirection = BaseFlipDirection.Right;

    public enum FlipMode { ByMovement, ByLastDirection, Manual }
    public enum BaseFlipDirection { Right, Left } // Направление по умолчанию
    #endregion

    #region Components & State
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastNonZeroDirection = new Vector2(0, 1);
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

    // Кэш для системы анимаций
    private MovementDirection _currentMovementDirection = MovementDirection.Down;
    private MovementDirection _lastMovementDirection = MovementDirection.Down;
    private bool _wasMoving = false;
    private bool _isMoving = false;
    #endregion

    #region Enums
    public enum MovementDirection { Down, Up, Left, Right }
    #endregion


    #region Unity Lifecycle
    void Start()
    {
        InitializeComponents();
        InitializeAppearance();
        InitializeGraphics();
        InitializeSpeedSystem();
        LoadPlayerPosition(); // ← Загружаем позицию при старте
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

    void OnApplicationQuit()
    {
        SavePlayerPosition(); // ← Сохраняем при выходе
    }

    void OnDestroy()
    {
        SavePlayerPosition(); // ← Сохраняем при уничтожении объекта

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
        if (graphicsTransform == null && transform.childCount > 0)
            graphicsTransform = transform.GetChild(0);

        if (graphicsTransform != null)
            targetScaleX = graphicsTransform.localScale.x;

        // Инициализируем спрайты
        InitializeAppearance();
        RefreshAppearance();
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

    private void InitializeAppearance()
    {
        // Индексы частей тела
        bodyIndex = PlayerPrefs.GetInt("PlayerBodyIndex", 2);
        hairIndex = PlayerPrefs.GetInt("PlayerHairIndex", 1);
        eyeIndex = PlayerPrefs.GetInt("PlayerEyeIndex", 1);

        // Цвета - сохраняем как отдельные компоненты RGB
        bodyColor = LoadColor("PlayerBodyColor", Color.white);
        headColor = LoadColor("PlayerHeadColor", Color.white);
        hairColor = LoadColor("PlayerHairColor", Color.white);
        eyeColor = LoadColor("PlayerEyeColor", Color.white);
    }
    #endregion

    #region Appearance
    private Color LoadColor(string key, Color defaultColor)
    {
        // Если ключ не существует, возвращаем цвет по умолчанию
        if (!PlayerPrefs.HasKey(key + "_r"))
            return defaultColor;

        float r = PlayerPrefs.GetFloat(key + "_r", defaultColor.r);
        float g = PlayerPrefs.GetFloat(key + "_g", defaultColor.g);
        float b = PlayerPrefs.GetFloat(key + "_b", defaultColor.b);
        float a = PlayerPrefs.GetFloat(key + "_a", defaultColor.a);

        return new Color(r, g, b, a);
    }

    public void SaveAppearance()
    {
        // Сохраняем индексы
        PlayerPrefs.SetInt("PlayerBodyIndex", bodyIndex);
        PlayerPrefs.SetInt("PlayerHairIndex", hairIndex);
        PlayerPrefs.SetInt("PlayerEyeIndex", eyeIndex);

        // Сохраняем цвета
        SaveColor("PlayerBodyColor", bodyColor);
        SaveColor("PlayerHairColor", hairColor);
        SaveColor("PlayerEyeColor", eyeColor);

        PlayerPrefs.Save();
    }

    private void SaveColor(string key, Color color)
    {
        PlayerPrefs.SetFloat(key + "_r", color.r);
        PlayerPrefs.SetFloat(key + "_g", color.g);
        PlayerPrefs.SetFloat(key + "_b", color.b);
        PlayerPrefs.SetFloat(key + "_a", color.a);
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
            case FlipMode.Manual: // Оставляем текущее targetScaleX без изменений
                break;
        }

        ApplySmoothFlip();
    }

    private void UpdateFlipByMovement()
    {
        if (movement.magnitude > walkThreshold && Mathf.Abs(movement.x) > 0.1f)
        {
            float direction = Mathf.Sign(movement.x);
            targetScaleX = GetBaseDirection() * direction;
        }
    }

    private void UpdateFlipByLastDirection()
    {
        if (movement.magnitude > walkThreshold)
            lastNonZeroDirection = movement;

        if (Mathf.Abs(lastNonZeroDirection.x) > 0.1f)
        {
            float direction = Mathf.Sign(lastNonZeroDirection.x);
            targetScaleX = GetBaseDirection() * direction;
        }
    }

    // Новый метод для определения базового направления
    private float GetBaseDirection()
    {
        return baseFlipDirection == BaseFlipDirection.Right ? 1f : -1f;
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

        // Определяем текущее состояние движения
        _isMoving = movement.magnitude > walkThreshold;

        // Определяем направление для анимаций
        if (_isMoving)
        {
            lastNonZeroDirection = movement;
            _currentMovementDirection = GetCurrentMovementDirection(movement);
        }
        else
        {
            _currentMovementDirection = GetCurrentMovementDirection(lastNonZeroDirection);
        }

        // Проверяем, изменилось ли состояние и нужно ли обновлять спрайты
        bool stateChanged = _currentMovementDirection != _lastMovementDirection ||
                           _isMoving != _wasMoving;

        // Обновляем спрайты только если состояние изменилось
        if (stateChanged)
        {
            UpdateAppearanceSprites(_currentMovementDirection);
            _lastMovementDirection = _currentMovementDirection;
            _wasMoving = _isMoving;
        }

        // Устанавливаем параметры аниматора с плавным переходом
        Vector2 animationDirection = _isMoving ? movement : lastNonZeroDirection;

        animator.SetFloat(horizontalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(horizontalAnimParam), animationDirection.x,
                            ref animationVelocity.x, animationSmoothTime));

        animator.SetFloat(verticalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(verticalAnimParam), animationDirection.y,
                            ref animationVelocity.y, animationSmoothTime));

        // Определяем скорость анимации в зависимости от состояния
        float speedValue = 0f;
        if (_isMoving)
        {
            if (_isExhausted)
                speedValue = 0.5f;    // Медленная анимация при истощении
            else if (_isSprinting)
                speedValue = 2f;      // Быстрая анимация при спринте
            else
                speedValue = 1f;      // Нормальная скорость анимации
        }
    }

    private MovementDirection GetCurrentMovementDirection(Vector2 direction)
    {
        // Приоритет вертикальным направлениям над горизонтальными
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            return direction.y > 0 ? MovementDirection.Up : MovementDirection.Down;
        }
        else
        {
            return direction.x > 0 ? MovementDirection.Right : MovementDirection.Left;
        }
    }

    private void UpdateAppearanceSprites(MovementDirection movementDir)
    {
        if (CharacterAppearanceManager.Instance == null) return;

        switch (movementDir)
        {
            case MovementDirection.Up:
                head.sprite = CharacterAppearanceManager.Instance.GetHeadUp();
                body.sprite = CharacterAppearanceManager.Instance.GetBodyUp(bodyIndex);
                hair.sprite = CharacterAppearanceManager.Instance.GetHairUp(hairIndex);
                eye.gameObject.SetActive(false);
                break;

            case MovementDirection.Down:
                head.sprite = CharacterAppearanceManager.Instance.GetHeadDown();
                body.sprite = CharacterAppearanceManager.Instance.GetBodyDown(bodyIndex);
                hair.sprite = CharacterAppearanceManager.Instance.GetHairDown(hairIndex);
                eye.gameObject.SetActive(true);
                eye.sprite = CharacterAppearanceManager.Instance.GetEyeDown(eyeIndex);
                break;

            case MovementDirection.Right:
            case MovementDirection.Left:
                head.sprite = CharacterAppearanceManager.Instance.GetHeadSide();
                body.sprite = CharacterAppearanceManager.Instance.GetBodySide(bodyIndex);
                hair.sprite = CharacterAppearanceManager.Instance.GetHairSide(hairIndex);
                eye.gameObject.SetActive(true);
                eye.sprite = CharacterAppearanceManager.Instance.GetEyeSide(eyeIndex);
                break;
        }

        // Для боковых направлений также устанавливаем поворот через scale
        if (movementDir == MovementDirection.Left || movementDir == MovementDirection.Right)
        {
            float scaleDirection = movementDir == MovementDirection.Right ? 1f : -1f;
            SetFlipDirection(scaleDirection, false);
        }
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

        // Учитываем базовое направление
        targetScaleX = GetBaseDirection() * Mathf.Sign(direction);

        if (immediate && graphicsTransform != null)
        {
            graphicsTransform.localScale = new Vector3(targetScaleX, graphicsTransform.localScale.y, graphicsTransform.localScale.z);
            currentFlipVelocity = 0f;
        }
    }

    public void SetFlipEnabled(bool enabled) => enableFlip = enabled;
    public void SetFlipMode(FlipMode mode) => flipMode = mode;
    public void SetBaseFlipDirection(BaseFlipDirection direction) => baseFlipDirection = direction;

    public float GetFlipDirection() => targetScaleX;
    public bool IsFacingRight() => graphicsTransform != null ? graphicsTransform.localScale.x > 0 : true;

    public void FlipImmediately(bool faceRight)
    {
        if (graphicsTransform == null) return;

        // Учитываем базовое направление
        bool shouldFaceRight = baseFlipDirection == BaseFlipDirection.Right ? faceRight : !faceRight;
        targetScaleX = shouldFaceRight ? 1f : -1f;

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

    #region Public API - Appearance
    public void RefreshAppearance()
    {
        // Принудительно обновляем все спрайты
        _lastMovementDirection = MovementDirection.Down; // Сброс кэша
        UpdateAppearanceSprites(_currentMovementDirection);

        // Обновляем цвета
        if (head != null) head.color = headColor;
        if (body != null) body.color = bodyColor;
        if (hair != null) hair.color = hairColor;
        if (eye != null) eye.color = eyeColor;
    }

    public void UpdateBodyPart(int newBodyIndex, int newHairIndex, int newEyeIndex)
    {
        bodyIndex = newBodyIndex;
        hairIndex = newHairIndex;
        eyeIndex = newEyeIndex;
        RefreshAppearance();
    }

    public void UpdateColors(Color newBodyColor, Color newHairColor, Color newEyeColor)
    {
        bodyColor = newBodyColor;
        hairColor = newHairColor;
        eyeColor = newEyeColor;
        RefreshAppearance();
    }

    public MovementDirection GetCurrentMovementDirection() => _currentMovementDirection;
    #endregion

    #region Save/Load Position
    private void SavePlayerPosition()
    {
        PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", transform.position.z);
        PlayerPrefs.SetString("PlayerScene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerPosX");
        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerPosY");
        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerPosZ");
        PlayerPrefsMigrationManager.Instance.RegisterStringPref("PlayerScene");
        PlayerPrefs.Save();

        Debug.Log($"Position saved: {transform.position}");
    }

    private void LoadPlayerPosition()
    {
        // Проверяем, есть ли сохраненная позиция
        if (!PlayerPrefs.HasKey("PlayerPosX"))
        {
            Debug.Log("No saved position found. Using default position.");
            return;
        }

        // Проверяем, та же ли сцена
        string savedScene = PlayerPrefs.GetString("PlayerScene", "");
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (savedScene != currentScene)
        {
            Debug.Log($"Saved scene ({savedScene}) differs from current ({currentScene}). Position not loaded.");
            return;
        }

        // Загружаем позицию
        float posX = PlayerPrefs.GetFloat("PlayerPosX", transform.position.x);
        float posY = PlayerPrefs.GetFloat("PlayerPosY", transform.position.y);
        float posZ = PlayerPrefs.GetFloat("PlayerPosZ", transform.position.z);

        Vector3 savedPosition = new Vector3(posX, posY, posZ);
        transform.position = savedPosition;

        //Debug.Log($"Position loaded: {savedPosition}");
    }

    public void ForceSavePosition()
    {
        SavePlayerPosition();
    }

    public void ForceLoadPosition()
    {
        LoadPlayerPosition();
    }
    #endregion
}