using TMPro;
using UnityEngine;

public class DialogCanvas : MonoBehaviour
{
    public TextMeshPro textDialogCanvas;
    public void GenerateEvent()
    {
        int r = 0;
        r = Random.RandomRange(1, 3);
        switch(r)
        {
                case 1:
                    textDialogCanvas.text = "The chest looks attractive, it seems to be filled with unreal imbues. Open?";
                    break;
                case 2:
                    textDialogCanvas.text = "The chest looks attractive, but there is a feeling that it sometimes breathes and it seems that the lid opens slightly and Alexey looks at you from it. Open?";
                    textDialogCanvas.fontSize = 400;
                    break;
        }
    }
}
