using UnityEngine;
using UnityEngine.EventSystems;

public class DevButtons : MonoBehaviour
{
    public void DeleteAllPlayerPrefs()
    {
        Debug.Log("DeleteAllPlayerPrefs");
        PlayerPrefs.DeleteAll();
    }
}