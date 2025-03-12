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
}