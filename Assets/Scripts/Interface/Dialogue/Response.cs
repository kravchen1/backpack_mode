

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Response
{
    public string responseText;
    public Dialogue nextDialogue; // ��������� ����� �������
    public int switchDialogID = 0;


    //����� �����
    public bool quest = false;
    public string questName = "";
    public string questDescription = "";
    public int necessaryProgress = 0;
    public int questID = 0;

    //������ ��������
    public bool giveItem = false;
    public List<GameObject> giveItemPrefab;

    //��������� �����
    public bool questComplete = false;
    public int idQuestComplete = 0;
}