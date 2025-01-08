using UnityEngine;

public class ButtonNewGame : Button
{
    [SerializeField] protected GameObject mainCanvas;
    [SerializeField] protected GameObject chooseCharCanvas;

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

    protected void ChangeActive()
    {
        mainCanvas.SetActive(!mainCanvas.activeSelf);
        chooseCharCanvas.SetActive(!chooseCharCanvas.activeSelf);
    }
}
