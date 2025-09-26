// PlayerAttributes.cs
[System.Serializable]
public class PlayerAttributes
{
    public Stat endurance = new Stat(); // ������������
    public Stat strength = new Stat(); // ����
    public Stat agility = new Stat(); // ��������
    public Stat intellect = new Stat(); // ���������
    public Stat charisma = new Stat(); // �������
    public Stat luck = new Stat(); // �����

    // ����� �������� �������� ��� ��������
    public int Strength => strength.GetValue();
    public int Agility => agility.GetValue();
    public int Intellect => intellect.GetValue();
    public int Charisma => charisma.GetValue();
    public int Luck => luck.GetValue();
    public int Endurance => endurance.GetValue();
}