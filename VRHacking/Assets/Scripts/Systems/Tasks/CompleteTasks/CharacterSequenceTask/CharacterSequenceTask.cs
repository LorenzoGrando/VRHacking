using UnityEngine;
using System;
using System.Collections.Generic;

public class CharacterSequenceTask : HackTask
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
        if(currentOrderValue == orderedSequence.Count - 1) {
            return true;
        }

        return false;
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
    public struct CharacterSequenceData {
        public string characters;
        public int numberInOrder;
        public bool isBugged;
    }

    private const string possibleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private char[] possibleCharsArray = possibleCharacters.ToCharArray();
    private int possibleCharsLenght = possibleCharacters.Length;
    private System.Random rnd;

    private List<CharacterSequenceData> orderedSequence;
    private List<CharacterSequenceData> shuffledSequence;
    
    private int currentOrderValue;

    #endregion

    [SerializeField]
    CharacterSequenceDisplay display;

    private void InitializeValues() {
        if(rnd == null) {
            rnd = new System.Random();
        }
        orderedSequence = new List<CharacterSequenceData>();
        shuffledSequence = new List<CharacterSequenceData>();
    }

    private int GenerateSequenceSize() {
        //At max difficulty (2), max possible value is 8;
        int sequenceSize = Mathf.CeilToInt(UnityEngine.Random.Range(3, 4) * gameSettingsData.difficulty);
        return sequenceSize;
    }

    private void GenerateTargetSequence(int stringSize) {
        //Reset Values
        orderedSequence.Clear();
        shuffledSequence.Clear();
        currentOrderValue = -1;
        CharacterSequenceData mineData = new CharacterSequenceData();


        for(int i = 0; i < stringSize; i++) {
            //Generates a 2-3 char long string
            int generationlenght = GenerateSingleCharLenght();
            char[] chars = new char[generationlenght];
            for(int l = 0; l < generationlenght; l++) {
                chars[l] = possibleCharsArray[rnd.Next(possibleCharsLenght)];
            }

            if(enableMines) {
                int rnd = UnityEngine.Random.Range(0,2);
                if(rnd == 0) {
                    CharacterSequenceData buggedData = new CharacterSequenceData() {
                        characters = new string(chars),
                        numberInOrder = i,
                        isBugged = true
                    };
                    mineData = buggedData;
                    enableMines = false;       
                }
            }

            //Initializes and adds data
            CharacterSequenceData data = new CharacterSequenceData() {
                characters = new string(chars),
                numberInOrder = i
            };
            orderedSequence.Add(data);
            shuffledSequence.Add(data);
        }

        if(mineData.isBugged) {
            shuffledSequence.Add(mineData);
        }

        shuffledSequence.Shuffle();

        display.InitiateDisplay(orderedSequence, shuffledSequence);
    }

    private int GenerateSingleCharLenght() {
        //TODO: Make it more likely to generate 3-letter strings on higher difficulties
        //max exclusive
        int lenght = Mathf.Clamp(Mathf.RoundToInt(2 * gameSettingsData.difficulty + (- 0.6f + UnityEngine.Random.Range(0,2))), 2, 3);
        return lenght;
    }

    public bool TryActivateChar(CharacterSequenceData data){
        if(data.isBugged) {
            CallGlitch();
            return false;
        }


        bool validButton = currentOrderValue == data.numberInOrder - 1;
        if(validButton) {
            currentOrderValue++;
        }
        else {
            currentOrderValue = -1;
        }

        display.UpdateDisplay(currentOrderValue);

        if(CheckTaskCompleted()) {
            CompleteTask();
        }

        return validButton;
    }

    private void DeactivePrefabObject() {
        prefabObject.SetActive(false);
    }

    #endregion

}