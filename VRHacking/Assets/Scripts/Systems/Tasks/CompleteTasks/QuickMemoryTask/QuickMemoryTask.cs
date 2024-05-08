using System.Collections.Generic;
using UnityEngine;

public class QuickMemoryTask : HackTask
{
    #region Generic Task Methods

    public override void HideTask()
    {
        display.ResetDisplay();

    }

    public override void StartTask(GameSettingsData settingsData)
    {
        this.gameSettingsData = settingsData;
        ResetTask();
        prefabObject.SetActive(true);
        GenerateOrderedSequence(GenerateSequenceSize());
    }

    protected override bool CheckTaskCompleted()
    {
        if(currentOrderInSequence == targetValue - 1) {
            return true;
        }
        else return false;
    }

    protected override void CompleteTask() {
        base.CompleteTask();
        prefabObject.SetActive(false);
    }

    protected override void ResetTask()
    {
        InitializeValues();
        currentOrderInSequence = 0;
        display.ResetDisplay();
    }

    #endregion

    #region Task Specific Methods

    public struct QuickMemoryData {
        public int numberInOrder;
        public bool isBugged;
    }

    private System.Random random;
    private List<QuickMemoryData> orderedData;
    [SerializeField]
    private QuickMemoryDisplay display;
    private int currentOrderInSequence;
    private int targetValue;

    private void InitializeValues() {
        random ??= new System.Random();
        orderedData = new List<QuickMemoryData>();
    }

    private void GenerateOrderedSequence(int size) {
        orderedData.Clear();
        targetValue = size;

        for(int i = 0; i < size; i++) {
            QuickMemoryData data = new QuickMemoryData(){
                numberInOrder = i,
            };

            if(enableMines) {
                int rnd = UnityEngine.Random.Range(0,3);
                if(rnd == 0) {
                    QuickMemoryData buggedData = new QuickMemoryData() {
                        numberInOrder = i,
                        isBugged = true
                    };
                    enableMines = false;     
                    orderedData.Add(buggedData);  
                }
            }

            orderedData.Add(data);
        }

        display.InitializeDisplay(orderedData);
    }

    private int GenerateSequenceSize() {
        int sequenceSize = Mathf.CeilToInt(UnityEngine.Random.Range(2, 4) * gameSettingsData.difficulty);
        return sequenceSize;
    }

    public bool TryActiveButton(QuickMemoryData data) {
        if(data.isBugged) {
            CallGlitch();
            return false;
        }

        bool validButton = currentOrderInSequence == data.numberInOrder;

        if(validButton) {
            currentOrderInSequence++;
        }
        else {
            CallRestartTask();
        }

        if(CheckTaskCompleted()) {
            CompleteTask();
        }

        return validButton;
    }

    public void CallRestartTask() {
        ResetTask();
        GenerateOrderedSequence(orderedData.Count);
    }


    #endregion
}