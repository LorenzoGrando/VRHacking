using UnityEngine;

public abstract class UIRestrainer : MonoBehaviour
{
    private bool hitBounds;
    public abstract bool TryRestrain(bool assignPosition);
}