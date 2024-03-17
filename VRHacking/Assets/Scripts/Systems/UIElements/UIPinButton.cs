using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class UIPinButton : MonoBehaviour
{
    [SerializeField]
    private UIPinManager pinManager;
    private bool isActive;

    void OnEnable()
    {
        isActive = false;
    }
    
    public void OnClickPin() {
        if(!isActive) {
            pinManager.AddNewPin(gameObject);
            isActive = true;
            return;
        }

        else {
            pinManager.RemovePin(gameObject);
            isActive = false;
            return;
        }
    }

    public void ChangePinStatus(bool newStatus) {
        isActive = newStatus;
    }
}
