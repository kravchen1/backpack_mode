using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscController : MonoBehaviour
{
    public GameObject ToogleCanvas;
    private bool isPaused = false;


    public GameObject EducationEncyclopedia;
    public GameObject EducationStart;
    public GameObject backpackEnemy;
    public GameObject caveMap;
    public GameObject settings;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "Cave"
                || SceneManager.GetActiveScene().name == "GenerateMap"
                || SceneManager.GetActiveScene().name == "GenerateMapFortress1"
                || SceneManager.GetActiveScene().name == "GenerateMapInternumFortress1"
                )
            {
                if (CloseEducationOrNull())
                {
                    if (settings != null && settings.activeSelf)
                    {
                        settings.GetComponent<ButtonSettings>().ChangeActive();
                    }
                    else if (caveMap != null && caveMap.activeSelf)
                    {
                        caveMap.SetActive(false);
                    }
                    else if (backpackEnemy != null && backpackEnemy.activeSelf)
                    {
                        GameObject canvasBackpackEnemy = GameObject.FindGameObjectWithTag("backpack");
                        GenerateBackpackOnMap generateBackpackOnMap = canvasBackpackEnemy.GetComponent<GenerateBackpackOnMap>();
                        generateBackpackOnMap.CloseBackpackEnemy();
                    }
                    else
                    {
                        TogglePause();
                    }
                }
                else
                {
                    if (EducationStart != null)
                    {
                        EducationStart.SetActive(false);
                    }
                }
            }
            else
            {
                if (CloseEducationOrNull())
                {
                    ExitBackpack();
                }
                else
                {
                    bool b = false;
                    if (EducationStart != null)
                    {
                        if (EducationStart.activeSelf)
                        {
                            EducationStart.SetActive(false);
                            b = true;
                        }

                    }
                    if (!b)
                    {
                        if (EducationEncyclopedia != null)
                        {
                            EducationEncyclopedia.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    private bool CloseEducationOrNull()
    {
        if((EducationEncyclopedia == null || !EducationEncyclopedia.activeSelf)
            && (EducationStart == null || !EducationStart.activeSelf))
        {
            return true;
        }
        return false;
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        // Останавливаем или возобновляем время
        if(SceneManager.GetActiveScene().name != "BackPackBattle")
            Time.timeScale = isPaused ? 0 : 1;

        // Включаем или выключаем меню паузы
        if (ToogleCanvas != null)
        {
            ToogleCanvas.SetActive(isPaused);
        }
    }

    void ExitBackpack()
    {
        GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();

        PlayerPrefs.SetString("ComputerName", System.Environment.MachineName.Replace("-", "_"));
        PlayerPrefs.SetInt("IdBackpack", PlayerPrefs.GetInt("IdBackpack") + 1);
        GameObject.Find("backpack").GetComponent<BackpackData>().SaveNewData(Path.Combine(PlayerPrefs.GetString("savePathTestBackpack"), PlayerPrefs.GetString("ComputerName") + "_" + PlayerPrefs.GetInt("IdBackpack").ToString() + ".json"));

        GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
        GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
        //Debug.Log("Unload");
        //SceneManager.UnloadSceneAsync("BackPack");
        //SceneManager.LoadScene(PlayerPrefs.GetString("currentLocation"));
        SceneLoader.Instance.LoadScene(PlayerPrefs.GetString("currentLocation"));
    }
}

