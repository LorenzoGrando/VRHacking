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
    [SerializeField]
    [Range(0,1.5f)]
    private float animationSpeed;

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
        gripInterpolator = InterpolateAnimationValue(gripInterpolator, gripTarget);
        triggerInterpolator = InterpolateAnimationValue(triggerInterpolator, triggerTarget);

        UpdateAnimatorParameters();
    }

    private float InterpolateAnimationValue(float currentValue, float targetValue) {
        //If not alredy in location, interpolates by speed and returns, otherwise just return current value;
        if(currentValue != targetValue) {
            currentValue = Mathf.MoveTowards(currentValue, targetValue, animationSpeed);
        }

        return currentValue;
    }

    private void UpdateAnimatorParameters() {
        animator.SetFloat(gripParameter, gripInterpolator);
        animator.SetFloat(triggerParameter, triggerInterpolator);
    }
}
