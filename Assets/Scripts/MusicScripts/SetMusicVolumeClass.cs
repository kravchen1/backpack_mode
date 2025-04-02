using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SetMusicVolumeClass : MonoBehaviour
{
    public void SetMusicVolume()
    {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", 1);
    }

    private void Awake()
    {
        SetMusicVolume();
    }
}

