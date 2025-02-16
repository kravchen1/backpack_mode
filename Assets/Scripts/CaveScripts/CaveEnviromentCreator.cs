using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CaveEnviromentCreator : MonoBehaviour
{
    public List<GameObject> caveEnviromentPrefabs;

    private void Start()
    {
        CreateCaveEnviroment();
    }

    void CreateCaveEnviroment()
    {
        foreach (var prefab in caveEnviromentPrefabs)
        {
            var r = UnityEngine.Random.Range(0, 2);
            if (r > 0)
            {
                Instantiate(prefab, gameObject.transform);
            }
        }
    }
}

