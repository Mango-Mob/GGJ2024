using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidProgressControllerUI : MonoBehaviour
{
    public CanvasRenderer[] LiquidRenderer;
    [Range(0.0f, 1.0f)]
    public float[] fill;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < LiquidRenderer.Length; i++)
        {
            if (LiquidRenderer[i].GetMaterial() != null)
            {
                LiquidRenderer[i].GetMaterial().SetFloat("_Progress", fill[i]);
            }
        }
    }
}
