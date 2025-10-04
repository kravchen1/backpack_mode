using UnityEngine;
using System.Collections.Generic;

public class EnemyEncounterTrigger : MonoBehaviour
{
    [Header("Enemy Encounter Settings")]
    public List<NPCDataManager> enemiesInThisEncounter;
    public float detectionRange = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("FightCollision");
        if (other.CompareTag("Player"))
            StartBattleWithPlayer();
    }

    private void StartBattleWithPlayer()
    {
        BattleStarter battleStarter = FindObjectOfType<BattleStarter>();
        if (battleStarter != null)
            battleStarter.StartBattleOnCollision(enemiesInThisEncounter);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}