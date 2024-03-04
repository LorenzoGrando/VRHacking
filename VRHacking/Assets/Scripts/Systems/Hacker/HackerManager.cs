using System;
using UnityEngine;

public class HackerManager : MonoBehaviour
{
    public event Action OnHackerTasksCompleted;

    [SerializeField]
    private HackerData[] hackers;
    private HackerData activeHacker;

    private GameSettings.GameSettingsData gameSettings;
    
    #region Game Loop Parameters
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
    }

    public void InitializeHackerData(GameSettings.GameSettingsData gameSettings) {
        this.gameSettings = gameSettings;

        SelectHackerByLevel(this.gameSettings.level);

        ResetValues();
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
        int sequenceSize = GenerateSequenceSize();
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
}