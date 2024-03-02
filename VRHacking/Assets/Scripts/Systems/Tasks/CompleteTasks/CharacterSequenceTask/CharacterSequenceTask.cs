using UnityEngine;
using System;
using System.Collections.Generic;

public class CharacterSequenceTask : HackTask
{
    #region Generic Task Methods

    protected override void ResetTask()
    {
        throw new NotImplementedException();
    }
    protected override void StartTask()
    {
        throw new NotImplementedException();
    }

    protected override bool CheckTaskCompleted()
    {
        throw new NotImplementedException();
    }

    protected override void CompleteTask()
    {
        //Other possible behaviour (anims, etc)

        //Will fire completion event
        base.CompleteTask();
    }

    #endregion

    #region Character Sequence 

    #region Parameters
    public struct CharacterSequenceData {
        public string characters;
        public int numberInOrder;
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

    void Start()
    {
        InitializeValues();
        GenerateTargetSequence(6);
    }

    private void InitializeValues() {
        if(rnd == null) {
            rnd = new System.Random();
        }
        orderedSequence = new List<CharacterSequenceData>();
        shuffledSequence = new List<CharacterSequenceData>();
    }

    private void GenerateTargetSequence(int stringSize) {
        //Reset Values
        orderedSequence.Clear();
        shuffledSequence.Clear();
        currentOrderValue = 0;


        for(int i = 0; i < stringSize; i++) {
            //Generates a 2-3 char long string
            int generationlenght = GenerateSingleCharLenght();
            char[] chars = new char[generationlenght];
            for(int l = 0; l < generationlenght; l++) {
                chars[l] = possibleCharsArray[rnd.Next(possibleCharsLenght)];
            }

            //Initializes and adds data
            CharacterSequenceData data = new CharacterSequenceData() {
                characters = new string(chars),
                numberInOrder = i
            };
            orderedSequence.Add(data);
            shuffledSequence.Add(data);
        }

        shuffledSequence.Shuffle();

        display.InitiateDisplay(orderedSequence, shuffledSequence);

        //Debugging
        string finalString = "";
        string orderedChars = "";
        string shuffledChars = "";
        for(int o = 0; o < orderedSequence.Count; o++) {
            orderedChars += $"{o}. {orderedSequence[o].characters}, ";
            shuffledChars += $"{o}. {shuffledSequence[o].characters}, ";
            finalString += orderedSequence[o].characters;
        }
        Debug.Log("Final Generated String: " + finalString);
        Debug.Log("Generated Order:");
        
        Debug.Log(orderedChars);
        Debug.Log("Shuffled Spiral Order:");
        Debug.Log(shuffledChars);
    }

    private int GenerateSingleCharLenght() {
        //TODO: Make it more likely to generate 3-letter strings on higher difficulties
        //max exclusive
        int lenght = rnd.Next(2,4);
        return lenght;
    }

    public bool TryActivateChar(CharacterSequenceData data){
        if(currentOrderValue != data.numberInOrder) {
            return false;
        }

        currentOrderValue++;

        display.UpdateDisplay(currentOrderValue);

        //CheckTaskCompleted();

        return true;
    }

    #endregion

}