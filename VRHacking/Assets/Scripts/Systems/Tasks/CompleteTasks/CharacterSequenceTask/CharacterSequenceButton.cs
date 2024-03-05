using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class CharacterSequenceButton : PokeButtonUI
{
    [SerializeField]
    private CharacterSequenceTask task;
    private CharacterSequenceTask.CharacterSequenceData thisButtonData;
    private bool hasActivatedProperly;

    void Start()
    {
       if (task == null)
            task = FindObjectOfType<CharacterSequenceTask>();
    }

    public void InitializeButton(CharacterSequenceTask.CharacterSequenceData data) {
        if(textRef == null) {
            textRef = GetComponentInChildren<TextMeshProUGUI>();
        }
        thisButtonData = data;
        textRef.text = data.characters;
    }

    public override void OnButtonPressed() {
        if(!hasActivatedProperly) {
            task.TryActivateChar(thisButtonData);
        }
        hasActivatedProperly = true;
    }

    public void ResetStatus() {
        hasActivatedProperly = false;
    }
}
