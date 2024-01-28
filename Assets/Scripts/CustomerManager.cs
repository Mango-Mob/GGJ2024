using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    public Animator CharacterArt;
    public Slider CharacterPatience;
    public GameObject GlassesParent;
    public GameObject[] Glasses;
    public GameObject SellUI;

    private MultiAudioAgent audio;

    bool has_character = false;
    public void Awake()
    {
        audio = GetComponent<MultiAudioAgent>();
        CharacterPatience.gameObject.SetActive(false);
        GlassesParent.SetActive(false);
    }
    public void UpdateCharacter( CustomerData data)
    {
        CharacterPatience.gameObject.SetActive(false);
        GlassesParent.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            if(i < data.count)
            {
                Glasses[i].SetActive(true);
                Glasses[i].GetComponent<LiquidProgressControllerUI>().SetValues(data.GetLiquid(i));
            }
            else
                Glasses[i].SetActive(false);
        }
        
        CharacterArt.SetInteger("Index", (int)data.type);
        audio.Play("CustomerEnter");
        UpdatePatience(1.0f);
    }

    public void UpdatePatience( float value )
    {
        CharacterPatience.value = value;
    }

    public void RemoveCustomer(bool happy)
    {
        CharacterArt.SetInteger("Index", -1);
        CharacterPatience.gameObject.SetActive(false);
        for (int i = 0; i < 3; i++)
            Glasses[i].SetActive(false);
        has_character = false;
        audio.Play("CustomerLeave");
        if(happy)
            audio.Play("happy customer");
    }

    public void Show()
    {
        GlassesParent.SetActive(true);
        CharacterPatience.gameObject.SetActive(true);
        has_character = true;
    }

    public void Update()
    {
        SellUI.SetActive(has_character && GameManager.Instance.canSell);
    }
}
