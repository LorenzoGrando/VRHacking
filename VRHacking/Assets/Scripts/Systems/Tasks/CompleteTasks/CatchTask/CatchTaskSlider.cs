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
        float xPosition = Mathf.Lerp(leftAnchor.localPosition.x, rightAnchor.localPosition.x, 0.5f);
        transform.localPosition = new Vector3(xPosition, transform.localPosition.y, transform.localPosition.z);
    }

    void Update()
    {
        restrainer.TryRestrain(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Catchable")) {
            other.GetComponent<CatchTaskCatchable>().OnExistanceFutile();
            OnCollectCatchable?.Invoke();
        }
    }

    public void ChangeColliderStatus(bool newStatus) {
        if(col != null)
            col.enabled = newStatus;
    }
 }