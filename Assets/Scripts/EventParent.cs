using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class EventParent : MonoBehaviour
{
    public bool isShowPressE = true;
    public GameObject infoText;

    public void SetActivePressE(bool active)
    {
        infoText.SetActive(active);
    }
}



