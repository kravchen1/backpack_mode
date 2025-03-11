using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EducationController : MonoBehaviour
{
    public GameObject page1, page2, page3;
    public GameObject Education;

    public void Page1Forward()
    {
        page1.SetActive(false);
        page2.SetActive(true);
    }

    public void Page2Forward()
    {
        page2.SetActive(false);
        page3.SetActive(true);
    }

    public void Page2Back()
    {
        page2.SetActive(false);
        page1.SetActive(true);
    }

    public void Page3Back()
    {
        page3.SetActive(false);
        page2.SetActive(true);
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