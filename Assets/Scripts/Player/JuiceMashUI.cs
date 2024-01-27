using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceMashUI : MonoBehaviour
{
    [SerializeField] private ParticleSystem splashParticles;
    [SerializeField] private LiquidProgressControllerUI liquidController;

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Color[] materialColors;

    private CanvasGroup canvasGroup;
    private Material material;
    private int currentType = -1;

    private float value = 0.0f;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void ToggleVisibility(bool _active)
    {
        if (canvasGroup.alpha == 0.0f && _active)
            liquidController.fill[0] = 1.0f;

        canvasGroup.alpha = _active ? 1.0f : 0.0f;
    }
    public void SetValue(float _value)
    {
        value = _value;

        liquidController.fill[0] = _value;
        splashParticles.Play();
    }
    public void SetJuice(int _index)
    {
        if (currentType == _index)
            return;

        var main = splashParticles.main;
        main.startColor = materialColors[_index];

        liquidController.Liquids[0].sprite = sprites[_index];
        liquidController.Liquids[0].color = materialColors[_index];

        liquidController.Liquids[0].material.color = materialColors[_index];
        liquidController.Liquids[0].material.SetColor("_WaterColor", materialColors[_index]);
    }
}
