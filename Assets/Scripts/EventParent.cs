using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


public class EventParent : MonoBehaviour
{
    public bool isShowPressE = true;
    public GameObject infoText;

    private string settingLanguage = "en";

    public void SetActivePressE(bool active)
    {
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");

        string pressE_text = LocalizationManager.Instance.GetTextUI(settingLanguage, "pressE_text");

        TMP_Text text;
        if (infoText.GetComponent<TextMeshPro>() != null)
            text = infoText.GetComponent<TextMeshPro>();
        else
            text = infoText.GetComponent<TextMeshProUGUI>();

        text.text = pressE_text;

        infoText.SetActive(active);
    }

    public void checkCameraPositionAndSavePlayerPosition(GameObject player)
    {
        var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MoveCamera>();
        var rtPlayer = player.GetComponent<RectTransform>().anchoredPosition;

        if (rtPlayer.x <= camera.minX)
        {
            PlayerPrefs.SetFloat("PostionMapX", camera.minX + 1);
        }
        else if (rtPlayer.x >= camera.maxX)
        {
            PlayerPrefs.SetFloat("PostionMapX", camera.maxX - 1);
        }
        else
        {
            PlayerPrefs.SetFloat("PostionMapX", rtPlayer.x);
        }

        if (rtPlayer.y <= camera.minY)
        {
            PlayerPrefs.SetFloat("PostionMapY", camera.minY + 1);
        }
        else if (rtPlayer.y >= camera.maxY)
        {
            PlayerPrefs.SetFloat("PostionMapY", camera.maxY - 1);
        }
        else
        {
            PlayerPrefs.SetFloat("PostionMapY", rtPlayer.y);
        }
    }
}



