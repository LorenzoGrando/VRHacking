using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class CharacterSequenceButton : PokeButtonUI
{
    [SerializeField]
    private CharacterSequenceTask task;
    private CharacterSequenceTask.CharacterSequenceData thisButtonData;
    private Button buttonScript;
    private bool hasActivatedProperly;
    [SerializeField]
    private Vector3 targetScale;
    [SerializeField]
    private float animDuration;

    void OnEnable()
    {
        if (task == null)
            task = FindObjectOfType<CharacterSequenceTask>();

        if(buttonScript == null) {
            buttonScript = GetComponent<Button>();
        }
    }

    public void InitializeButton(CharacterSequenceTask.CharacterSequenceData data) {
        if(textRef == null) {
            textRef = GetComponentInChildren<TextMeshProUGUI>();
        }
        thisButtonData = data;
        textRef.text = data.characters;

        buttonScript.enabled = false;

        transform.localScale = Vector3.zero;
        transform.DOScale(targetScale, animDuration).SetEase(Ease.InQuad).OnComplete(() => ActivateButton());
    }

    private void ActivateButton() {
        buttonScript.enabled = true;
    }

    public void DeactiveButton(bool generateCallback, CharacterSequenceDisplay display) {
        Tween tween = transform.DOScale(Vector3.zero, animDuration).SetEase(Ease.InQuad);
        if(generateCallback)
            tween.OnComplete(() => display.FireAnimCompletionEvent(false));
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

    public override void OnXRUIHover(UIHoverEventArgs enterArgs)
    {
    }

    public override void OnXRUIHoverExit(UIHoverEventArgs exitArgs)
    {
    }
}
