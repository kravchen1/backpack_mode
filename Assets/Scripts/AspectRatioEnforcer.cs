using UnityEngine;

public class AspectRatioEnforcer : MonoBehaviour
{
    private Camera _camera;
    private float _targetAspect = 16f / 9f; // 16:9

    private void Start()
    {
        _camera = GetComponent<Camera>();
        UpdateViewport();
    }

    private void Update()
    {
        // ���������, ���� ��������� ������ ���� (�����������)
        if (Screen.width != _lastWidth || Screen.height != _lastHeight)
        {
            UpdateViewport();
        }
    }

    private int _lastWidth;
    private int _lastHeight;

    private void UpdateViewport()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;

        float currentAspect = (float)Screen.width / Screen.height;

        // ���� ����� ���� 16:9 (��������, 21:9) � ��������� ������ ���� �� �����
        if (currentAspect > _targetAspect)
        {
            float scaleHeight = 1f;
            float scaleWidth = scaleHeight * (_targetAspect / currentAspect);
            float offsetX = (1f - scaleWidth) / 2f;

            _camera.rect = new Rect(offsetX, 0, scaleWidth, scaleHeight);
        }
        // ���� ����� ��� 16:9 (��������, 4:3) � ��������� ������ ���� ������ � �����
        else
        {
            float scaleWidth = 1f;
            float scaleHeight = scaleWidth * (currentAspect / _targetAspect);
            float offsetY = (1f - scaleHeight) / 2f;

            _camera.rect = new Rect(0, offsetY, scaleWidth, scaleHeight);
        }
    }
}