using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Video;


public class CutsceneManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        StartCutscene();
    }
    public void StartCutscene()
    {
        videoPlayer.loopPointReached += HandleVideoEnd;
        videoPlayer.Play();
    }
    public void SkipCutscene()
    {
        videoPlayer.Stop();
        HandleVideoEnd(videoPlayer);
    }
    private void HandleVideoEnd(VideoPlayer vp)
    {
        PlayerPrefs.SetInt("NeedCutscene", 0);
        PlayerPrefs.SetString("currentLocation", "GenerateMapInternumFortress1");
        SceneLoader.Instance.LoadScene("GenerateMapInternumFortress1");
    }


    private void Update()
    {
        if (!PlayerPrefs.HasKey("NeedCutscene") && Input.GetKeyDown(KeyCode.Escape))
        {
            SkipCutscene();
        }
    }
}

