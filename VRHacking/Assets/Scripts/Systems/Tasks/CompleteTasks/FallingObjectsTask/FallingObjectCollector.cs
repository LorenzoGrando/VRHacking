using UnityEngine;

public class FallingObjectCollector : MonoBehaviour
{
    [SerializeField] private FallingObjectsTask taskManager;
    [SerializeField] private FallingObjectsTask.FallingObjectType _collectionType;
    public FallingObjectsTask.FallingObjectType CollectionType {
        get { return _collectionType; }
    }
    private FallingObjectCatchable lastCollidedObject;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("FallingObject")) {
            lastCollidedObject = other.GetComponent<FallingObjectCatchable>();
            taskManager.TryCollectObject(lastCollidedObject, this);
        } 
    }

    public void DestroyCollidedObject() {
        if(lastCollidedObject != null) {
            lastCollidedObject.OnExistanceFutile();
        }
    }
}