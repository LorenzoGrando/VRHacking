using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UIDraggable : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;
    private Transform interactorTransform;
    private bool isSelected;
    private Vector3 selectionOffset;
    private Camera mainCam;
    public void OnSelectEnterEvent(SelectEnterEventArgs args) {
        interactorTransform = args.interactorObject.GetAttachTransform(args.interactableObject);
        isSelected = true;
    }

    public void OnSelectExitEvent(SelectExitEventArgs args) {
        interactorTransform = null;
        isSelected = false;
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<RectTransform>();
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if(isSelected)
            MoveToInteractorTransform();
    }

    private void MoveToInteractorTransform() {
        /*
        Vector3 thisWorldPos = rectTransform.TransformPoint(rectTransform.rect.center);
        
        thisWorldPos.x = interactorTransform.position.x;
        thisWorldPos.y = interactorTransform.position.y;

        thisWorldPos += selectionOffset;

        rectTransform.anchoredPosition = rectTransform.InverseTransformPoint(thisWorldPos);

        */
        rectTransform.anchoredPosition = GetLocalPositionInRect();
    }

    private Vector2 GetLocalPositionInRect() {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (canvasRectTransform, mainCam.WorldToScreenPoint(interactorTransform.position), mainCam, out localPosition);
        return localPosition;
    }
}
