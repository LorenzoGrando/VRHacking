using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchingImagesTask : HackTask
{
    #region Generic Task Methods

    protected override void ResetTask()
    {
        InitializeValues();
        display.ResetDisplay();
    }

    public override void HideTask()
    {
        display.HideButtons();
    }
    public override void StartTask(GameSettingsData settingsData)
    {
        this.gameSettingsData = settingsData;
        ResetTask();

        GenerateTargetSequence(GenerateSequenceSize());
    }

    protected override bool CheckTaskCompleted()
    {
        return correctMatches == numberOfPairs;
    }

    protected override void CompleteTask()
    {
        //Other possible behaviour (anims, etc)
        //Will fire completion event
        display.HideButtons();
        display.OnLeaveAnimationFinish += DeactivePrefabObject;
        base.CompleteTask();
    }

    #endregion

    #region Character Sequence 

    #region Internal Parameters
    public enum ImageType {
        Null, Icon1, Icon2, Icon3, Icon4
    }
    private List<ImageType> orderedSequence;
    private List<ImageType> shuffledSequence;
    private ImageType currentSelectedType;
    private int correctMatches;
    private int numberOfPairs;
    

    #endregion

    [SerializeField]
    private MatchingImagesDisplay display;


    private void InitializeValues() {
        orderedSequence = new List<ImageType>();
        shuffledSequence = new List<ImageType>();
        currentSelectedType = ImageType.Null;
        correctMatches = 0;
    }

    private int GenerateSequenceSize() {
        int sequenceSize = Enum.GetNames(typeof(ImageType)).Length - 1;
        numberOfPairs = (int)UnityEngine.Random.Range(2, sequenceSize/2 + ((sequenceSize/2) * (2 * (gameSettingsData.difficulty - 1) )));
        return numberOfPairs;
    }

    private void GenerateTargetSequence(int imageAmount) {
        //Reset Values
        orderedSequence.Clear();
        shuffledSequence.Clear();

        for(int i = 1; i < imageAmount + 1; i++) {
            orderedSequence.Add((ImageType)i);
            orderedSequence.Add((ImageType)i);
        }

        shuffledSequence = new List<ImageType>(orderedSequence);
        shuffledSequence.Shuffle();

        display.InitiateDisplay(orderedSequence, shuffledSequence);
    }

    public void TryActivateImage(ImageType selectedType) {
        if(currentSelectedType == ImageType.Null) {
            currentSelectedType = selectedType;
            display.SelectLastButton();
        }
        else if(currentSelectedType != ImageType.Null) {
            if(currentSelectedType == selectedType) {
                correctMatches++;
                display.ActivateMatchingButtons(currentSelectedType);
            }
            currentSelectedType = ImageType.Null;
            display.DeselectButtons();
        }

        if(CheckTaskCompleted()) {
            CompleteTask();
        }
    }

    public void ResetActiveType() => currentSelectedType = ImageType.Null;

    private void DeactivePrefabObject() {
        prefabObject.SetActive(false);
    }

    #endregion
}