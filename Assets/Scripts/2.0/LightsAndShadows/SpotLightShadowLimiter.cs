using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AdvancedSpotLight : MonoBehaviour
{
    [Header("Light Components")]
    public Light2D primaryLight;      // �������� ���� � ������
    public Light2D secondaryLight;    // �������������� ���� ��� �����

    [Header("Light Settings")]
    public float shadowLength = 5f;
    public float totalLength = 15f;
    [Range(0f, 1f)]
    public float primaryIntensity = 1f;
    [Range(0f, 1f)]
    public float secondaryIntensity = 0.3f;

    void Start()
    {
        InitializeLights();
    }

    void InitializeLights()
    {
        // ������� ��� ����������� �������� ���� (� ������)
        if (primaryLight == null)
            primaryLight = CreateLight("PrimarySpotLight", true);

        // ������� ��� ����������� �������������� ���� (��� �����)
        if (secondaryLight == null)
            secondaryLight = CreateLight("SecondarySpotLight", false);

        UpdateLightParameters();
    }

    Light2D CreateLight(string name, bool withShadows)
    {
        GameObject lightObj = new GameObject(name);
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = Vector3.zero;
        lightObj.transform.localRotation = Quaternion.identity;

        Light2D light = lightObj.AddComponent<Light2D>();
        light.lightType = Light2D.LightType.Point;
        light.color = Color.white;

        if (withShadows)
        {
            light.shadowsEnabled = true;
            light.shadowIntensity = 0.8f;
        }
        else
        {
            light.shadowsEnabled = false;
        }

        return light;
    }

    void UpdateLightParameters()
    {
        // ��������� ��������� ����� (��������, � ������)
        if (primaryLight != null)
        {
            primaryLight.intensity = primaryIntensity;
            primaryLight.falloffIntensity = 0.9f; // ������ ���������
            UpdateLightShape(primaryLight, shadowLength);
        }

        // ��������� ��������������� ����� (�������, ��� �����)
        if (secondaryLight != null)
        {
            secondaryLight.intensity = secondaryIntensity;
            secondaryLight.falloffIntensity = 0.3f; // ������� ���������
            secondaryLight.color = primaryLight.color * 0.8f; // ���� ������
            UpdateLightShape(secondaryLight, totalLength);
        }
    }

    void UpdateLightShape(Light2D light, float length)
    {
        if (light.shapePath != null && light.shapePath.Length == 4)
        {
            light.shapePath[0] = new Vector2(-0.5f, 0);
            light.shapePath[1] = new Vector2(0.5f, 0);
            light.shapePath[2] = new Vector2(0.25f, length);
            light.shapePath[3] = new Vector2(-0.25f, length);
        }
    }

    // ������ ��� ������������� ��������� ����������
    public void SetShadowLength(float newLength)
    {
        shadowLength = newLength;
        UpdateLightParameters();
    }

    public void SetTotalLength(float newLength)
    {
        totalLength = newLength;
        UpdateLightParameters();
    }

    private void OnValidate()
    {
        if (primaryLight != null && secondaryLight != null)
        {
            UpdateLightParameters();
        }
    }
}