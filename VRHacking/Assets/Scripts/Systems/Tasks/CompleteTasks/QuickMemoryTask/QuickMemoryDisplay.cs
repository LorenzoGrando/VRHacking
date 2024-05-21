using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class QuickMemoryDisplay : MonoBehaviour
{
    public event Action OnScalingAnimComplete;
    [SerializeField]
    private GameObject displayHolder;

    [SerializeField]
    private QuickMemoryButton[] characterButtons;

    [SerializeField]
    private Transform bottomLeftAnchor, topRightAnchor;

    private Vector2[] validPositions;
    private List<int> positionIndexes;
    private List<QuickMemoryTask.QuickMemoryData> savedData;

    //Scaling
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private Vector3 targetTextLocalScale;
    [SerializeField]
    private float textScaleDuration;

    private Coroutine displayRoutine;

    public void ResetDisplay() {
        ResetRoutine();
        foreach(QuickMemoryButton button in characterButtons) {
            button.ResetStatus();
            button.gameObject.SetActive(false);
        }
        displayHolder.SetActive(false);
    }

    public void InitializeDisplay(List<QuickMemoryTask.QuickMemoryData> data, bool triggerScale) {
        if(savedData == null) {
            savedData = new List<QuickMemoryTask.QuickMemoryData>();
        }
        savedData.Clear();
        savedData = new List<QuickMemoryTask.QuickMemoryData>(data);
        if(validPositions == null) {
            GeneratePositions();
        }

        displayHolder.SetActive(true);

        if(triggerScale)
            ScaleDisplay(true);
    }

    private void GeneratePositions() {
        float xLenght = topRightAnchor.transform.localPosition.x - bottomLeftAnchor.transform.localPosition.x;
        float yLenght = topRightAnchor.transform.localPosition.y - bottomLeftAnchor.transform.localPosition.y;

        float stepSizeX = xLenght/4f;
        float stepSizeY = yLenght/4f;
        validPositions = new Vector2[16];
        int index = 0;

        positionIndexes = new List<int>();

        for(int x = 0; x < 4; x++) {
            for(int y = 0; y < 4; y++) {
                validPositions[index] = new Vector2((bottomLeftAnchor.localPosition.x) + (x * stepSizeX), (bottomLeftAnchor.localPosition.y * 0.5f) + ( y * stepSizeY));
                positionIndexes.Add(index);
                index++;
            } 
        }
    }

    public Vector2 GenerateButtonPos(int index) {
        return validPositions[index];
    }

    private IEnumerator DisplayButtonsRoutine(List<QuickMemoryTask.QuickMemoryData> data) {
        positionIndexes.Shuffle();
        for(int i = 0; i < data.Count; i++) {
            characterButtons[i].gameObject.SetActive(true);
            characterButtons[i].transform.localPosition = GenerateButtonPos(positionIndexes[i]);
            characterButtons[i].InitializeButton(data[i], 0.25f);

            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1.5f);

        foreach(QuickMemoryButton button in characterButtons) {
            button.HideButtonData();
            button.UpdateButtonStatus(true);
        }

        yield break;
    }

    public void ScaleDisplay(bool isStart) {
        Vector3 targetTextScale;
        Vector3 targetSliderScale;
        float textDur;
        if(isStart) {

            targetTextScale = targetTextLocalScale;
            textDur = textScaleDuration;
        }
        else {
            targetTextScale = Vector3.zero;
            targetSliderScale = Vector3.zero;
            textDur = textScaleDuration/2;
        }
        


        Sequence sequence = DOTween.Sequence();
        sequence.Append(descriptionText.transform.DOScale(targetTextScale, textDur));
        sequence.OnComplete(() => OnCompleteAnim(isStart));
    }

    public void OnCompleteAnim(bool isStart) {
        if(isStart) {
            displayRoutine = StartCoroutine(routine:DisplayButtonsRoutine(savedData));
        }

        else {
            OnScalingAnimComplete?.Invoke();
        }
    }

    private void ResetRoutine() {
        if(displayRoutine != null) {
            StopCoroutine(displayRoutine);
            displayRoutine = null;
        }
    }
}