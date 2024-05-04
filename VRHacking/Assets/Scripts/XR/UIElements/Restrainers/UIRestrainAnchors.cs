using UnityEngine;

public class UIRestrainAnchors : UIRestrainer
{
    private RectTransform myRect;
    [SerializeField]
    private Transform upperRightAnchor;
    [SerializeField]
    private Transform lowerLeftAnchor;

    void Start() {
        myRect = GetComponent<RectTransform>();
    }

    public void UpdateAnchors(Transform bottomLeftAnchor, Transform topRightAnchor) {
        lowerLeftAnchor = bottomLeftAnchor;
        upperRightAnchor = topRightAnchor;
    }
    public override bool TryRestrain(bool assignPosition)
    {
        bool hitBounds = false;

        if(transform.localPosition.x > (upperRightAnchor.localPosition.x)) {
            if(assignPosition)
                transform.localPosition = new Vector3(upperRightAnchor.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            hitBounds = true;
        }

        if(transform.localPosition.x < (lowerLeftAnchor.localPosition.x)) {
            if(assignPosition)
                transform.localPosition = new Vector3(lowerLeftAnchor.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            hitBounds = true;
        }

        if(transform.localPosition.y  > (upperRightAnchor.localPosition.y)) {
            if(assignPosition)
                transform.localPosition = new Vector3(transform.localPosition.x, upperRightAnchor.localPosition.y,  transform.localPosition.z);
            hitBounds = true;
        }

        if(transform.localPosition.y < (lowerLeftAnchor.localPosition.y)) {
            if(assignPosition)
                transform.localPosition = new Vector3(transform.localPosition.x, lowerLeftAnchor.localPosition.y,  transform.localPosition.z);
            hitBounds = true;
        }

        if(hitBounds) {
            FireHitEvent();
        }

        return hitBounds;
    }

}