using UnityEngine;

public class DynamicCameraSize : MonoBehaviour
{
    [Header("Base Resolution Settings")]
    public int baseWidth = 960;
    public int baseHeight = 540;
    public int pixelsPerUnit = 32;

    [Header("Debug")]
    public bool logChanges = true;

    private Camera _camera;
    private int _lastScreenWidth;
    private int _lastScreenHeight;
    private TopDownCameraScrollController topDownCameraScrollController;

    void Start()
    {
        _lastScreenWidth = Screen.width;
        _lastScreenHeight = Screen.height;

        topDownCameraScrollController = GetComponent<TopDownCameraScrollController>();

        UpdateCameraSize(true);
    }

    void Update()
    {
        if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
        {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            UpdateCameraSize();
        }
    }

    void UpdateCameraSize(bool forceUpdate = false)
    {
        if (topDownCameraScrollController == null) return;

        // 1. ������������ ������� ������ ������ (��� baseHeight)
        // ��� ���� ���������, ������� �� ������� �� �������� ����������.
        // ��� ��� ������, ��� ������� ���� �������� "������" �� ������� ����������.
        float baseWorldHeight = (float)baseHeight / pixelsPerUnit;
        float baseOrthoSize = baseWorldHeight / 4f; // ~8.4375 ��� 540/32

        // 2. ����������, ��������� ������� ���������� ������ ��������
        // ���������� �� ������, ��� ��� orthographicSize ��������� ������ �������.
        // ���� ���������� ����� ������ -> scaleFactor > 1 -> ������ ����� ��������.
        float resolutionScale = (float)Screen.height / baseHeight;

        // 3. ������������ ������� ������ ������ �� ���� �����������
        // �������� ������� ������ �� �����������, ����� ���������� ������
        // ��� ���������� ����������.
        float newOrthoSize = baseOrthoSize * resolutionScale;

        // 4. ��������� ����� ������
        //_camera.orthographicSize = newOrthoSize;
        topDownCameraScrollController.minOrthographicSize = newOrthoSize;
        topDownCameraScrollController.maxOrthographicSize = newOrthoSize * 2.5f;
        GameObject.FindGameObjectWithTag("Player").GetComponent<TopDownCharacterController>().moveSpeed = newOrthoSize * 2;

        // 5. ����������� ��� �����������
        if (logChanges || forceUpdate)
        {
            Debug.Log($"New Resolution: {Screen.width}x{Screen.height} " +
                      $"(Scale: x{resolutionScale:F2}) | " +
                      $"Camera Size: {newOrthoSize:F4}");
        }
    }
}