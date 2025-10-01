using System;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemQuality
{
    VeryBad,        // очень плохой -40%
    Bad,            // плохой -20%
    Normal,         // нормальный 0%
    Good,           // Хороший +20%
    Excellent       // Великолепный +40%
}

public class ItemQualityGenerator
{
    public static ItemQuality GetRandomQuality(float rarityBoost = 0f) //boost ot -4 do +4
    {
        // Базовые вероятности (в процентах)
        float[] baseProbabilities = { 40f, 30f, 20f, 7f, 3f };

        // Применяем повышение редкости
        float[] adjustedProbabilities = AdjustProbabilities(baseProbabilities, rarityBoost);

        // Генерируем случайное число от 0 до 100
        float randomValue = UnityEngine.Random.Range(0f, 100f);

        // Определяем качество на основе случайного числа
        float cumulativeProbability = 0f;

        for (int i = 0; i < adjustedProbabilities.Length; i++)
        {
            cumulativeProbability += adjustedProbabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return (ItemQuality)i;
            }
        }

        return ItemQuality.VeryBad;
    }

    private static float[] AdjustProbabilities(float[] baseProbabilities, float rarityBoost)
    {
        float[] adjusted = new float[baseProbabilities.Length];
        Array.Copy(baseProbabilities, adjusted, baseProbabilities.Length);

        // Ограничиваем boost диапазоном от -4 до 4
        rarityBoost = Math.Max(-4f, Math.Min(rarityBoost, 4f));

        if (rarityBoost != 0f)
        {
            if (rarityBoost > 0f)
            {
                // Положительный буст - увеличиваем шансы редких предметов
                for (int i = 0; i < 2; i++)
                {
                    adjusted[i] *= (1f - rarityBoost * 0.3f);
                }
                adjusted[2] *= (1f - rarityBoost * 0.15f);

                for (int i = 3; i < adjusted.Length; i++)
                {
                    adjusted[i] *= (1f + rarityBoost * 0.5f);
                }
            }
            else
            {
                // Отрицательный буст - увеличиваем шансы обычных предметов
                float negativeBoost = Math.Abs(rarityBoost);

                for (int i = 0; i < 2; i++)
                {
                    adjusted[i] *= (1f + negativeBoost * 0.5f);
                }
                adjusted[2] *= (1f + negativeBoost * 0.25f);

                for (int i = 3; i < adjusted.Length; i++)
                {
                    adjusted[i] *= (1f - negativeBoost * 0.7f);
                }
            }

            // Нормализуем вероятности чтобы сумма была 100%
            NormalizeProbabilities(adjusted);
        }

        return adjusted;
    }

    private static void NormalizeProbabilities(float[] probabilities)
    {
        float sum = 0f;
        foreach (float prob in probabilities)
        {
            sum += prob;
        }

        if (sum > 0f)
        {
            float multiplier = 100f / sum;
            for (int i = 0; i < probabilities.Length; i++)
            {
                probabilities[i] *= multiplier;
            }
        }
    }

    // Вспомогательный метод для тестирования распределения
    public static void TestDistribution(int sampleCount = 10000, float rarityBoost = 0f)
    {
        int[] counts = new int[Enum.GetValues(typeof(ItemQuality)).Length];

        for (int i = 0; i < sampleCount; i++)
        {
            ItemQuality quality = GetRandomQuality(rarityBoost);
            counts[(int)quality]++;
        }

        Debug.Log($"Тест распределения (boost: {rarityBoost}):");
        for (int i = 0; i < counts.Length; i++)
        {
            float percentage = (float)counts[i] / sampleCount * 100f;
            Debug.Log($"{(ItemQuality)i}: {counts[i]} ({percentage:F2}%)");
        }
        Debug.Log("");
    }


    //private void Start()
    //{
    //    ItemQualityGenerator.TestDistribution(10000, -2f);   // Без boost
    //    ItemQualityGenerator.TestDistribution(10000, -1f);   // Без boost
    //    ItemQualityGenerator.TestDistribution(10000, 0f);   // Без boost
    //    ItemQualityGenerator.TestDistribution(10000, 1.0f); // Со средним boost
    //    ItemQualityGenerator.TestDistribution(10000, 2.0f); // С максимальным boost
    //    ItemQualityGenerator.TestDistribution(10000, 3.0f); // С максимальным boost
    //    ItemQualityGenerator.TestDistribution(10000, 4.0f); // С максимальным boost
    //}

   
}