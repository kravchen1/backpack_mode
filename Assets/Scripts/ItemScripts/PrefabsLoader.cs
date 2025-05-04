using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class PrefabsLoader : MonoBehaviour
{
    void Start()
    {
        if (PrefabsManager._cachedPrefabs == null || PrefabsManager._cachedPrefabs.Length == 0)
        {
            PrefabsManager._cachedPrefabs = Resources.LoadAll<GameObject>("Items/");
        }
    }
}

