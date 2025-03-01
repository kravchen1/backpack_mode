using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItem : MonoBehaviour
{
    public TextMeshPro textBody;
    public TextMeshPro Stats;

    public int damageMin = 1, damageMax = 2;
    public float staminaCost = 1;
    public int accuracyPercent = 95;
    public int chanceCrit = 5;
    public int critDamage = 130;
    public float cooldown = 1.1f;

    public int Armor = 0;


    private void Awake()
    {
    }

    void Update()
    {
        
    }
}
