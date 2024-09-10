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
                ChangeActive(false);
                break;
            case "Button_NewGame":
                ChangeActive(true);
                break;
        }
    }

    void ChangeActive(bool active)
    {
        mainCanvas.SetActive(!active);
        chooseCharCanvas.SetActive(active);
    }
}
