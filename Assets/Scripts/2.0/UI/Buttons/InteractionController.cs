using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class InteractionController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private float interactionCheckRate = 0.1f;

    [Header("UI References")]
    [SerializeField] private Text interactionPromptText;
    [SerializeField] private GameObject interactionPromptPanel;

    private HashSet<EnvironmentTrigger> availableInteractions = new HashSet<EnvironmentTrigger>();
    private EnvironmentTrigger closestInteraction;
    private float lastCheckTime;

    private void Awake()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (interactionPromptPanel != null)
        {
            interactionPromptPanel.SetActive(false);
        }
    }

    private void Update()
    {
        HandleInteractionInput();

        if (Time.time - lastCheckTime >= interactionCheckRate)
        {
            UpdateInteractions();
            lastCheckTime = Time.time;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            TryPerformInteraction();
        }
    }

    private void UpdateInteractions()
    {
        // Очищаем только уничтоженные объекты
        availableInteractions.RemoveWhere(x => x == null);

        if (availableInteractions.Count == 0)
        {
            SetClosestInteraction(null);
            return;
        }

        // Находим ближайший триггер с игроком внутри
        EnvironmentTrigger newClosest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        foreach (var interaction in availableInteractions)
        {
            if (interaction == null) continue;

            // ВСЕГДА проверяем находится ли игрок в триггере в реальном времени
            if (!interaction.IsPlayerInTrigger())
                continue;

            float distance = Vector3.Distance(playerPosition, interaction.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                newClosest = interaction;
            }
        }

        SetClosestInteraction(newClosest);
    }

    private void SetClosestInteraction(EnvironmentTrigger interaction)
    {
        if (closestInteraction == interaction) return;

        closestInteraction = interaction;
        UpdateInteractionUI();
    }

    private void UpdateInteractionUI()
    {
        if (closestInteraction != null && closestInteraction.IsPlayerInTrigger())
        {
            ShowInteractionPrompt(closestInteraction);
        }
        else
        {
            HideInteractionPrompt();
            closestInteraction = null;
        }
    }

    private void ShowInteractionPrompt(EnvironmentTrigger interaction)
    {
        if (interaction == null) return;

        try
        {
            if (interactionPromptText != null)
            {
                interactionPromptText.text = interaction.GetInteractionPrompt();
            }

            if (interactionPromptPanel != null)
            {
                interactionPromptPanel.SetActive(true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to show interaction prompt: {e.Message}");
            HideInteractionPrompt();
        }
    }

    private void HideInteractionPrompt()
    {
        try
        {
            if (interactionPromptPanel != null)
            {
                interactionPromptPanel.SetActive(false);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to hide interaction prompt: {e.Message}");
        }
    }

    private void TryPerformInteraction()
    {
        if (closestInteraction == null || !closestInteraction.IsPlayerInTrigger())
        {
            Debug.Log("No valid interaction available");
            return;
        }

        try
        {
            closestInteraction.PerformManualInteraction();
            Debug.Log($"Successfully interacted with: {closestInteraction.name}");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Interaction failed: {ex.Message}");
        }
    }

    public void RegisterInteraction(EnvironmentTrigger interaction)
    {
        if (interaction != null)
        {
            availableInteractions.Add(interaction);
            Debug.Log($"Registered interaction: {interaction.name}");
        }
    }

    public void UnregisterInteraction(EnvironmentTrigger interaction)
    {
        if (interaction != null)
        {
            availableInteractions.Remove(interaction);
            Debug.Log($"Unregistered interaction: {interaction.name}");

            if (closestInteraction == interaction)
            {
                closestInteraction = null;
                UpdateInteractions();
            }
        }
    }

    public bool HasAvailableInteractions()
    {
        availableInteractions.RemoveWhere(x => x == null);
        return availableInteractions.Any(x => x.IsPlayerInTrigger());
    }

    // Для дебаггинга
    private void OnGUI()
    {
        int activeCount = availableInteractions.Count(x => x != null && x.IsPlayerInTrigger());
        GUI.Label(new Rect(10, 100, 300, 20), $"Total interactions: {availableInteractions.Count}");
        GUI.Label(new Rect(10, 120, 300, 20), $"Active interactions: {activeCount}");
        GUI.Label(new Rect(10, 140, 300, 20), $"Closest: {(closestInteraction != null ? closestInteraction.name : "None")}");
    }
}