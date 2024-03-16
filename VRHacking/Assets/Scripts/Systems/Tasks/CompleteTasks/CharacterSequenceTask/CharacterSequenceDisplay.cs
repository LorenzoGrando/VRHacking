using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CharacterSequenceDisplay : MonoBehaviour
{
    public event Action OnLeaveAnimationFinish;
    [SerializeField]
    TextMeshProUGUI mainSequenceTextRef;
    private const string defaultColor = "#959595";
    private const string correctColor = "#56E554";

    [SerializeField]
    CharacterSequenceButton[] characterButtons;

    [SerializeField]
    private Vector3 targetTextScale;
    private List<CharacterSequenceTask.CharacterSequenceData> orderedData;
    
    public void InitiateDisplay(List<CharacterSequenceTask.CharacterSequenceData> orderedList, List<CharacterSequenceTask.CharacterSequenceData> shuffledList) {
        gameObject.SetActive(true);
        orderedData = orderedList;

        GenerateButtons(shuffledList);
        DisplaySequenceCharacters();
    }

    public void UpdateDisplay(int orderValue) {
        string correctChars = "";
        string remainingChars = "";

        for(int i = 0; i < orderedData.Count; i++) {
            if(i <= orderValue) {
                correctChars += orderedData[i].characters;
            }
            else {
                remainingChars += orderedData[i].characters;
            }
        }

        if(orderValue <= 0) {
            foreach(CharacterSequenceButton button in characterButtons) {
                button.ResetStatus();
            }
        }

        mainSequenceTextRef.text = $"<color={correctColor}>{correctChars}</color>" + $"<color={defaultColor}>{remainingChars}</color>";
    }

    public void GenerateButtons(List<CharacterSequenceTask.CharacterSequenceData> characterList) {
        int numberOfButtons = characterList.Count;
        int buttonsArrayLenght = characterButtons.Length;
        //Start at a random point in elipse
        int randomStart = UnityEngine.Random.Range(0,buttonsArrayLenght);
        for(int i = 0; i < numberOfButtons; i++) {
            characterButtons[randomStart].gameObject.SetActive(true);
            characterButtons[randomStart].InitializeButton(characterList[i]);

            randomStart++;

            //Restart from beginning
            if(randomStart == buttonsArrayLenght) {
                randomStart = 0;
            }
        }
    }

    public void DisplaySequenceCharacters() {
        mainSequenceTextRef.transform.localScale = Vector3.zero;
        mainSequenceTextRef.transform.DOScale(targetTextScale, .25f);

        UpdateDisplay(-1)
;    }
    public void HideSequenceCharacters() {
        mainSequenceTextRef.transform.DOScale(Vector3.zero, .5f);
    }

    public void HideButtons() {
        bool isFirst = true;
        HideSequenceCharacters();
        foreach(CharacterSequenceButton button in characterButtons) {
            button.DeactiveButton(isFirst, this);

            if(isFirst)
                isFirst = false;
        }
    }

    public void ResetDisplay() {
        foreach(CharacterSequenceButton button in characterButtons) {
            button.ResetStatus();
            button.gameObject.SetActive(false);
        }
    }

    public void FireAnimCompletionEvent(bool isEntry) {
        if(!isEntry) {
            OnLeaveAnimationFinish?.Invoke();
        }
    }
}
