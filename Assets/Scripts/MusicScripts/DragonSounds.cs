using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class DragonSounds : MonoBehaviour
{
    public GameObject dragonRoarObject;
    public GameObject dragonWingsObject;
    public void PlayDragonRoar()
    {
        dragonRoarObject.GetComponent<AudioSource>().Play();
    }

    public void PlayDragonWings()
    {
        dragonWingsObject.GetComponent<AudioSource>().Play();
    }
}

