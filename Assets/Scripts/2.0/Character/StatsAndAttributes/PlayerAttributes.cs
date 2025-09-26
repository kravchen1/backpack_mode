// PlayerAttributes.cs
[System.Serializable]
public class PlayerAttributes
{
    public Stat endurance = new Stat(); // Выносливость
    public Stat strength = new Stat(); // Сила
    public Stat agility = new Stat(); // Ловкость
    public Stat intellect = new Stat(); // Интеллект
    public Stat charisma = new Stat(); // Харизма
    public Stat luck = new Stat(); // Удача

    // Можно добавить свойства для удобства
    public int Strength => strength.GetValue();
    public int Agility => agility.GetValue();
    public int Intellect => intellect.GetValue();
    public int Charisma => charisma.GetValue();
    public int Luck => luck.GetValue();
    public int Endurance => endurance.GetValue();
}