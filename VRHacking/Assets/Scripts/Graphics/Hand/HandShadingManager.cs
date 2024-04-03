using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandShadingManager : MonoBehaviour
{
    [SerializeField]
    private Material material;
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

    public void TriggerGlitchEffect() {
        material.color = baseColor;
        Sequence sequenceToExecute = CreateSequence();
        sequenceToExecute.Play();
    }

    private Sequence CreateSequence() {
        Sequence sequence = DOTween.Sequence();
        //Lerp towards first color
        sequence.Append(material.DOColor(errorGradient.colorKeys[0].color, 0.25f).SetEase(Ease.InExpo));
        //Loop in error gradient color
        sequence.Append(material.DOGradientColor(errorGradient, holdErrorAnimDuration));
        //Return to base color through gradient
        sequence.Append(material.DOGradientColor(returnGradient, inOutAnimDuration).SetEase(Ease.OutExpo));

        return sequence;
    }
}
