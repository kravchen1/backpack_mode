using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonSettingsBattle : ButtonSettings
{
    public override void ChangeActive()
    {
        settingsCanvas.SetActive(!settingsCanvas.activeSelf);
    }
}
