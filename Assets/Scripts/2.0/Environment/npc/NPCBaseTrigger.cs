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
                    case "ViewInventory":
                        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ViewInventory());
                        break;
                    case "View":
                        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => View());
                        break;
                    case "Rob":
                        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Rob());
                        break;
                    case "Trade":
                        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Trade());
                        break;
                    default:
                        break;
                }
            }
        }

    }


    public void Attack()
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

    private void ViewInventory()
    {
        CloseMenuButtons();
        DragManager.Instance.isDragActive = false;
        buttonsController.OpenInventory();
        canvasShop.SetActive(true);
        shopData.settingsKey = NPCController.Config.settingKey;
        shopData.LoadData();
    }

    private void View()
    {

    }

    private void Rob()
    {
        CloseMenuButtons();
        //buttonsController.OpenInventory();
        //canvasShop.SetActive(true);
        //shopData.settingsKey = NPCController.Config.settingKey;
        //shopData.LoadData();
        RobManager.Instance.trigger = this;
        RobManager.Instance.StartRob(NPCController.Config.settingKey);
    }

    private void Trade()
    {

    }


    protected override void OnExitChild()
    {
        CloseAllUI();
    }
}