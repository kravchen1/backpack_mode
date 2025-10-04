using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class FriendSystem : MonoBehaviour
{
    public static FriendSystem Instance;

    [Header("Friend Settings")]
    public int maxFriendsInBattle = 2;
    public float friendJoinChance = 0.3f;
    public float joinCheckInterval = 5f;

    [Header("UI References")]
    public GameObject friendJoinNotification;
    public TextMeshProUGUI friendJoinText;

    [Header("Friend Prefabs")]
    public List<GameObject> friendPrefabs;

    private List<NPCDataManager> availableFriends = new List<NPCDataManager>();
    private List<NPCDataManager> activeFriends = new List<NPCDataManager>();
    private Coroutine friendJoinCoroutine;

    const int maxPlayerTeamSize = 4;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeFriends();
    }

    private void InitializeFriends()
    {
        foreach (var friendPrefab in friendPrefabs)
        {
            GameObject friendObj = Instantiate(friendPrefab);
            NPCDataManager friend = friendObj.GetComponent<NPCDataManager>();

            if (friend != null)
            {
                friend.gameObject.name = friendPrefab.name;
                friendObj.SetActive(false);
                availableFriends.Add(friend);
            }
        }
    }

    public void StartFriendJoinProcess()
    {
        if (friendJoinCoroutine != null)
            StopCoroutine(friendJoinCoroutine);

        friendJoinCoroutine = StartCoroutine(FriendJoinRoutine());
    }

    private IEnumerator FriendJoinRoutine()
    {
        while (BattleManager.Instance != null && BattleManager.Instance.GetPlayerTeam().Count > 0)
        {
            yield return new WaitForSeconds(joinCheckInterval);

            if (CanFriendJoin() && Random.value <= friendJoinChance)
                TryJoinFriend();
        }
    }

    private bool CanFriendJoin()
    {
        int totalAllies = BattleManager.Instance.GetPlayerTeam().Count + activeFriends.Count;
        return activeFriends.Count < maxFriendsInBattle &&
               totalAllies < maxPlayerTeamSize &&
               availableFriends.Count > 0;
    }

    // ИСПРАВЛЕНО: метод теперь public
    public void TryJoinFriend()
    {
        if (availableFriends.Count == 0) return;

        var joinCandidate = availableFriends[Random.Range(0, availableFriends.Count)];
        if (joinCandidate != null)
            JoinFriend(joinCandidate);
    }

    private void JoinFriend(NPCDataManager friend)
    {
        friend.gameObject.SetActive(true);
        //friend.RestoreFullHealth();

        availableFriends.Remove(friend);
        activeFriends.Add(friend);

        BattleManager.Instance.AddFriendToBattle(friend);
        ShowJoinNotification(friend.CharacterName);

        OnFriendJoined?.Invoke(friend);
    }

    private void ShowJoinNotification(string friendName)
    {
        if (friendJoinNotification != null && friendJoinText != null)
        {
            friendJoinText.text = $"{friendName} присоединился к бою!";
            friendJoinNotification.SetActive(true);
            StartCoroutine(HideNotificationAfterDelay(3f));
        }
    }

    private IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (friendJoinNotification != null)
            friendJoinNotification.SetActive(false);
    }

    public void FriendLeave(NPCDataManager friend)
    {
        if (activeFriends.Contains(friend))
        {
            activeFriends.Remove(friend);
            availableFriends.Add(friend);
            friend.gameObject.SetActive(false);
        }
    }

    public void StopFriendSystem()
    {
        if (friendJoinCoroutine != null)
        {
            StopCoroutine(friendJoinCoroutine);
            friendJoinCoroutine = null;
        }

        foreach (var friend in activeFriends)
        {
            friend.gameObject.SetActive(false);
            availableFriends.Add(friend);
        }
        activeFriends.Clear();
    }

    public List<NPCDataManager> GetActiveFriends() => new List<NPCDataManager>(activeFriends);
    public List<NPCDataManager> GetAvailableFriends() => new List<NPCDataManager>(availableFriends);
    public int GetActiveFriendsCount() => activeFriends.Count;

    public System.Action<NPCDataManager> OnFriendJoined;
}