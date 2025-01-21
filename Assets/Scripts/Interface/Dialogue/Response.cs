

using UnityEngine;

[System.Serializable]
public class Response
{
    public string responseText;
    public Dialogue nextDialogue; // ��������� ����� �������
    public bool quest = false;
    public string questName = "";
    public string questDescription = "";
    public int necessaryProgress = 0;

    public bool giveItem = false;
    public GameObject giveItemPrefab;
}