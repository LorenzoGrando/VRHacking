using UnityEngine;

public class FallingObjectDeadZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("FallingObject")) {
            other.GetComponent<FallingObjectCatchable>().OnExistanceFutile();
        } 
    }
}