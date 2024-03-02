using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ActionBasedController))]
public class HandController : MonoBehaviour
{
    private ActionBasedController controller;
    [SerializeField]
    private Hand hand;

    void Start()
    {
        controller = GetComponent<ActionBasedController>();
    }


    void Update()
    {
        ReadHandInputs();
    }

    private void ReadHandInputs() {
        hand.UpdateGripValue(controller.selectAction.action.ReadValue<float>());
        hand.UpdateTriggerValue(controller.activateAction.action.ReadValue<float>());
    }
}
