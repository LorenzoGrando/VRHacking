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

        if(transform.position.x > (upperRightAnchor.position.x)) {
            if(assignPosition)
                transform.position = new Vector3(upperRightAnchor.position.x, transform.position.y, transform.position.z);
            hitBounds = true;
        }

        if(transform.position.x < (lowerLeftAnchor.position.x)) {
            if(assignPosition)
                transform.position = new Vector3(lowerLeftAnchor.position.x, transform.position.y, transform.position.z);
            hitBounds = true;
        }

        if(transform.position.y  > (upperRightAnchor.position.y)) {
            if(assignPosition)
                transform.position = new Vector3(transform.position.x, upperRightAnchor.position.y,  transform.position.z);
            hitBounds = true;
        }

        if(transform.position.y < (lowerLeftAnchor.position.y)) {
            if(assignPosition)
                transform.position = new Vector3(transform.position.x, lowerLeftAnchor.position.y,  transform.position.z);
            hitBounds = true;
        }

        if(hitBounds) {
            FireHitEvent();
        }

        return hitBounds;
    }

}