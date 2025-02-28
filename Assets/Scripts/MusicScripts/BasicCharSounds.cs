using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class BasicCharSounds : MonoBehaviour
{
    public GameObject attackScreamObject;
    public GameObject weaponAttackObject;
    public void AttackScream()
    {
        attackScreamObject.GetComponent<AudioSource>().Play();
    }

    public void WeaponAttackSound()
    {
        weaponAttackObject.GetComponent<AudioSource>().Play();
    }
}

