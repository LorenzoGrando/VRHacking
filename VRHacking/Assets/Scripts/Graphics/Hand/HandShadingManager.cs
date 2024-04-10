using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandShadingManager : MonoBehaviour
{
    [SerializeField]
    private Material material;
    private Sequence animationSequence;
    [Header("Animation")]
    [SerializeField]
    private float inOutAnimDuration;
    [SerializeField]
    private float holdErrorAnimDuration;
    [Header("Color Attributes")]
    [SerializeField]
    private Color baseColor;
    [SerializeField]
    private Gradient errorGradient, returnGradient;

    [Header("Displacement Attributes")]
    [SerializeField]
    private int temp;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            TriggerGlitchEffect();
        }
    }

    void OnEnable()
    {
        ResetToDefault();
    }

    public void TriggerGlitchEffect() {
        material.color = baseColor;
        
        if(animationSequence != null) {
            animationSequence.Pause();
            animationSequence.Kill();
        }

        animationSequence = CreateSequence();
        animationSequence.Play();
    }

    private void ResetToDefault() {
        material.color = baseColor;
        material.SetFloat("_UseDisplacement", 0);
    }

    private Sequence CreateSequence() {
        Sequence sequence = DOTween.Sequence();
        //Lerp towards first color
        sequence.Append(material.DOColor(errorGradient.colorKeys[0].color, 0.15f).SetEase(Ease.InExpo));
        //Interpolate vertex displacement
        Tween tTween = DOVirtual.Float(0, 1, holdErrorAnimDuration/4, UpdateInterpolator);
        sequence.Insert(0, tTween);
        //Loop in error gradient color
        sequence.Append(material.DOGradientColor(errorGradient, holdErrorAnimDuration));
        //Return to base color through gradient
        sequence.Append(material.DOGradientColor(returnGradient, inOutAnimDuration).SetEase(Ease.OutExpo));
        //Interpolate vertex displacement
        Tween tTweenEnd = DOVirtual.Float(1, 0, inOutAnimDuration, UpdateInterpolator);
        sequence.Insert(2, tTweenEnd);

        return sequence;
    }

    private void UpdateInterpolator(float t) {
        material.SetFloat("_DisplacementInterpolator", t);
        material.SetFloat("_UseDisplacement", t);
    }
}
