using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceMashUI : MonoBehaviour
{
    [SerializeField] private ParticleSystem splashParticles;
    [SerializeField] private LiquidProgressControllerUI liquidController;

    private CanvasGroup canvasGroup;

    private float value = 0.0f;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void ToggleVisibility(bool _active)
    {
        if (canvasGroup.alpha == 0.0f && _active)
            liquidController.fill[0] = 0.0f;

        canvasGroup.alpha = _active ? 1.0f : 0.0f;
    }
    public void SetValue(float _value)
    {
        value = _value;

        liquidController.fill[0] = _value;
        splashParticles.Play();
    }
}
