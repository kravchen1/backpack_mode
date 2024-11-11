using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public void MoveCamera(Vector2 vector)
    {
        gameObject.transform.position = vector;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
