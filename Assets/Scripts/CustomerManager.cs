using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    public Animator CharacterArt;
    public Slider CharacterPatience;
    public GameObject[] Glasses;

    public void UpdateCharacter( CustomerData data)
    {
        for (int i = 0; i < 3; i++)
        {
            if(i < data.count)
            {
                Glasses[i].SetActive(true);
                Glasses[i].GetComponent<LiquidProgressControllerUI>().SetValues(data.GetLiquidUI(i));
            }
            else
                Glasses[i].SetActive(false);
        }
        UpdatePatience(1.0f);
    }

    public void UpdatePatience( float value )
    {
        CharacterPatience.value = value;
    }

    public void RemoveCustomer(bool happy)
    {

    }
}
