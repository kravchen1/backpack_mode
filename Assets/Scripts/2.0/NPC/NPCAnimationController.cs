using UnityEngine;
using UnityEngine.AI;

public class NPCAnimationController : MonoBehaviour
{
    #region Settings
    [Header("Appearance")]
    public int bodyIndex = 2;
    public int hairIndex = 0;
    public int eyeIndex = 1;
    
    public SpriteRenderer head;
    public SpriteRenderer body;
    public SpriteRenderer hair;
    public SpriteRenderer eye;

    private Color bodyColor = Color.white;
    private Color headColor = Color.white;
    private Color hairColor = Color.white;
    private Color eyeColor = Color.white;

    [Header("Animation References")]
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

    [Header("Forced Look Settings")]
    [SerializeField] private float forcedLookDuration = 3f;

    public enum FlipMode { ByMovement, ByLastDirection, Manual }
    public enum BaseFlipDirection { Right, Left }
    #endregion

    #region Components & State
    private NavMeshAgent navMeshAgent;
    private Vector2 movement;
    private Vector2 lastNonZeroDirection = new Vector2(0, 1);
    private Vector2 animationVelocity;
    private float currentFlipVelocity;
    private float targetScaleX = 1f;

    // Forced look system
    private bool isForcedLooking = false;
    private float forcedLookTimer = 0f;
    private Vector2 forcedLookDirection;

    // Кэш для системы анимаций (как в TopDownCharacterController)
    private MovementDirection _currentMovementDirection = MovementDirection.Down;
    private MovementDirection _lastMovementDirection = MovementDirection.Down;
    private bool _wasMoving = false;
    private bool _isMoving = false;
    #endregion

