using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class CatchTaskCatchable : MonoBehaviour
{
    public Vector3 targetLocalScale;
    private IObjectPool<CatchTaskCatchable> poolReference;
    private Rigidbody rb;
    [SerializeField]
    private UIRestrainer restrainer;

    private float moveSpeed;


    public void OnEnable()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody>();
        
    }
    public void UpdatePool (IObjectPool<CatchTaskCatchable> pool) {
        poolReference = pool;
    }

    public void SetSpeed(float speed) {
        moveSpeed = -speed;
        rb.velocity = new Vector3(0, moveSpeed, 0);
    }

    public void OnExistanceFutile() {
        Debug.Log("Called Existance Futile");
        poolReference.Release(this);
    }

    void LateUpdate()
    {
        if(restrainer.TryRestrain(true)) {
            OnExistanceFutile();
        }
    }

}