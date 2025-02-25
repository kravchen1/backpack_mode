using System.Collections.Generic;
using UnityEngine;

public class ButtonsScrollController : MonoBehaviour
{
    public List<GameObject> characters;
    public List<GameObject> caractersDescriptions;

    private int currentChoice = 0;
    public int countChoices = 2;

    private void Start()
    {
        countChoices = countChoices - 1;
    }
    public void ScrollLeft()
   {
        Deactive(currentChoice);

        currentChoice--;
        if(currentChoice < 0)
        {
            currentChoice = countChoices;
        }

        Active(currentChoice);
   }

    public void ScrolRight()
    {
        Deactive(currentChoice);

        currentChoice++;
        if (currentChoice > countChoices)
        {
            currentChoice = 0;
        }

        Active(currentChoice);
    }

    public void Deactive(int index)
    {
        characters[index].gameObject.SetActive(false);
        caractersDescriptions[index].gameObject.SetActive(false);
    }

    public void Active(int index)
    {
        characters[index].gameObject.SetActive(true);
        caractersDescriptions[index].gameObject.SetActive(true);
    }
}