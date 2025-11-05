using UnityEngine;

[CreateAssetMenu(fileName = "NPC Config", menuName = "Game/NPC Config")]
public class NPCConfig : ScriptableObject
{
    [Header("Base Settings")]
    public string NPCName = "Denis";
    public float moveSpeed = 5f;
    public float acceleration = 8f;
    public float angularSpeed = 360f;
    public float stoppingDistance = 0.1f;
    public string settingKey = "NPCTest";
    public NPCType type;
    public bool autoBraking = true;

    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float interactionRadius = 2f;

    [Header("State Settings")]
    public NPCStateType initialState = NPCStateType.Neutral;
    public int CharismaForHostile = 0;
    public int CharismaForFriendly = 10;
    public float attackRange = 5f;

    [Header("Hostile Settings")]
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

public enum NPCType
{
    BanditPowerClan_shooter,
    BanditPowerClan_warrior,
    BanditPowerClan_healer,
    BanditWhiteClan_shooter,
    BanditWhiteClan_warrior,
    BanditWhiteClan_healer,
    NeutralWarrior,
    NeutralShooter,
    NeutralHealer,
    Farmer,
    Seeker,
    Courier,
    Fisherman,
    Lumberjack,
    Miner,
    Trader,

    Wolf,
    Bear,
    Cow,
    Dog,
    Pig,
    Deer,
    Scorpion,
    Slug,
    Crab
}