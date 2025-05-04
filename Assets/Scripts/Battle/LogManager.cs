using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    [System.Serializable]
    public class LogPrefab
    {
        public string tag;
        public GameObject playerPrefab;
        public GameObject enemyPrefab;
    }

    [Header("Settings")]
    public List<LogPrefab> logPrefabs = new List<LogPrefab>();
    public ScrollRect scrollRect;
    public int initialPoolSize = 30;

    [Header("Performance")]
    public bool enablePooling = true;
    public bool autoClearOldLogs = false;
    public int maxLogsBeforeClear = 200;
    public int logsToKeepOnClear = 50;

    private GameObject placeForLogDescription;
    private TimeSpeed ts;
    private GridLayoutGroup gridLayoutGroup;
    private ContentSizeFitter contentSizeFitter;
    private string settingLanguage = "en";

    private Dictionary<string, Queue<GameObject>> playerPools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, Queue<GameObject>> enemyPools = new Dictionary<string, Queue<GameObject>>();
    private List<GameObject> activeLogs = new List<GameObject>();

    private void Start()
    {
        placeForLogDescription = GameObject.FindGameObjectWithTag("BattleLogContent");
        gridLayoutGroup = placeForLogDescription.GetComponent<GridLayoutGroup>();
        contentSizeFitter = placeForLogDescription.GetComponent<ContentSizeFitter>();
        ts = GameObject.FindGameObjectWithTag("SliderTime").GetComponent<TimeSpeed>();
        settingLanguage = PlayerPrefs.GetString("LanguageSettings", "en");

        InitializeAllPools();
    }

    private void InitializeAllPools()
    {
        foreach (var prefab in logPrefabs)
        {
            if (!playerPools.ContainsKey(prefab.tag))
            {
                playerPools[prefab.tag] = new Queue<GameObject>();
                enemyPools[prefab.tag] = new Queue<GameObject>();

                for (int i = 0; i < initialPoolSize; i++)
                {
                    CreatePooledObject(prefab.tag, true);
                    CreatePooledObject(prefab.tag, false);
                }
            }
        }
    }

    private GameObject CreatePooledObject(string tag, bool isPlayer)
    {
        var logPrefab = logPrefabs.Find(x => x.tag == tag) ?? logPrefabs[0];
        GameObject prefab = isPlayer ? logPrefab.playerPrefab : logPrefab.enemyPrefab;
        GameObject obj = Instantiate(prefab, placeForLogDescription.transform);
        obj.SetActive(false);

        var pool = isPlayer ? playerPools[tag] : enemyPools[tag];
        pool.Enqueue(obj);

        return obj;
    }

    private GameObject GetLogFromPool(string tag, bool isPlayer)
    {
        if (!enablePooling)
        {
            return InstantiateLog(tag, isPlayer);
        }

        var pool = isPlayer ? playerPools : enemyPools;

        if (!pool.ContainsKey(tag))
        {
            Debug.LogWarning($"Log type {tag} not found in pools, using default");
            tag = logPrefabs[0].tag;
        }

        if (pool[tag].Count == 0)
        {
            CreatePooledObject(tag, isPlayer);
        }

        GameObject logObj = pool[tag].Dequeue();
        logObj.SetActive(true);
        return logObj;
    }

    private void ReturnLogToPool(GameObject logObj, string tag, bool isPlayer)
    {
        if (!enablePooling)
        {
            Destroy(logObj);
            return;
        }

        logObj.SetActive(false);
        var pool = isPlayer ? playerPools : enemyPools;

        if (pool.ContainsKey(tag))
        {
            pool[tag].Enqueue(logObj);
        }
        else
        {
            pool[logPrefabs[0].tag].Enqueue(logObj);
        }
    }

    private GameObject InstantiateLog(string tag, bool isPlayer)
    {
        var logPrefab = logPrefabs.Find(x => x.tag == tag) ?? logPrefabs[0];
        GameObject prefab = isPlayer ? logPrefab.playerPrefab : logPrefab.enemyPrefab;
        return Instantiate(prefab, placeForLogDescription.transform);
    }

    private void ManageLogCount()
    {
        if (!autoClearOldLogs || activeLogs.Count < maxLogsBeforeClear)
            return;

        int logsToRemove = activeLogs.Count - logsToKeepOnClear;
        for (int i = 0; i < logsToRemove; i++)
        {
            GameObject log = activeLogs[0];
            activeLogs.RemoveAt(0);

            bool found = false;
            foreach (var prefab in logPrefabs)
            {
                if (log.name.StartsWith(prefab.playerPrefab.name))
                {
                    ReturnLogToPool(log, prefab.tag, true);
                    found = true;
                    break;
                }
                else if (log.name.StartsWith(prefab.enemyPrefab.name))
                {
                    ReturnLogToPool(log, prefab.tag, false);
                    found = true;
                    break;
                }
            }

            if (!found) ReturnLogToPool(log, logPrefabs[0].tag, true);
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    //private void SaveScroll()
    //{
    //    if (scrollRect != null)
    //    {
    //        scrollPosition = scrollRect.normalizedPosition;
    //    }
    //    gridLayoutGroup.enabled = false;
    //    if (contentSizeFitter != null)
    //        contentSizeFitter.enabled = false;
    //}

    //private void LoadScroll()
    //{
    //    gridLayoutGroup.enabled = true;
    //    if (contentSizeFitter != null)
    //        contentSizeFitter.enabled = true;

    //    LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.GetComponent<RectTransform>());
    //    Canvas.ForceUpdateCanvases();

    //    if (scrollRect != null)
    //    {
    //        scrollRect.normalizedPosition = scrollPosition;
    //    }
    //}

    private void CreateLogMessageInternal(string text, string tag, bool isPlayer)
    {
        if (autoClearOldLogs) ManageLogCount();

        string textTime = ts.nowTime.ToString("0.0") + "s";

        //SaveScroll();
        GameObject logObj = GetLogFromPool(tag, isPlayer);
        logObj.transform.SetAsLastSibling();
        logObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        logObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
        activeLogs.Add(logObj);
        //LoadScroll();
    }

    public void ClearAllLogs()
    {
        foreach (var log in activeLogs)
        {
            bool found = false;
            foreach (var prefab in logPrefabs)
            {
                if (log.name.StartsWith(prefab.playerPrefab.name))
                {
                    ReturnLogToPool(log, prefab.tag, true);
                    found = true;
                    break;
                }
                else if (log.name.StartsWith(prefab.enemyPrefab.name))
                {
                    ReturnLogToPool(log, prefab.tag, false);
                    found = true;
                    break;
                }
            }

            if (!found) ReturnLogToPool(log, logPrefabs[0].tag, true);
        }

        activeLogs.Clear();
        //LoadScroll();
    }

    // ========== Оригинальные методы создания логов ==========

    public void CreateLogMessageAttackOnArmor(string nameItem, int damage, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Destroy")} {damage} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Armor")}.";
        CreateLogMessageInternal(text, "attack", Player);
    }

    public void CreateLogMessageAttackOnHalfArmor(string nameItem, int damage, int armor, bool Player)
    {
        string text1 = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Destroy")} {armor} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Armor")}.";
        CreateLogMessageInternal(text1, "attack", Player);

        string text2 = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Deal")} {damage} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Damage")}.";
        CreateLogMessageInternal(text2, "attack", Player);
    }

    public void CreateLogMessageAttackWithoutArmor(string nameItem, int damage, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Deal")} {damage} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Damage")}.";
        CreateLogMessageInternal(text, "attack", Player);
    }

    public void CreateLogMessageMiss(string nameItem, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Miss")}.";
        CreateLogMessageInternal(text, "stamina", Player);
    }

    public void CreateLogMessageNoHaveStamina(string nameItem, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "NoHaveStamina")}.";
        CreateLogMessageInternal(text, "attack", Player);
    }

    public void CreateLogMessageGive(string nameItem, string tagIcon, int count, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Give")} {count}.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }

    public void CreateLogMessageGive(string nameItem, string tagIcon, float count, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Give")} {count:0.0}.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }

    public void CreateLogMessageInflict(string nameItem, string tagIcon, int count, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Inflict")} {count}.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }

    public void CreateLogMessageUse(string nameItem, string tagIcon, int count, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Used")} {count}.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }

    public void CreateLogMessageUseFromEnemy(string nameItem, string tagIcon, int count, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Used")} {count} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "FromEnemy")}.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }

    public void CreateLogMessageSteal(string nameItem, string tagIcon, int count, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Steal")} {count}.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }

    public void CreateLogMessageBlock(string nameItem, string tagIcon, int count, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Block")} {count}.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }

    public void CreateLogMessageDecreaseStamina(string nameItem, string tagIcon, float count, string decreaseItem, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "DecreaseStamina")} {decreaseItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "On")} {count:0.0}.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }

    public void CreateLogMessageRemove(string nameItem, string tagIcon, int count, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Remove")} {count}.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }

    public void CreateLogMessageReduced(string nameItem, string tagIcon, double count, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "CooldownReducded")} {count:0.0}%.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }

    public void CreateLogMessageReducedForItem(string nameItem, string tagIcon, double count, string reducedItem, bool Player)
    {
        string text = $"{nameItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "CooldownReducdedFor")} {reducedItem} {LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "By")} {count:0.0}%.";
        CreateLogMessageInternal(text, tagIcon, Player);
    }
}