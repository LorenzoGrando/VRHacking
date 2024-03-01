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

    }

    private void ReadHandInputs() {
        
    }
}
