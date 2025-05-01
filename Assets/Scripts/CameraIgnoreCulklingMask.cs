using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class CameraIgnoreCullingMask : MonoBehaviour
{

    private void Start()
    {
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("IgnoreMouse"));
    }


    private void Update()
    {
        
    }

}
