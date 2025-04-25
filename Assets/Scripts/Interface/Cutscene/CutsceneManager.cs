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
    private float holdTimer = 0f;
    private bool isHolding = false;
    public float requiredHoldTime = 3f;
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

    private void ResetProgress()
    {
        isHolding = false;
        holdTimer = 0f;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isHolding = true;
            holdTimer = 0f;
        }
        if (isHolding && Input.GetKey(KeyCode.Escape))
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= requiredHoldTime)
            {
                SkipCutscene();
            }
            if (isHolding && Input.GetKeyUp(KeyCode.Escape))
            {
                ResetProgress();
            }
        }
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    SkipCutscene();
        //}
    }
}

