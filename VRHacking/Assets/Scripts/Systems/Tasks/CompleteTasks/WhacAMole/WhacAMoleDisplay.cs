using System;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class WhacAMoleDisplay : MonoBehaviour
{
    public event Action OnLeaveAnimationFinish;
    [SerializeField]
    private GameObject displayHolder;
    [SerializeField]
    private WhacAMoleButton[] buttons;
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private Vector3 targetButtonScale;
    
    public void InitiateDisplay() {
        descriptionText.transform.localScale = Vector3.zero;
        descriptionText.transform.DOScale(1, 0.25f);
        AnimateButtons(true);
    }

    public void ChangeButtonVisibility(bool status) {
        for(int i = 0; i < buttons.Length; i++) {
            buttons[i].gameObject.SetActive(status);
            if(!status)
                buttons[i].ResetStatus();
        }
    }

    public void AnimateButtons(bool isEntry) {
        Sequence sequence = DOTween.Sequence();

        Vector3 targetScale = isEntry ? targetButtonScale : Vector3.zero;

        for(int i = 0; i < buttons.Length; i++) {
            if(isEntry) {
                buttons[i].transform.localScale = Vector3.zero;
                buttons[i].gameObject.SetActive(true);
            }
            sequence.Insert(0, buttons[i].transform.DOScale(targetScale, 0.25f + (0.1f * i)));
        }

        if(!isEntry)
            sequence.Insert(0, descriptionText.transform.DOScale(0, 0.20f));

        sequence.OnComplete(() => FireAnimCompletionEvent(isEntry));
    }

    public void ResetDisplay() {
        ChangeButtonVisibility(false);
    }

    public void FireAnimCompletionEvent(bool isEntry) {
        if(!isEntry) {
            ChangeButtonVisibility(false);
            OnLeaveAnimationFinish?.Invoke();
            displayHolder.SetActive(false);
        }
    }


    public bool CheckButtonAvailability(int index) {
        return !buttons[index].isActive;
    }

    public void SpawnMole(int index, float availableDuration, bool isMined) {
        buttons[index].mined = isMined;
        buttons[index].UpdateAppearance();
        buttons[index].AnimateMoleButton(availableDuration);
    }
}