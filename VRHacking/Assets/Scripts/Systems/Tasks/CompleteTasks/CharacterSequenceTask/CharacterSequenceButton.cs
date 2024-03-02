using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSequenceButton : MonoBehaviour
{
    [SerializeField]
    private CharacterSequenceTask task;
    private CharacterSequenceTask.CharacterSequenceData thisButtonData;
    private bool hasActivatedProperly;

    void Start()
    {
        if (task == null)
            task = GetComponentInParent<CharacterSequenceTask>();
    }

    void OnButtonPressed() {
        if(!hasActivatedProperly) {
            if(task.TryActivateChar(thisButtonData)) {
                hasActivatedProperly = true;
            }

            else {
                //Do Error Effect
            }
        }
    }
}
