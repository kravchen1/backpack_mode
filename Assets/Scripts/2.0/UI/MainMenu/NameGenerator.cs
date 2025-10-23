using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameGenerator : MonoBehaviour
{
    public enum Language
    {
        Russian,
        English,
        ChineseSimplified,
        ChineseTraditional
    }

    [SerializeField] private Language selectedLanguage = Language.Russian;
    [SerializeField] private TMPro.TMP_InputField targetInputField;

    private Dictionary<Language, List<string>> namesDatabase;

    private void Awake()
    {
        InitializeNamesDatabase();

        // Если InputField не назначен в инспекторе, попробуем найти его на этом же объекте
        if (targetInputField == null)
            targetInputField = GetComponent<TMP_InputField>();

        RandomName();
    }

    private void InitializeNamesDatabase()
    {
        namesDatabase = new Dictionary<Language, List<string>>();

        // Русские имена
        namesDatabase[Language.Russian] = new List<string>
        {
            "Александр", "Алексей", "Анатолий", "Андрей", "Антон",
            "Аркадий", "Арсений", "Артём", "Борис", "Вадим",
            "Валентин", "Валерий", "Василий", "Виктор", "Виталий",
            "Владимир", "Владислав", "Вячеслав", "Геннадий", "Георгий",
            "Григорий", "Даниил", "Денис", "Дмитрий", "Евгений",
            "Егор", "Иван", "Игорь", "Илья", "Кирилл",
            "Константин", "Лев", "Леонид", "Максим", "Марк",
            "Матвей", "Михаил", "Никита", "Николай", "Олег",
            "Павел", "Пётр", "Роман", "Сергей", "Станислав",
            "Степан", "Тимофей", "Фёдор", "Юрий", "Ярослав"
        };

        // Английские имена
        namesDatabase[Language.English] = new List<string>
        {
            "James", "John", "Robert", "Michael", "William",
            "David", "Richard", "Charles", "Joseph", "Thomas",
            "Christopher", "Daniel", "Paul", "Mark", "Donald",
            "George", "Kenneth", "Steven", "Edward", "Brian",
            "Ronald", "Anthony", "Kevin", "Jason", "Matthew",
            "Gary", "Timothy", "Jose", "Larry", "Jeffrey",
            "Frank", "Scott", "Eric", "Stephen", "Andrew",
            "Raymond", "Gregory", "Joshua", "Jerry", "Dennis",
            "Walter", "Patrick", "Peter", "Harold", "Douglas",
            "Henry", "Carl", "Arthur", "Ryan", "Roger"
        };

        // Китайские упрощенные имена
        namesDatabase[Language.ChineseSimplified] = new List<string>
        {
            "张伟", "王伟", "王芳", "李伟", "李娜",
            "张敏", "李静", "王静", "刘伟", "张静",
            "王秀英", "李秀英", "王丽", "张丽", "李强",
            "张秀英", "李敏", "王敏", "王磊", "李军",
            "刘洋", "王强", "张磊", "李杰", "王军",
            "张杰", "王艳", "李艳", "张强", "王刚",
            "李刚", "刘敏", "张艳", "王杰", "李鹏",
            "刘杰", "王超", "李超", "张军", "王勇",
            "李勇", "张超", "王浩", "李浩", "刘强",
            "张勇", "王鑫", "李鑫", "刘磊", "张浩"
        };

        // Китайские традиционные имена
        namesDatabase[Language.ChineseTraditional] = new List<string>
        {
            "張偉", "王偉", "王芳", "李偉", "李娜",
            "張敏", "李靜", "王靜", "劉偉", "張靜",
            "王秀英", "李秀英", "王麗", "張麗", "李強",
            "張秀英", "李敏", "王敏", "王磊", "李軍",
            "劉洋", "王強", "張磊", "李傑", "王軍",
            "張傑", "王艷", "李艷", "張強", "王剛",
            "李剛", "劉敏", "張艷", "王傑", "李鵬",
            "劉傑", "王超", "李超", "張軍", "王勇",
            "李勇", "張超", "王浩", "李浩", "劉強",
            "張勇", "王鑫", "李鑫", "劉磊", "張浩"
        };
    }

    /// <summary>
    /// Генерирует случайное имя в соответствии с выбранным языком
    /// </summary>
    public void RandomName()
    {
        if (targetInputField == null)
        {
            Debug.LogWarning("Target InputField is not assigned!");
            return;
        }

        if (namesDatabase.ContainsKey(selectedLanguage) && namesDatabase[selectedLanguage].Count > 0)
        {
            List<string> names = namesDatabase[selectedLanguage];
            string randomName = names[Random.Range(0, names.Count)];
            targetInputField.text = randomName;
        }
        else
        {
            Debug.LogWarning($"No names available for language: {selectedLanguage}");
        }
    }

    /// <summary>
    /// Устанавливает язык для генерации имен
    /// </summary>
    public void SetLanguage(Language language)
    {
        selectedLanguage = language;
    }

    /// <summary>
    /// Генерирует случайное имя для указанного языка
    /// </summary>
    public void RandomNameForLanguage(Language language)
    {
        SetLanguage(language);
        RandomName();
    }

    // Метод для вызова из UI (например, из кнопки)
    public void OnRandomNameButtonClicked()
    {
        RandomName();
    }
}