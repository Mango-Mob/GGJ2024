using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "Data/FruitData", order = 1)]
public class CustomerData : ScriptableObject
{
    public enum CustomerType { Cat, Fish, Gecko, Hamster, Seal, NumCustomerType};

    public CustomerType type;

    LiquidQuantity[] quantities;

    const float glassQuantMean = 1.25f;
    const float glassQuantSD = 1.25f;
    const float glassFlavourMean = 1.65f;
    const float glassFlavourSD = 0.75f;

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
}
