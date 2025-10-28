using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartNewGame : MonoBehaviour
{
    public RotateCustomizationCharacter rotateCustomizationCharacter;
    public void StartGameWithoutSave()
    {
        if (PlayerPrefsMigrationManager.Instance != null)
        {
            PlayerPrefsMigrationManager.Instance.ClearAllPlayerPrefsAndKeys();
        }

        PlayerPrefs.SetInt("PlayerBodyIndex", rotateCustomizationCharacter.bodyIndex);
        PlayerPrefs.SetInt("PlayerHairIndex", rotateCustomizationCharacter.hairIndex);
        PlayerPrefs.SetInt("PlayerEyeIndex", rotateCustomizationCharacter.eyeIndex);

        PlayerPrefs.SetFloat("PlayerBodyColor_r", rotateCustomizationCharacter.bodyColor.r);
        PlayerPrefs.SetFloat("PlayerBodyColor_g", rotateCustomizationCharacter.bodyColor.g);
        PlayerPrefs.SetFloat("PlayerBodyColor_b", rotateCustomizationCharacter.bodyColor.b);

        PlayerPrefs.SetFloat("PlayerHeadColor_r", rotateCustomizationCharacter.headColor.r);
        PlayerPrefs.SetFloat("PlayerHeadColor_g", rotateCustomizationCharacter.headColor.g);
        PlayerPrefs.SetFloat("PlayerHeadColor_b", rotateCustomizationCharacter.headColor.b);

        PlayerPrefs.SetFloat("PlayerEyeColor_r", rotateCustomizationCharacter.eyeColor.r);
        PlayerPrefs.SetFloat("PlayerEyeColor_g", rotateCustomizationCharacter.eyeColor.g);
        PlayerPrefs.SetFloat("PlayerEyeColor_b", rotateCustomizationCharacter.eyeColor.b);

        PlayerPrefsMigrationManager.Instance.RegisterIntPref("PlayerBodyIndex");
        PlayerPrefsMigrationManager.Instance.RegisterIntPref("PlayerHairIndex");
        PlayerPrefsMigrationManager.Instance.RegisterIntPref("PlayerEyeIndex");

        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerBodyColor_r");
        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerBodyColor_g");
        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerBodyColor_b");

        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerHeadColor_r");
        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerHeadColor_g");
        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerHeadColor_b");

        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerEyeColor_r");
        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerEyeColor_g");
        PlayerPrefsMigrationManager.Instance.RegisterFloatPref("PlayerEyeColor_b");

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.OpenLoadingCanvas();
            StartCoroutine(LoadSceneDelayed());

        }
    }

    private IEnumerator LoadSceneDelayed()
    {
        yield return null;
        yield return null;
        yield return null;
        SceneLoader.Instance.LoadSceneNewGame("MainGame");
    }
}