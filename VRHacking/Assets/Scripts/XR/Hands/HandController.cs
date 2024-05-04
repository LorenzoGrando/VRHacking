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
    [SerializeField]
    private XRPokeInteractor pokeInteractor;

    void Start()
    {
        controller = GetComponent<ActionBasedController>();
    }


    void Update()
    {
        ReadHandInputs();
    }

    public void ChangeInteractorState(bool newState) => pokeInteractor.enabled = newState;

    private void ReadHandInputs() {
        float gripValue = controller.selectAction.action.ReadValue<float>();
        float triggerValue = controller.activateAction.action.ReadValue<float>();
        hand.UpdateGripValue(gripValue);
        hand.UpdateTriggerValue(triggerValue);
    
        UpdateInteractors(gripValue, triggerValue);
    }

    private void UpdateInteractors(float gripValue, float triggerValue) {
        pokeInteractor.enabled = triggerValue > 0 ? true : false;

        //grip
    }
}
