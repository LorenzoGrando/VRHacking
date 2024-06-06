using UnityEngine;

public class FindAndSeekRaycaster : MonoBehaviour
{
    private FindAndSeekTask.TargetIcon currentTargetIcon;
    private bool isActive;

    public void UpdateFireStatus(bool isActive) => this.isActive = isActive;
    public void UpdateTargetIcon(FindAndSeekTask.TargetIcon newTarget) => currentTargetIcon = newTarget;

    public bool HitTargetIcon() {
        return false;
    }
}