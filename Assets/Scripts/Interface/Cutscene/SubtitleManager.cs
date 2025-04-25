using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Linq;

public class SubtitleManager : MonoBehaviour
{
    public TextAsset subtitlesFile; // JSON-файл с субтитрами
    public TextMeshPro subtitleText; // Текстовое поле для вывода
    private List<SubtitleText> subtitles;

    void Start()
    {
        LoadSubtitles();
    }

    void LoadSubtitles()
    {
        if (subtitlesFile == null)
        {
            Debug.LogError("Subtitles file not assigned!");
            return;
        }

    }
}