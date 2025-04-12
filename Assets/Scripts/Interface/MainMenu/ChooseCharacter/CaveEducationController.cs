using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveEducationController : MonoBehaviour
{
  

    public GameObject caveEducation;

    private void Start()
    {
        //PlayerPrefs.SetString("LanguageSettings", "ru");
        PlayerPrefs.SetInt("CaveEducation", 0);
        if (PlayerPrefs.GetInt("CaveEducation") == 0)
        {
            OpenEducation();
            PlayerPrefs.SetInt("CaveEducation", 1);
        }
    }

    public void CloseEducation()
    {
        caveEducation.SetActive(false);
    }

    public void OpenEducation()
    {
        caveEducation.SetActive(true);
    }
}