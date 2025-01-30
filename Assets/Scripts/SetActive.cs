using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SetActive:MonoBehaviour
{
    public void ToggleActive()
    {
        // Меняем состояние активности объекта на противоположное
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
