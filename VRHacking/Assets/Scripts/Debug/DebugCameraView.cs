#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCameraView : MonoBehaviour
{
    [SerializeField]
    private GameObject[] viewTargets;
    int currentIndex;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            currentIndex++;
            if(currentIndex > viewTargets.Length - 1) {
                currentIndex = 0;
            }
            transform.LookAt(viewTargets[currentIndex].transform);
        }
    }
}

#endif
