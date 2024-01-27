using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "Data/FruitData", order = 1)]
public class CustomerData : ScriptableObject
{
    public enum CustomerType { Cat, Fish, Dog, Seal, Gecko, Hamster, NumCustomerType};

    public CustomerType type;

    LiquidQuantity[] quantities;
    public float glassesRevelTime = 2.0f;

    const float glassQuantMean = 1.25f;
    const float glassQuantSD = 1.25f;
    const float glassFlavourMean = 1.65f;
    const float glassFlavourSD = 0.75f;
    public int count { get 
        {
            int size = 0;
            for (int i = 0; i < quantities.Length; i++)
            {
                if (quantities[i] != null)
                    size++;
            }
            return size; 
        } }

    public static CustomerData GenerateCustomer( System.Random generator )
    {
        CustomerData data = new CustomerData();
        data.type = (CustomerType)Random.Range(0, (int)CustomerType.NumCustomerType );
        var quant = Mathf.Clamp((int)generator.RandomNormalDistribution(glassQuantMean, glassQuantSD), 1, 3);
        data.quantities = new LiquidQuantity[quant];
        for (int i = 0; i < quant; i++)
            data.quantities[i] = new LiquidQuantity(Mathf.Clamp((int)generator.RandomNormalDistribution(glassFlavourMean, glassFlavourSD), 1, 3));

        return data;
    }

    public LiquidQuantity GetLiquid(int i) { return quantities[i]; }

    public float RemoveBestCase(LiquidQuantity other)
    {
        float best_diff = 0.0f;
        int index = -1;
        for (int i = 0; i < quantities.Length; i++)
        {
            if (quantities[i] == null)
                continue;

            float diff = quantities[i].CompareTo(other);
            if (index == -1 || diff > best_diff)
            {
                index = i;
                best_diff = diff;
            }
        }

        quantities[index] = null;
        return best_diff;
    }
}