    #region Enums
    public enum MovementDirection { Down, Up, Left, Right }
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        InitializeComponents();
        InitializeAppearance();
        InitializeGraphics();
    }

    void Start()
    {
        RefreshAppearance();
    }

    void Update()
    {
        UpdateForcedLook();
        UpdateMovementFromNavMesh();
        HandleFlip();
        UpdateAnimations();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void InitializeGraphics()
    {
        if (graphicsTransform == null && transform.childCount > 0)
            graphicsTransform = transform.GetChild(0);

        if (graphicsTransform != null)
            targetScaleX = graphicsTransform.localScale.x;
    }

    private void InitializeAppearance()
    {
        bodyColor = body.color;
        headColor = head.color;
        hairColor = hair.color;
        eyeColor  = eye.color;
    }
    #endregion

    #region Appearance System
    //private Color LoadColor(string key, Color defaultColor)
    //{
    //    // Если ключ не существует, возвращаем цвет по умолчанию
    //    if (!PlayerPrefs.HasKey(key + "_r"))
    //        return defaultColor;

    //    float r = PlayerPrefs.GetFloat(key + "_r", defaultColor.r);
    //    float g = PlayerPrefs.GetFloat(key + "_g", defaultColor.g);
    //    float b = PlayerPrefs.GetFloat(key + "_b", defaultColor.b);
    //    float a = PlayerPrefs.GetFloat(key + "_a", defaultColor.a);

    //    return new Color(r, g, b, a);
    //}

    //public void SaveAppearance()
    //{
    //    // Сохраняем индексы
    //    PlayerPrefs.SetInt("NPC_BodyIndex", bodyIndex);
    //    PlayerPrefs.SetInt("NPC_HairIndex", hairIndex);
    //    PlayerPrefs.SetInt("NPC_EyeIndex", eyeIndex);

    //    // Сохраняем цвета
    //    SaveColor("NPC_BodyColor", bodyColor);
    //    SaveColor("NPC_HairColor", hairColor);
    //    SaveColor("NPC_EyeColor", eyeColor);

    //    PlayerPrefs.Save();
    //}

    //private void SaveColor(string key, Color color)
    //{
    //    PlayerPrefs.SetFloat(key + "_r", color.r);
    //    PlayerPrefs.SetFloat(key + "_g", color.g);
    //    PlayerPrefs.SetFloat(key + "_b", color.b);
    //    PlayerPrefs.SetFloat(key + "_a", color.a);
    //}

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

    private void UpdateAppearanceSprites(MovementDirection movementDir)
    {
        if (CharacterAppearanceManager.Instance == null) return;

        switch (movementDir)
        {
            case MovementDirection.Up:
                if (head != null) head.sprite = CharacterAppearanceManager.Instance.GetHeadUp();
                if (body != null) body.sprite = CharacterAppearanceManager.Instance.GetBodyUp(bodyIndex);
                if (hair != null) hair.sprite = CharacterAppearanceManager.Instance.GetHairUp(hairIndex);
                if (eye != null) eye.gameObject.SetActive(false);
                break;

            case MovementDirection.Down:
                if (head != null) head.sprite = CharacterAppearanceManager.Instance.GetHeadDown();
                if (body != null) body.sprite = CharacterAppearanceManager.Instance.GetBodyDown(bodyIndex);
                if (hair != null) hair.sprite = CharacterAppearanceManager.Instance.GetHairDown(hairIndex);
                if (eye != null)
                {
                    eye.gameObject.SetActive(true);
                    eye.sprite = CharacterAppearanceManager.Instance.GetEyeDown(eyeIndex);
                }
                break;

            case MovementDirection.Right:
            case MovementDirection.Left:
                if (head != null) head.sprite = CharacterAppearanceManager.Instance.GetHeadSide();
                if (body != null) body.sprite = CharacterAppearanceManager.Instance.GetBodySide(bodyIndex);
                if (hair != null) hair.sprite = CharacterAppearanceManager.Instance.GetHairSide(hairIndex);
                if (eye != null)
                {
                    eye.gameObject.SetActive(true);
                    eye.sprite = CharacterAppearanceManager.Instance.GetEyeSide(eyeIndex);
                }
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

    #region Forced Look System
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
    #endregion

    #region Movement & Animation
    private void UpdateMovementFromNavMesh()
    {
        if (navMeshAgent != null)
        {
            // Получаем направление движения из velocity NavMeshAgent
            movement = new Vector2(navMeshAgent.velocity.x, navMeshAgent.velocity.y);
            _isMoving = movement.magnitude > walkThreshold;
        }
        else
        {
            movement = Vector2.zero;
            _isMoving = false;
        }
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        // Приоритет у принудительного поворота
        Vector2 animationDirection;
        if (isForcedLooking)
        {
            animationDirection = forcedLookDirection;
            _currentMovementDirection = GetCurrentMovementDirection(forcedLookDirection);
        }
        else
        {
            // Определяем направление для анимаций (как в TopDownCharacterController)
            if (_isMoving)
            {
                lastNonZeroDirection = movement.normalized;
                _currentMovementDirection = GetCurrentMovementDirection(movement);
            }
            else
            {
                _currentMovementDirection = GetCurrentMovementDirection(lastNonZeroDirection);
            }
            animationDirection = _isMoving ? movement.normalized : lastNonZeroDirection;
        }

        // Проверяем, изменилось ли состояние (как в TopDownCharacterController)
        bool stateChanged = _currentMovementDirection != _lastMovementDirection ||
                           _isMoving != _wasMoving;

        // Обновляем спрайты если состояние изменилось
        if (stateChanged)
        {
            UpdateAppearanceSprites(_currentMovementDirection);
            _lastMovementDirection = _currentMovementDirection;
            _wasMoving = _isMoving;
        }

        // Устанавливаем параметры аниматора с плавным переходом (как в TopDownCharacterController)
        animator.SetFloat(horizontalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(horizontalAnimParam), animationDirection.x,
                            ref animationVelocity.x, animationSmoothTime));

        animator.SetFloat(verticalAnimParam,
            Mathf.SmoothDamp(animator.GetFloat(verticalAnimParam), animationDirection.y,
                            ref animationVelocity.y, animationSmoothTime));

        // Определяем скорость анимации в зависимости от состояния движения
        float speedValue = 0f;
        if (_isMoving)
        {
            // Нормализуем скорость относительно максимальной скорости агента
            speedValue = Mathf.Clamp01(navMeshAgent.velocity.magnitude / navMeshAgent.speed);

            // Дополнительные модификаторы скорости анимации (как в TopDownCharacterController)
            if (TryGetComponent<NPC>(out var npcController))
            {
                if (npcController.IsHostile())
                {
                    speedValue *= 1.2f; // Ускоренная анимация при преследовании
                }
            }
        }

        // Если в аниматоре есть параметр Speed
        if (AnimatorHasParameter(animator, "Speed"))
        {
            animator.SetFloat("Speed", speedValue);
        }

        // Если в аниматоре есть параметр IsMoving
        if (AnimatorHasParameter(animator, "IsMoving"))
        {
            animator.SetBool("IsMoving", _isMoving);
        }
    }

    private MovementDirection GetCurrentMovementDirection(Vector2 direction)
    {
        // Та же логика определения направления как в TopDownCharacterController
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

    private bool AnimatorHasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
    #endregion

    #region Flip System (одинаковая логика с TopDownCharacterController)
    private void HandleFlip()
    {
        if (!enableFlip || graphicsTransform == null) return;

        // Приоритет у принудительного поворота
        if (isForcedLooking)
        {
            if (Mathf.Abs(forcedLookDirection.x) > 0.1f)
            {
                float direction = Mathf.Sign(forcedLookDirection.x);
                targetScaleX = GetBaseDirection() * direction;
            }
        }
        else
        {
            switch (flipMode)
            {
                case FlipMode.ByMovement: UpdateFlipByMovement(); break;
                case FlipMode.ByLastDirection: UpdateFlipByLastDirection(); break;
                case FlipMode.Manual: // Оставляем текущее targetScaleX без изменений
                    break;
            }
        }

        ApplySmoothFlip();
    }

    private void UpdateFlipByMovement()
    {
        if (_isMoving && Mathf.Abs(movement.x) > 0.1f)
        {
            float direction = Mathf.Sign(movement.x);
            targetScaleX = GetBaseDirection() * direction;
        }
    }

    private void UpdateFlipByLastDirection()
    {
        if (_isMoving)
            lastNonZeroDirection = movement.normalized;

        if (Mathf.Abs(lastNonZeroDirection.x) > 0.1f)
        {
            float direction = Mathf.Sign(lastNonZeroDirection.x);
            targetScaleX = GetBaseDirection() * direction;
        }
    }

    // Тот же метод для определения базового направления
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

    #region Public API - Flip Control (одинаковые методы)
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

    #region Public API - Forced Look
    public void ForceLookAt(Vector2 direction, float duration = -1f)
    {
        isForcedLooking = true;
        forcedLookDirection = direction.normalized;
        forcedLookTimer = duration > 0 ? duration : forcedLookDuration;

        // Сразу применяем поворот
        if (Mathf.Abs(forcedLookDirection.x) > 0.1f)
        {
            float directionSign = Mathf.Sign(forcedLookDirection.x);
            targetScaleX = GetBaseDirection() * directionSign;
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

    public bool IsForcedLooking() => isForcedLooking;
    #endregion

    #region Public API - Movement Info
    public Vector2 GetMovementDirection() => movement;
    public Vector2 GetFacingDirection() => lastNonZeroDirection;
    public bool IsMoving() => _isMoving;
    public MovementDirection GetCurrentMovementDirection() => _currentMovementDirection;
    #endregion

    #region Public API - Appearance
    public void SetBodyIndex(int index)
    {
        bodyIndex = index;
        RefreshAppearance();
    }

    public void SetHairIndex(int index)
    {
        hairIndex = index;
        RefreshAppearance();
    }

    public void SetEyeIndex(int index)
    {
        eyeIndex = index;
        RefreshAppearance();
    }

    public void SetBodyColor(Color color)
    {
        bodyColor = color;
        RefreshAppearance();
    }

    public void SetHairColor(Color color)
    {
        hairColor = color;
        RefreshAppearance();
    }

    public void SetEyeColor(Color color)
    {
        eyeColor = color;
        RefreshAppearance();
    }

    public void SetHeadColor(Color color)
    {
        headColor = color;
        RefreshAppearance();
    }

    public void SetFullAppearance(int bodyIdx, int hairIdx, int eyeIdx, Color bodyClr, Color hairClr, Color eyeClr, Color headClr)
    {
        bodyIndex = bodyIdx;
        hairIndex = hairIdx;
        eyeIndex = eyeIdx;
        bodyColor = bodyClr;
        hairColor = hairClr;
        eyeColor = eyeClr;
        headColor = headClr;
        RefreshAppearance();
    }
    #endregion
}