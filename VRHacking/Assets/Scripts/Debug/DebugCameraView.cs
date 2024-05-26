#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugCameraView : MonoBehaviour
{
    [SerializeField]
    private GameObject xrOriginObject, leftContrl, rightContrl;
    private bool enabled = false;
    void Update() {
        if(Input.GetKey(KeyCode.D)) 
            CallEnableDebug();

        if(enabled) {
            if(Input.GetKeyDown(KeyCode.Q)) 
                ExecuteRotation(true);
            if(Input.GetKeyDown(KeyCode.E))
                ExecuteRotation(false);
        }
    }

    private void CallEnableDebug() {
        enabled = true;
        Vector3 pos = xrOriginObject.transform.position;
        pos.y = 0.12f;
        xrOriginObject.transform.position = pos;

        leftContrl.SetActive(false);
        rightContrl.SetActive(false);
    }

    private void ExecuteRotation(bool isLeft) {
        float rot = 90;
        if(isLeft)
            rot = -90;

        xrOriginObject.transform.Rotate(new Vector3(0, rot, 0));
    }
}

#endif
