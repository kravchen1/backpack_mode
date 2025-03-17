using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class MusicVolume : MonoBehaviour
{
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider soundVolumeSlider;
    [SerializeField] List<GameObject> musicObjects;
    float savedMusicVolume;

    private void Start()
    {
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        musicObjects = GameObject.FindGameObjectsWithTag("Music").ToList();
        savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        foreach (var musicObject in musicObjects)
        {
            if (musicObject != null)
            {
                musicObject.GetComponent<AudioSource>().volume = savedMusicVolume;
            }
        }
        musicVolumeSlider.value = savedMusicVolume;
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

        float savedSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        soundVolumeSlider.value = savedSoundVolume;
        soundVolumeSlider.onValueChanged.AddListener(SetSoundVolume);

    }
    private void SetMusicVolume(float savedMusicVolume)
    {
        foreach(var musicObject in musicObjects)
        {

            if (musicObject != null)
            {
                musicObject.GetComponent<AudioSource>().volume = savedMusicVolume;
            }
        }
        PlayerPrefs.SetFloat("MusicVolume", savedMusicVolume);
    }

    private void SetSoundVolume(float savedSoundVolume)
    {
        PlayerPrefs.SetFloat("SoundVolume", savedSoundVolume);
    }
}

