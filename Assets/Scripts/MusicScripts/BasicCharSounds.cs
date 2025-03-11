using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BasicCharSounds : MonoBehaviour
{
    public GameObject attackScreamObject;
    public GameObject weaponAttackObject;
    public void AttackScream()
    {
        if(SceneManager.GetActiveScene().name == "BackPackBattle")
            attackScreamObject.GetComponent<AudioSource>().Play();
    }

    public void WeaponAttackSound()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
            weaponAttackObject.GetComponent<AudioSource>().Play();
    }
}

