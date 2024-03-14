using System;
using UnityEngine;

public class HackerManager : MonoBehaviour
{
    public event Action OnHackerTasksCompleted;
    public event Action<HackerData> OnHackerBugUploaded;

    [SerializeField]
    private HackerMainDisplay display;
    [SerializeField]
    private HackerData[] hackers;
    private HackerData activeHacker;

    private GameSettings.GameSettingsData gameSettings;
    
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

    public void InitializeHackerData(GameSettings.GameSettingsData gameSettings) {
        this.gameSettings = gameSettings;

        SelectHackerByLevel(this.gameSettings.level);

        ResetValues();

        display.InitiateCanvas();
        display.UpdateHackerData(activeHacker);
    }

    private void ResetValues() {
        taskCompletionAdditionalModifier = 1;
        bugUploadAdditionalModifier = 1;
        lastBugUploadTime = 0;
        lastTaskCompletionTime = 0;

        currentlyExecuting = false;
    }

    private void SelectHackerByLevel(int levelIndex) {
        activeHacker = hackers[levelIndex];
    }

    private int GenerateSequenceSize() {
        int size = baseSequenceSize - Mathf.RoundToInt((gameSettings.difficulty - 1) * baseSequenceSize/3);

        return size;
    }

    public void BeginHackerSequence() {
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
            Debug.Log("Hacker wins!");
        }
        
        else {
            CalculateNextTaskCompletion();
        }
    }

    private void CompleteBugUpload() {
        Debug.Log("Uploaded Bug!");
        OnHackerBugUploaded?.Invoke(activeHacker);
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
        taskCompletionAdditionalModifier = Mathf.Clamp01(modifierValue);
        CalculateNextTaskCompletion();
    }

    public void SetBugUploadModifier(float modifierValue) {
        bugUploadAdditionalModifier = Mathf.Clamp01(modifierValue);
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

}