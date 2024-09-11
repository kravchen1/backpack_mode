using UnityEngine;

public class ButtonNewGame : Button
{
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject chooseCharCanvas;

    public override void OnMouseUpAsButton()
    {
        switch (gameObject.name)
        {
            case "Button_MainMenu":
                ChangeActive();
                break;
            case "Button_NewGame":
                Time.timeScale = 1f;
                ChangeActive();
                break;
        }
    }

    void ChangeActive()
    {
        mainCanvas.SetActive(!mainCanvas.activeSelf);
        chooseCharCanvas.SetActive(!chooseCharCanvas.activeSelf);
    }
}
