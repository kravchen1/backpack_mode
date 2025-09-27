using UnityEngine;

[CreateAssetMenu(fileName = "NPC Config", menuName = "Game/NPC Config")]
public class NPCConfig : ScriptableObject
{
    [Header("Base Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 8f;
    public float angularSpeed = 360f;
    public float stoppingDistance = 0.1f;
    public bool autoBraking = true;

    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float interactionRadius = 2f;

    [Header("State Settings")]
    public NPCStateType initialState = NPCStateType.Neutral;

    [Header("Hostile Settings")]
    public float attackRange = 3f;
    public float chaseSpeed = 7f;
    public float chaseStoppingDistance = 1.5f;

    [Header("Patrol Settings")]
    public float patrolSpeed = 3f;
    public float waypointWaitTime = 2f;
    public float waypointReachedDistance = 0.5f;

    [Header("Visual Settings")]
    public Color hostileColor = Color.red;
    public Color neutralColor = Color.yellow;
    public Color friendlyColor = Color.green;
}