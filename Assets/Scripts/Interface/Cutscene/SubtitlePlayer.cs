using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class SubtitlePlayer : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private int currentSubtitleIndex = 0;
    private string settingLanguage;
    List<SubtitleText> subtitles;
    public TextMeshPro subtitleText;


    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");
        subtitles = LocalizationManager.Instance.GetTextSubtitles(settingLanguage, videoPlayer.clip.name);
    }

    void Update()
    {
        if (videoPlayer.isPlaying)
        {
            float currentTime = (float)videoPlayer.time;

            // Проверяем, нужно ли показать следующий субтитр
            if (currentSubtitleIndex < subtitles.Count)
            {
                SubtitleText nextSub = subtitles[currentSubtitleIndex];

                if (currentTime >= nextSub.startTime && currentTime <= nextSub.endTime)
                {
                    subtitleText.text = nextSub.text;
                }
                else if (currentTime > nextSub.endTime)
                {
                    currentSubtitleIndex++;
                    subtitleText.text = ""; // Очищаем, если субтитр закончился
                }
            }
        }
    }
}