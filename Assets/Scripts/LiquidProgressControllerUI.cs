using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiquidProgressControllerUI : MonoBehaviour
{
    public CanvasRenderer[] LiquidRenderer;
    public Image[] Liquids;

    [Range(0.0f, 1.0f)]
    public float[] fill;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Liquids.Length; i++)
        {
            if (Liquids[i].material != null)
            {
                Material mat = new Material(Liquids[i].material);
                mat.SetFloat("_Progress", fill[i]);
                Liquids[i].material = mat;
            }
        }
    }

    public void SetValues( float[] values)
    {
        for (int i = 0; i < fill.Length; i++)
        {
            fill[i] = values[i];
        }
    }
}
