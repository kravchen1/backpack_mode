using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EducationGlobalMapController : MonoBehaviour
{
    public GameObject Education;

    public GameObject Page1, Page2;

    private void Start()
    {
        if (PlayerPrefs.GetInt("EducationGlobalMap") == 0)
        {
            OpenEducation();
            PlayerPrefs.SetInt("EducationGlobalMap", 1);
        }
    }

    public void CloseEducation()
    {
        Education.SetActive(false);
    }

    public void OpenEducation()
    {
        Education.SetActive(!Education.activeSelf);
    }

    public void Page1Forward()
    {
        Page1.SetActive(false);
        Page2.SetActive(true);
    }

    public void Page2Back()
    {
        Page2.SetActive(false);
        Page1.SetActive(true);
    }
}