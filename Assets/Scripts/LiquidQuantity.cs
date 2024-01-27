using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidQuantity 
{
    public enum LiquidType : int { Apple, Orange, Mango, Lime, Grapes, NumOfLiquid};

    private float[] quantity = new float[(int)LiquidType.NumOfLiquid];
    private float current_total
    {
        get
        {
            float value = 0.0f;
            for (int i = 0; i < (int)LiquidType.NumOfLiquid; i++)
            {
                value += quantity[i];
            }
            return value; 
        } 
    }

    public float GetQuantity(LiquidType type)
    {
        return quantity[(int)type];
    }

    public void AddQuantity(LiquidType type, float value)
    {
        if (current_total < 1.0f)
            quantity[(int)type] += Mathf.Min(value, 1.0f - current_total);
    }

    public float CompareTo(LiquidQuantity other)
    {
        float difference = 1.0f;
        for (int i = 0; i < (int)LiquidType.NumOfLiquid; i++)
        {
            difference -= Mathf.Abs(quantity[i] - other.quantity[i]);
        }
        return Mathf.Max(difference, 0.0f);
    }

    public float[] GetUIValues()
    {
        float[] value = new float[(int)LiquidType.NumOfLiquid];

        for (int i = (int)LiquidType.NumOfLiquid - 1; i >= 0; i--)
        {
            if (quantity[i] <= 0.0f)
            {
                for (int j = i; j < (int)LiquidType.NumOfLiquid; j++)
                    value[i] += quantity[j];
            }
            else
                value[i] = 0.0f;
        }

        return value;
    }
}
