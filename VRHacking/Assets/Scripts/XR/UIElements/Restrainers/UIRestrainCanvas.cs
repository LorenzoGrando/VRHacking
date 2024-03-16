using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRestrainCanvas : UIRestrainer
{
    private RectTransform myRect;
    [SerializeField]
    private RectTransform canvasRect;

    void OnEnable()
    {
        if(myRect == null)
            myRect = GetComponent<RectTransform>();
        if(canvasRect == null) {
            canvasRect = GetComponentInParent<RectTransform>();
        }
    }
    public override bool TryRestrain(bool assignPosition) {
        bool hitBounds = false;

        if(myRect.anchoredPosition.x > (canvasRect.rect.width/2) - (myRect.rect.width/2 * myRect.localScale.x)) {
            if(assignPosition)
                myRect.anchoredPosition = new Vector3(canvasRect.rect.width/2 - (myRect.rect.width/2 * myRect.localScale.x), myRect.anchoredPosition.y, myRect.anchoredPosition3D.z);
            hitBounds = true;
        }

        if(myRect.anchoredPosition.x  < (-canvasRect.rect.width/2) + (myRect.rect.width/2 * myRect.localScale.x)) {
            if(assignPosition)
                myRect.anchoredPosition = new Vector3(-canvasRect.rect.width/2 + (myRect.rect.width/2 * myRect.localScale.x), myRect.anchoredPosition.y, myRect.anchoredPosition3D.z);
            hitBounds = true;
        }

        if(myRect.anchoredPosition.y > (canvasRect.rect.height/2) - (myRect.rect.height/2 * myRect.localScale.y)) {
            if(assignPosition)
                myRect.anchoredPosition = new Vector3(myRect.anchoredPosition.x, canvasRect.rect.height/2 - (myRect.rect.height/2 * myRect.localScale.y),  myRect.anchoredPosition3D.z);
            hitBounds = true;
        }

        if(myRect.anchoredPosition.y  < (-canvasRect.rect.height/2) + (myRect.rect.height/2 * myRect.localScale.y)) {
            if(assignPosition)
                myRect.anchoredPosition = new Vector3(myRect.anchoredPosition.x, -canvasRect.rect.height/2 + (myRect.rect.height/2 * myRect.localScale.y),  myRect.anchoredPosition3D.z);
            hitBounds = true;
        }

        if(hitBounds) {
            FireHitEvent();
        }

        return hitBounds;
    }
}
