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
        GenerateOrderedSequence(GenerateSequenceSize(), false);
    }

    protected override bool CheckTaskCompleted()
    {
        if(currentOrderInSequence == targetValue) {
            display.OnScalingAnimComplete += CompleteTask;
            display.ScaleDisplay(false);
            return true;
        }
        else return false;
    }

    protected override void CompleteTask() {
        display.OnScalingAnimComplete -= CompleteTask;
        display.ResetDisplay();
        base.CompleteTask();
    }

    protected override void ResetTask()
    {
        InitializeValues();
        currentOrderInSequence = 1;
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

    private void GenerateOrderedSequence(int size, bool isReset) {
        orderedData.Clear();
        currentOrderInSequence = 1;
        targetValue = size + 1;

        for(int i = 1; i <= size; i++) {
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

        display.InitializeDisplay(orderedData, !isReset);
    }

    private int GenerateSequenceSize() {
        int sequenceSize = Mathf.CeilToInt(UnityEngine.Random.Range(2, 4) * gameSettingsData.difficulty);
        return sequenceSize;
    }

    public bool TryActiveButton(QuickMemoryData data) {
        if(data.isBugged) {
            CallGlitch();
            CallRestartTask();
            return false;
        }

        bool validButton = currentOrderInSequence == data.numberInOrder;

        if(validButton) {
            currentOrderInSequence++;
        }
        else {
            CallRestartTask();
            return false;
        }

        if(CheckTaskCompleted()) {
            CompleteTask();
        }

        return validButton;
    }

    public void CallRestartTask() {
        display.ResetDisplay();
        display.InitializeDisplay(orderedData, false);
        display.OnCompleteAnim(true);
    }


    #endregion
}