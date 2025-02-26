using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsEscController : MonoBehaviour
{
    public void BackToMainMenu()
    {
        PlayerPrefs.SetString("savePath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Backpack Seeker's"));
        if (!Directory.Exists(PlayerPrefs.GetString("savePath")))
        {
            Directory.CreateDirectory(PlayerPrefs.GetString("savePath"));
        }
        DeleteAllData();
        StartBackPack();
        StartQeust();

    }

}