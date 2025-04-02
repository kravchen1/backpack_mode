using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea(3, 10)]
    public string dialogueText;
    public LocalizationText dialogueLocalizationText;
    public Response[] responses;
}