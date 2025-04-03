using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartEducationEatController : MonoBehaviour
{
    public GameObject page1, page2;

    public GameObject startEducation;

    private void Start()
    {
        if(PlayerPrefs.GetInt("StartEatEducation") == 0)
        {
            OpenEducation();
            PlayerPrefs.SetInt("StartEatEducation", 1);
        }
    }

    public void CloseEducation()
    {
        startEducation.SetActive(false);
    }

    public void OpenEducation()
    {
        startEducation.SetActive(true);
    }


    public void Page1Forward()
    {
        page1.SetActive(false);
        page2.SetActive(true);
    }

    public void Page2Back()
    {
        page2.SetActive(false);
        page1.SetActive(true);
    }
    public void Page2End()
    {
        CloseEducation();
    }
}