using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SoundVolume : MonoBehaviour
{
    public void SetSoundVolume()
    {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1);
    }
}

