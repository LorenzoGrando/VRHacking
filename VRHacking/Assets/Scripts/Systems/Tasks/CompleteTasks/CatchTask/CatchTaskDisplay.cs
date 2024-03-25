using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CatchTaskDisplay : MonoBehaviour
{
    public event Action OnScalingAnimComplete;
    [Header("Slider Anim Info")]
    [SerializeField]
    private CatchTaskSlider sliderObject;
    [SerializeField]
    private Vector3 targetSliderLocalScale;
    [SerializeField]
    private float scaleSliderDuration;
    [Header("Description Anim Info")]
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private Vector3 targetTextLocalScale;
    [SerializeField]
    private float scaleTextDuration;

    GameSettingsData data;
    public void ResetDisplay() {
        sliderObject.ResetToDefaultPosition();
        sliderObject.transform.localScale = Vector3.zero;
        descriptionText.transform.localScale = Vector3.zero;
    }

    public void InitiateDisplay(GameSettingsData data) {
        this.data = data;
        gameObject.SetActive(true);
        ScaleDisplay(true);
    }

    private void ScaleDisplay(bool isStart) {
        Vector3 targetTextScale;
        Vector3 targetSliderScale;
        float textDur;
        float sliderDur;
        if(isStart) {

            targetTextScale = targetTextLocalScale;
            targetSliderScale = targetSliderLocalScale;
            targetSliderScale.x /= data.difficulty;
            textDur = scaleTextDuration;
            sliderDur = scaleSliderDuration;
        }
        else {
            targetTextScale = Vector3.zero;
            targetSliderScale = Vector3.zero;
            textDur = scaleTextDuration/2;
            sliderDur = scaleSliderDuration/2;
        }
        


        Sequence sequence = DOTween.Sequence();
        sequence.Append(descriptionText.transform.DOScale(targetTextScale, textDur));
        sequence.Append(sliderObject.transform.DOScale(targetSliderScale, sliderDur));
        sequence.OnComplete(() => OnCompleteAnim(isStart));
    }

    private void OnCompleteAnim(bool wasStart) {
        sliderObject.ChangeColliderStatus(wasStart);
        OnScalingAnimComplete?.Invoke();
    }

    public void HideDisplay() {
        ScaleDisplay(false);
    }
}
