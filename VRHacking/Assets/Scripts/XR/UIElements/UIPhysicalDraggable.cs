using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UIPhysicalDraggable : MonoBehaviour
{
    [SerializeField]
    private UIRestrainer restrainer;
    [SerializeField]
    private Transform interactorTransform;
    [SerializeField]
    private Vector3 allowedAxis;    

    private bool isSelected;
    private Vector3 selectionOffset;
    private Vector3 initialPos;
    private Vector3 reversePos;

    private Vector3 previousPos;
    [SerializeField]
    private bool useMomentum;

    [SerializeField]
    private float baseMomentumMultiplier;

    [SerializeField]
    private int maxMomentumSteps;
    private int momentumSteps;

    private bool isOnMomentum;
    private Vector3 momentumDirection;
    float momentumStrength;



    public void OnSelectEnterEvent(SelectEnterEventArgs args) {
        interactorTransform = args.interactorObject.GetAttachTransform(args.interactableObject);
        isSelected = true;
        isOnMomentum = false;

        //Make the object render of top of other canvas elements in same canvas
        transform.SetAsLastSibling();
    }

    public void OnSelectExitEvent(SelectExitEventArgs args) {
        interactorTransform = null;
        isSelected = false;

        if(useMomentum)
            StartMomentum();
    }

    void Start()
    {
        initialPos = transform.position;

        reversePos = new Vector3();
        if(initialPos.x != 0) {
            reversePos.x = 1f;
        }
        if(initialPos.y != 0) {
            reversePos.y = 1f;
        }
        if(initialPos.z != 0) {
            reversePos.z = 1f;
        }
        
    }


    void LateUpdate()
    {
        if(isSelected)
            MoveToInteractorTransform();
            isOnMomentum = restrainer.TryRestrain(true);
        if(isOnMomentum) {
            ExecuteMomentum();
            isOnMomentum = restrainer.TryRestrain(true);
        }
    }

    private void MoveToInteractorTransform() {
        previousPos = transform.position;
        Vector3 targetPos = Vector3.Scale(allowedAxis, interactorTransform.position);
        Vector3 constrainedPos = Vector3.Scale(reversePos, initialPos);

        Vector3 finalPos = new Vector3();

        if(allowedAxis.x > 0) {
            finalPos.x = targetPos.x;
        }
        else {
            finalPos.x = constrainedPos.x;
        }

        if(allowedAxis.y > 0) {
            finalPos.y = targetPos.y;
        }
        else {
            finalPos.y = constrainedPos.y;
        }

        if(allowedAxis.z > 0) {
            finalPos.z = targetPos.z;
        }
        else {
            finalPos.z = constrainedPos.z;
        }

        transform.position = finalPos;
    }

    private void StartMomentum() {
        isOnMomentum = true;
        momentumSteps = maxMomentumSteps;

        momentumDirection = transform.position - previousPos;
        momentumStrength = momentumDirection.magnitude * baseMomentumMultiplier;
        momentumDirection = momentumDirection.normalized;
    }

    private void ExecuteMomentum() {
        transform.position += momentumDirection * momentumStrength;
        momentumStrength /= 4;

        momentumSteps--;

        if(momentumSteps <= 0) {
            isOnMomentum = false;
        }
    }
}
