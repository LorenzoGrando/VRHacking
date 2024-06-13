using UnityEngine;

public class FindAndSeekRaycaster : MonoBehaviour
{
    private FindAndSeekTask.TargetIcon currentTargetIcon;
    private bool isActive;
    [SerializeField]
    private LayerMask collisionLayers;

    public void UpdateFireStatus(bool isActive) => this.isActive = isActive;
    public void UpdateTargetIcon(FindAndSeekTask.TargetIcon newTarget) => currentTargetIcon = newTarget;

    public bool HitTargetIcon() {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, collisionLayers)) {
            FindAndSeekIcon icon;
            if(hit.collider.gameObject.TryGetComponent<FindAndSeekIcon>(out icon))
                return icon.thisIconType == currentTargetIcon;

            else return false;
        }
    
        else return false;
    }
}