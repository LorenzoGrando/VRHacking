using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    #region Input Definitions
    //valueTarget == value to shoot towards in animation
    //valueInterpolator == current value in animation
    //valueParameter == const name for anim parameter
    //--------------------------------

    //Grip Button
    private float gripTarget;
    private float gripInterpolator;
    private const string gripParameter = "GripParameter";

    //Trigger Button
    private float triggerTarget;
    private float triggerInterpolator;
    private const string triggerParameter = "TriggerParameter";

    #endregion
    [SerializeField]
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        AnimateHand();
    }

    internal void UpdateGripValue(float value) {
        gripTarget = value;
    }

    internal void UpdateTriggerValue(float value) {
        triggerTarget = value;
    }

    private void AnimateHand() {

    }

    private void TryInterpolateAnimation(float currentValue, float targetValue) {
        if(currentValue != targetValue) {
            
        }
    }
}
