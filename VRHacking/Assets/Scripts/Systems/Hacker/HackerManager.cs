using System;
using System.Collections.Generic;
using UnityEngine;

public class HackerManager : MonoBehaviour
{
    public event Action OnHackerTasksCompleted;
    public event Action<HackerData> OnHackerBugUploaded;
    public event Action<DialogueRequestData> OnMessageTrigger;

    public HackerMainDisplay display;
    [SerializeField]
    private HackerData[] hackers;
    private HackerData activeHacker;
    private List<int> usedHackerIndexes;

    private GameSettingsData gameSettings;
    
    #region Game Loop Parameters
    private int sequenceSize;
    private int remainingTasksInSequence;
    [SerializeField]
    private int baseSequenceSize;


    [SerializeField]
    [Range(1,20)]
    private float baseTaskCompletionTime;
    [SerializeField]
    [Range(1,20)]
    private float baseBugUploadTime;

    private float taskCompletionAdditionalModifier;
    private float bugUploadAdditionalModifier;

    private bool blockNextBug;

    private bool currentlyExecuting;
    #endregion

    #region Timer Data
    private float lastTaskCompletionTime;
    private float taskCompletionInterval;

    private float lastBugUploadTime;
    private float bugUploadInterval;

    #endregion

    void Update() {
        if(currentlyExecuting)
            CheckTimers();
    }

    private void CheckTimers() {
        float currentTime = Time.time;
        if(lastTaskCompletionTime + taskCompletionInterval < currentTime) {
            lastTaskCompletionTime = currentTime;
            CompleteTask();
        }

        if(lastBugUploadTime + bugUploadInterval < currentTime) {
            lastBugUploadTime = currentTime;
            CompleteBugUpload();
        }

        SendProgressData();
    }

    public HackerData InitializeHackerData(GameSettingsData gameSettings) {
        this.gameSettings = gameSettings;

        if(gameSettings.thisGameMode == GameSettingsData.GameMode.Campaign)
            SelectHackerByLevel(this.gameSettings.level);
        else
            SelectUnchosenHacker();

        ResetValues();

        return activeHacker;
    }

    private void ResetValues() {
        taskCompletionAdditionalModifier = 1;
        bugUploadAdditionalModifier = 1;
        lastBugUploadTime = 0;
        lastTaskCompletionTime = 0;

        blockNextBug = false;

        currentlyExecuting = false;
    }

    private void SelectHackerByLevel(int levelIndex) {
        activeHacker = hackers[levelIndex];
    }

    private void SelectUnchosenHacker() {
        //Regenerates Indexes
        if(usedHackerIndexes == null || usedHackerIndexes.Count <= 0) {
            usedHackerIndexes = new List<int>();
            for(int i = 0; i < hackers.Length; i++) {
                usedHackerIndexes.Add(i);
            }
        }


        int index = UnityEngine.Random.Range(0, usedHackerIndexes.Count);
        activeHacker = hackers[index];
        usedHackerIndexes.RemoveAt(index);
    }

    private int GenerateSequenceSize() {
        int size = baseSequenceSize - Mathf.RoundToInt((gameSettings.difficulty - 1) * baseSequenceSize/3);

        return size;
    }

    public void BeginHackerSequence() {
        display.InitiateCanvas();
        display.UpdateHackerData(activeHacker);
        sequenceSize = GenerateSequenceSize();
        remainingTasksInSequence = sequenceSize;

        CalculateNextBugUpload();
        CalculateNextTaskCompletion();

        float currentTime = Time.time;
        lastBugUploadTime = currentTime;
        lastTaskCompletionTime = currentTime;

        currentlyExecuting = true;
    }

    private void CompleteTask() {
        Debug.Log("Completed Task!");
        remainingTasksInSequence--;
        SendTaskCompletionData();
        if(remainingTasksInSequence <= 0) {
            OnHackerTasksCompleted?.Invoke();
            currentlyExecuting = false;
        }

        else {
            if(remainingTasksInSequence == sequenceSize/2) {
                DialogueRequestData requestData = new DialogueRequestData {
                    type = DialogueAsset.DialogueType.Hacker,
                    source =  DialogueAsset.DialogueSource.TaskThreshold
                };
                OnMessageTrigger?.Invoke(requestData);
            }

            CalculateNextTaskCompletion();
        }
    }

    private void CompleteBugUpload() {
        
        if(!blockNextBug) {
            Debug.Log("Uploaded Bug!");
            OnHackerBugUploaded?.Invoke(activeHacker);

            DialogueRequestData requestData = new DialogueRequestData {
                type = DialogueAsset.DialogueType.Hacker,
                source =  DialogueAsset.DialogueSource.PlayerGlitched
            };
            OnMessageTrigger?.Invoke(requestData);
        }
        else {
            Debug.Log("Bug was Blocked!");
            blockNextBug = false;
        }
        CalculateNextBugUpload();
    }

    private void CalculateNextTaskCompletion() {
        float result = baseTaskCompletionTime * activeHacker.efficiency * (1/gameSettings.difficulty) * taskCompletionAdditionalModifier;

        taskCompletionInterval = result;
    }

    private void CalculateNextBugUpload() {
        float result = baseBugUploadTime * activeHacker.aggressiveness * (1/gameSettings.difficulty) * bugUploadAdditionalModifier;

        bugUploadInterval = result;
    }

    public void SetTaskCompletionModifier(float modifierValue) {
        taskCompletionAdditionalModifier = Mathf.Clamp(modifierValue, 1, 2);
        CalculateNextTaskCompletion();
    }

    public void SetBugUploadModifier(float modifierValue) {
        bugUploadAdditionalModifier = Mathf.Clamp(modifierValue, 1, 2);
        CalculateNextBugUpload();
    }

    private void SendProgressData() {
        float hackerBugUploadData = ClampedTimerData(lastBugUploadTime, Time.time, bugUploadInterval);
        float hackerNextTaskData = ClampedTimerData(lastTaskCompletionTime, Time.time, taskCompletionInterval);

        HackerMainDisplay.SliderData sliderData = new()
        {
            hackerBugUploadValue = hackerBugUploadData,
            hackerNextTaskValue = hackerNextTaskData
            
        };

        display.UpdateContinuousSliders(sliderData);
    }

    private float ClampedTimerData(float initial, float current, float interval) {
        float currentTime = current - initial;

        float remaningTime = (initial + interval) - current;

        if(remaningTime <= 0)
            return 0f;
        
        float clampedValue = currentTime / (currentTime + remaningTime);

        return clampedValue;
    }

    private void SendTaskCompletionData() {
        float value = Mathf.Lerp(0, 1, Mathf.Abs((float)remainingTasksInSequence - (float)sequenceSize)/(float)sequenceSize);
        display.UpdateMainSlider(value);
    }

    public void ModifyCompletedTasks(int modifier) {
        remainingTasksInSequence += modifier;
        Mathf.Clamp(remainingTasksInSequence, 0, sequenceSize);
        SendTaskCompletionData();
    }

    public void TriggerBlockNextBug() {
        blockNextBug = true;
    }

    public void OnEndDispute() {
        display.DisableCanvas();
    }

}