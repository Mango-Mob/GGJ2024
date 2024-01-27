using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum LiquidType : int { Apple, Orange, Mango, Lime, Grapes, NumOfLiquid};

public class LiquidQuantity 
{

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
    public bool isEmpty { get { return current_total <= 0.0f; } }
    public LiquidQuantity() {}
    public float At(int i) { return quantity[i]; }

    public LiquidQuantity( int number_of_flaviours )
    {
        switch (number_of_flaviours)
        {
            default:
            case 1:
                AddQuantity((LiquidType)Random.Range(0, (int)LiquidType.NumOfLiquid), Random.Range(0, 2) == 0 ? 1.0f : 0.5f);
                break;
            case 2:
                {
                    List<LiquidType> options = new List<LiquidType> { LiquidType.Apple, LiquidType.Orange, LiquidType.Mango, LiquidType.Lime, LiquidType.Grapes };
                    var select = Random.Range(0, options.Count);
                    var pick_one = options[select];
                    options.RemoveAt(select);
                    select = Random.Range(0, options.Count);
                    var pick_two = options[select];
                    switch (Random.Range(0, 2))
                    {
                        default:
                        case 0: //50/50
                            AddQuantity(pick_one, 0.5f);
                            AddQuantity(pick_two, 0.5f);
                            break;
                        case 1: //80/20
                            AddQuantity(pick_one, 0.8f);
                            AddQuantity(pick_two, 0.2f);
                            break;
                    }
                }
                break;
            case 3:
                {
                    List<LiquidType> options = new List<LiquidType> { LiquidType.Apple, LiquidType.Orange, LiquidType.Mango, LiquidType.Lime, LiquidType.Grapes };
                    var select = Random.Range(0, options.Count);
                    var pick_one = options[select];
                    options.RemoveAt(select);
                    select = Random.Range(0, options.Count);
                    var pick_two = options[select];
                    options.RemoveAt(select);
                    select = Random.Range(0, options.Count);
                    var pick_three = options[select];
                    switch (Random.Range(0, 2))
                    {
                        default:
                        case 0: //30/30/30
                            AddQuantity(pick_one, 0.3f);
                            AddQuantity(pick_two, 0.3f);
                            AddQuantity(pick_three, 0.3f);
                            break;
                        case 1: //60/20/20
                            AddQuantity(pick_one, 0.6f);
                            AddQuantity(pick_two, 0.2f);
                            AddQuantity(pick_three, 0.2f);
                            break;
                    }
                }
                break;
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

        float total = 0.0f;
        for (int i = (int)LiquidType.NumOfLiquid - 1; i >= 0; i--) //0.2
        {
            value[i] = total + quantity[i];
            total += quantity[i];
        }

        return value;
    }
}
