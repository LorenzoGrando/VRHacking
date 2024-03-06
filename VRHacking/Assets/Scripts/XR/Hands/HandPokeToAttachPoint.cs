using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRPokeInteractor))]
public class HandPokeToAttachPoint : MonoBehaviour
{
    [SerializeField]
    private XRPokeInteractor pokeInteractor;
    [SerializeField]
    private Transform attachPoint;

    void Start()
    {
        SetPokeAttachPoint();
    }

    private void SetPokeAttachPoint() {
        if(pokeInteractor == null) {
            Debug.LogError("You must set the poke interaction reference.");
            return;
        }
            
        if(attachPoint == null) {
            Debug.LogError("You must set the target attach point transform.");
            return;
        }

        pokeInteractor.attachTransform = attachPoint;

    }
}
