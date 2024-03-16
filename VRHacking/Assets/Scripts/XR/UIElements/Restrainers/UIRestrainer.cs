using UnityEngine;
using System;

public abstract class UIRestrainer : MonoBehaviour
{
    public event Action OnHitBounds;
    public abstract bool TryRestrain(bool assignPosition);

    protected void FireHitEvent() {
        OnHitBounds?.Invoke();
    }
}