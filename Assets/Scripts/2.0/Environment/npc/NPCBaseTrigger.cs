using TMPro;
using UnityEngine;

public class NPCBaseTrigger : EnvironmentTrigger
{
    [Header("Chest Settings")]
    public NPC NPCController;



    protected override void Start()
    {
        base.Start();
        NPCController = transform.parent.GetComponent<NPC>();
        settingsKey = "NPCBaseTrigget" + NPCController.Config.settingKey;//todo запись каждого объекте в Saver
    }

    protected override void PerformManualInteractionChild()
    {
        OpenMenuButtons();
        foreach (var buttonsKeyText in ButtonsKeyTexts)
        {
            GameObject button = Instantiate(ButtonPrefab, menuContent.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = buttonsKeyText;

            if (buttonsKeyText != null)
            {
                button.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();

                switch (buttonsKeyText)
                {
                    case "Attack":
                        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Attack());
                        break;
                    default:
                        //могут быть и другие ключи
                        break;
                }
            }
        }

    }


    private void Attack()
    {
        ExitTrigger();

        if (NPCController.npcGroups != null && NPCController.npcGroups.Count > 0)
        {
            foreach (NPC npc in NPCController.npcGroups)
            {
                npc.SetState(NPCStateType.Hostile);
                npc.currentState.OnPlayerDetected(NPCController, PlayerDataManager.Instance.playerCharacter.GetComponent<TopDownCharacterController>());
            }
        }
        else
        {
            NPCController.SetState(NPCStateType.Hostile);
            NPCController.currentState.OnPlayerDetected(NPCController, PlayerDataManager.Instance.playerCharacter.GetComponent<TopDownCharacterController>());
        }
    }


    protected override void OnExitChild()
    {
        CloseAllUI();
    }
}