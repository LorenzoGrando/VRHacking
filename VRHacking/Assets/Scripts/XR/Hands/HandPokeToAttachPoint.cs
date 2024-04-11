using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

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

    public void TryHoverEnterUI(UIHoverEventArgs enterArgs) {
        PokeButtonUI interactable;
        enterArgs.uiObject.TryGetComponent<PokeButtonUI>(out interactable);

        if(interactable != null) {
            interactable.OnXRUIHover(enterArgs);
        }
    }

    public void TryHoverExitUI(UIHoverEventArgs exitArgs) {
        PokeButtonUI interactable;
        exitArgs.uiObject.TryGetComponent<PokeButtonUI>(out interactable);

        if(interactable != null) {
            interactable.OnXRUIHoverExit(exitArgs);
        }
    }
}
