using UnityEngine;

public class Draw : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
