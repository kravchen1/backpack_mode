using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSpawner : MonoBehaviour
{
    public List<GameObject> battleSpawns;
    public List<GameObject> battlesPrefabs;
    public string Biom = "1";
    private BattlesSpawnerData battlesSpawnerData;
    private void Start()
    {
        battlesSpawnerData = new BattlesSpawnerData();
        //PlayerPrefs.SetInt("NeedSpawnEnemys", 1);
        if (PlayerPrefs.GetInt("NeedSpawnEnemys") == 1)
        {
            battlesSpawnerData = new BattlesSpawnerData();
            battlesSpawnerData.battlesSpawnerDataClass = new BattlesSpawnerDataClass();
            battlesSpawnerData.battlesSpawnerDataClass.battleData = new List<BattleData>();
            Generate();
        }
        else
        {
            if (File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn" + Biom + ".json")))
            {
                battlesSpawnerData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn" + Biom + ".json"));
                GenerateFromFile();
            }
            else
            {
                Debug.Log(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn" + Biom + ".json") + "�� ����������");
            }
        }
    }

    private void GenerateFromFile()
    {
        for (int i = 0; i < battleSpawns.Count; i++)
        {
            List<BattleData> battleDatas = battlesSpawnerData.battlesSpawnerDataClass.battleData.Where(e => e.id == i).ToList();
            if (battleDatas.Count != 0)
            {
                var instPref = Instantiate(battlesPrefabs[battleDatas[0].type], battleSpawns[i].transform);
                instPref.GetComponent<Enemy>().lvlEnemy = battleDatas[0].lvlEnemy;
            }
        }
    }

    private void Generate()
    {
        for (int i = 0; i < battleSpawns.Count; i++) {
            int randomCreate = Random.Range(0, 2);
            if (randomCreate == 1)//������ �����
            {
                int randomPrefab = Random.Range(0, battlesPrefabs.Count);
                var instPref = Instantiate(battlesPrefabs[randomPrefab], battleSpawns[i].transform);

                int randomLevel = Random.Range(1, 16);
                instPref.GetComponent<Enemy>().lvlEnemy = randomLevel;

                BattleData battleData = new BattleData(i, randomPrefab, randomLevel);
                battlesSpawnerData.battlesSpawnerDataClass.battleData.Add(battleData);
            }
        }
        battlesSpawnerData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn" + Biom + ".json"));
        PlayerPrefs.SetInt("NeedSpawnEnemys", 0);
    }


}