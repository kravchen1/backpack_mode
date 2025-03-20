using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    void Start()
    {
        if (SteamAPI.Init())
        {
            Debug.Log("Steamworks initialized successfully.");
        }
        else
        {
            Debug.LogError("Failed to initialize Steamworks.");
        }
    }

    void OnDestroy()
    {
        SteamAPI.Shutdown();
    }

    void Update()
    {
        SteamAPI.RunCallbacks();
    }
}
