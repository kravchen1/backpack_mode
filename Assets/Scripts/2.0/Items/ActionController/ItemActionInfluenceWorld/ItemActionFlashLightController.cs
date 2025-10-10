using UnityEngine;
using System.Collections;

public class ItemActionFlashLightController : ItemActionInfluenceWorldController
{
    
    //protected ArmorStats itemArmorStats;
    //public string animationKeyTakeDamage = "TakeDamage";

    protected override void Awake()
    {
        base.Awake();
        //itemArmorStats = GetComponent<ArmorStats>();
    }

    /// <summary>
    /// Применяет урон к предмету, уменьшая его прочность.
    /// Возвращает остаточный урон, превышающий прочность предмета.
    /// </summary>
    /// <param name="damage">Входящий урон</param>
    /// <returns>Остаток урона, не поглощенный прочностью предмета</returns>
    public override void InfluenceOnTheWorld()
    {
    }

}