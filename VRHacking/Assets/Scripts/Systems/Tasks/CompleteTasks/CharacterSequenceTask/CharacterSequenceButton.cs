using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class CharacterSequenceButton : MonoBehaviour
{
    [SerializeField]
    private CharacterSequenceTask task;
    private CharacterSequenceTask.CharacterSequenceData thisButtonData;
    private bool hasActivatedProperly;

    [SerializeField]
    private TextMeshProUGUI textRef;
    [SerializeField]

    void Start()
    {
        if (task == null)
            task = FindObjectOfType<CharacterSequenceTask>();
        
        if(textRef == null)
            textRef = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void InitializeButton(CharacterSequenceTask.CharacterSequenceData data) {
        thisButtonData = data;
        textRef.text = data.characters;
    }

    public void OnButtonPressed() {
        task.TryActivateChar(thisButtonData);
    }
}
