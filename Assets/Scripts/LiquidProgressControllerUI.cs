using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiquidProgressControllerUI : MonoBehaviour
{
    public CanvasRenderer[] LiquidRenderer;
    public Image[] Liquids;
    public Image[] Icons;

    [Range(0.0f, 1.0f)]
    public float[] fill;

    public float yMax;
    public float yMin;
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

    public void SetValues(LiquidQuantity liquid)
    {
        float[] fill = liquid.GetUIValues();
        for (int i = 0; i < fill.Length; i++)
        {
            if (i < Icons.Length)
            {
                if (liquid.At(i) > 0)
                {
                    Icons[i].gameObject.SetActive(true);
                    Icons[i].rectTransform.anchoredPosition = new Vector2(Icons[i].rectTransform.anchoredPosition.x, Mathf.Lerp(yMin, yMax, fill[i]));
                }
                else
                    Icons[i].gameObject.SetActive(false);
            }
            this.fill[i] = fill[i];
        }
    }
}
