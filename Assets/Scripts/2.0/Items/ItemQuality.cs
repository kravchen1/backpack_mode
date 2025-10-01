using System;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemQuality
{
    VeryBad,        // ����� ������ -40%
    Bad,            // ������ -20%
    Normal,         // ���������� 0%
    Good,           // ������� +20%
    Excellent       // ������������ +40%
}

public class ItemQualityGenerator
{
    public static ItemQuality GetRandomQuality(float rarityBoost = 0f) //boost ot -4 do +4
    {
        // ������� ����������� (� ���������)
        float[] baseProbabilities = { 40f, 30f, 20f, 7f, 3f };

        // ��������� ��������� ��������
        float[] adjustedProbabilities = AdjustProbabilities(baseProbabilities, rarityBoost);

        // ���������� ��������� ����� �� 0 �� 100
        float randomValue = UnityEngine.Random.Range(0f, 100f);

        // ���������� �������� �� ������ ���������� �����
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

        // ������������ boost ���������� �� -4 �� 4
        rarityBoost = Math.Max(-4f, Math.Min(rarityBoost, 4f));

        if (rarityBoost != 0f)
        {
            if (rarityBoost > 0f)
            {
                // ������������� ���� - ����������� ����� ������ ���������
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
                // ������������� ���� - ����������� ����� ������� ���������
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

            // ����������� ����������� ����� ����� ���� 100%
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

    // ��������������� ����� ��� ������������ �������������
    public static void TestDistribution(int sampleCount = 10000, float rarityBoost = 0f)
    {
        int[] counts = new int[Enum.GetValues(typeof(ItemQuality)).Length];

        for (int i = 0; i < sampleCount; i++)
        {
            ItemQuality quality = GetRandomQuality(rarityBoost);
            counts[(int)quality]++;
        }

        Debug.Log($"���� ������������� (boost: {rarityBoost}):");
        for (int i = 0; i < counts.Length; i++)
        {
            float percentage = (float)counts[i] / sampleCount * 100f;
            Debug.Log($"{(ItemQuality)i}: {counts[i]} ({percentage:F2}%)");
        }
        Debug.Log("");
    }


    //private void Start()
    //{
    //    ItemQualityGenerator.TestDistribution(10000, -2f);   // ��� boost
    //    ItemQualityGenerator.TestDistribution(10000, -1f);   // ��� boost
    //    ItemQualityGenerator.TestDistribution(10000, 0f);   // ��� boost
    //    ItemQualityGenerator.TestDistribution(10000, 1.0f); // �� ������� boost
    //    ItemQualityGenerator.TestDistribution(10000, 2.0f); // � ������������ boost
    //    ItemQualityGenerator.TestDistribution(10000, 3.0f); // � ������������ boost
    //    ItemQualityGenerator.TestDistribution(10000, 4.0f); // � ������������ boost
    //}

   
}