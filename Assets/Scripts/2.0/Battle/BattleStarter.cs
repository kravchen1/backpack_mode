using UnityEngine;
using System.Collections.Generic;

public class BattleStarter : MonoBehaviour
{


    [Header("Player Team")]
    public List<NPCDataManager> playerTeamCharacters;

    [Header("Enemy Team")]
    public List<NPCDataManager> enemyTeamCharacters;

    [Header("Test Settings")]
    public KeyCode startBattleKey = KeyCode.B;
    public KeyCode escapeKey = KeyCode.F;
    public KeyCode addEnemyKey = KeyCode.E;
    public KeyCode addFriendKey = KeyCode.G;

    private void Update()
    {
        if (Input.GetKeyDown(startBattleKey))
            StartTestBattle();

        if (Input.GetKeyDown(escapeKey) && BattleManager.Instance != null && BattleManager.Instance.CanEscape())
            BattleManager.Instance.AttemptEscape();

        if (Input.GetKeyDown(addEnemyKey) && BattleManager.Instance != null && enemyTeamCharacters.Count > 0)
        {
            var newEnemy = Instantiate(enemyTeamCharacters[0]);
            newEnemy.CharacterName = "Новый враг";
            BattleManager.Instance.AddEnemyToBattle(newEnemy);
        }

        // ИСПРАВЛЕНО: теперь метод TryJoinFriend public и доступен
        if (Input.GetKeyDown(addFriendKey) && FriendSystem.Instance != null)
            FriendSystem.Instance.TryJoinFriend();
    }

    [ContextMenu("Start Test Battle")]
    public void StartTestBattle()
    {
        if (BattleManager.Instance != null)
        {
            //foreach (var player in playerCharacters)
            //    player.RestoreFullHealth();
            //foreach (var enemy in enemyCharacters)
            //    enemy.RestoreFullHealth();

            BattleManager.Instance.StartBattle(playerTeamCharacters, enemyTeamCharacters);
        }
    }

    public void StartBattleOnCollision(List<NPCDataManager> enemies)
    {
        if (BattleManager.Instance != null)
        {
            //foreach (var player in playerCharacters)
            //    player.RestoreFullHealth();

            BattleManager.Instance.StartBattle(playerTeamCharacters, enemies);
        }
    }
}