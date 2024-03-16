using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider))]
public class CatchTaskSlider : MonoBehaviour
{
    public event Action OnCollectCatchable;
    private BoxCollider col;
    [SerializeField]
    UIRestrainer restrainer;
    [SerializeField]
    private Transform leftAnchor, rightAnchor;
    public void ResetToDefaultPosition() {
        if(col == null) {
            col = GetComponent<BoxCollider>();
        }
        float xPosition = Mathf.Lerp(leftAnchor.position.x, rightAnchor.position.x, 0.5f);
        transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Catchable")) {
            other.GetComponent<CatchTaskCatchable>().OnExistanceFutile();
            OnCollectCatchable?.Invoke();
        }
    }

    public void ChangeColliderStatus(bool newStatus) {
        col.enabled = newStatus;
    }
 }