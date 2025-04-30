using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using UnityEngine.Video;


public class CutsceneManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public TextMeshPro text;
    private Coroutine hideCoroutine;
    public float hideDelay = 3f;
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

    private IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        text.enabled = false;
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            text.enabled = true;
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }
            hideCoroutine = StartCoroutine(HideTextAfterDelay());
        }
        if (Input.GetKeyDown(KeyCode.Escape) && text.enabled && videoPlayer.time > 5.0)
        {
            SkipCutscene();
        }
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    SkipCutscene();
        //}
    }
}

