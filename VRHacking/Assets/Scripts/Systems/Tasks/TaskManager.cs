using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TaskManager : MonoBehaviour
{
    public event Action OnPlayerTasksCompleted;
    public event Action<DialogueRequestData> OnMessageTrigger;
    [SerializeField]
    private HackTask[] availableTasks;
    private int lastPerformedTaskIndex;
    private int numberOfAvailableTasks;

    [SerializeField]
    private int baseSequenceLenght;
    private int generatedSequenceSize;
    private int remainingTasksInSequence;
    
    [SerializeField]
    private float newTaskInvervalDuration;

    private GameSettingsData currentData;

    private float currentTaskBeginTime;
    private bool hasCheckedQuickness;

    [SerializeField]
    private AudioSource completeAudioSource;
    [SerializeField]
    private GameObject taskWindowObject;

    [Space(5)]
    [SerializeField]
    private MainScreenDisplay mainScreenDisplay;
    [HideInInspector]
    public bool enableMines;
    private float lastTaskStartTime;

    void Start()
    {
        numberOfAvailableTasks = availableTasks.Length;
        enableMines = false;
    }

    void Update()
    {
        if(!hasCheckedQuickness)
            hasCheckedQuickness = TestQuickness(isCompletion: false);
    }

    public bool TestQuickness(bool isCompletion) {
        if(isCompletion) {
            bool wasQuick = Time.time <= currentTaskBeginTime + availableTasks[lastPerformedTaskIndex].taskQuicknessTimeThreshold;
            return wasQuick;
        }

        //See if is slow
        else {
            if(Time.time > currentTaskBeginTime +  (availableTasks[lastPerformedTaskIndex].taskQuicknessTimeThreshold * 3)) {
                DialogueRequestData requestData = new DialogueRequestData {
                    type = DialogueAsset.DialogueType.Hacker,
                    source = DialogueAsset.DialogueSource.SlowTask
                };

                OnMessageTrigger?.Invoke(requestData);

                return true;
            }

            else return false;
        }
    }

    public void BeginTaskSequence(GameSettingsData gameData) {
        currentData = gameData;
        remainingTasksInSequence = CalculateLenght();
        generatedSequenceSize = remainingTasksInSequence;

        taskWindowObject.transform.DOScale(Vector3.one, 0.25f).OnComplete(() => StartNewTask());
    }

    private int CalculateLenght() {
        int result = baseSequenceLenght + Mathf.RoundToInt((currentData.difficulty - 1) * baseSequenceLenght/2);

        return result;
    }

    private void StartNewTask() {
        int index = UnityEngine.Random.Range(0, numberOfAvailableTasks);

        if(index == lastPerformedTaskIndex) {
            index++;

            if(index > numberOfAvailableTasks - 1) {
                index = 0;
            }
        }
        if(enableMines) {
            availableTasks[index].enableMines = true;
            enableMines = false;
        }

        lastTaskStartTime = Time.time;
        availableTasks[index].StartTask(currentData);
        availableTasks[index].OnTaskCompleted += TaskCompleted;


        lastPerformedTaskIndex = index;
        currentTaskBeginTime = Time.time;
        hasCheckedQuickness = false;
    }

    private void TaskCompleted() {
        //Remove from event reaction
        availableTasks[lastPerformedTaskIndex].OnTaskCompleted -= TaskCompleted;
        remainingTasksInSequence--;

        mainScreenDisplay.UpdateTaskSlider(Mathf.Lerp(0, 1, Mathf.Abs((float)remainingTasksInSequence - (float)generatedSequenceSize)/(float)generatedSequenceSize));
        completeAudioSource.Play();
        if(TestQuickness(isCompletion: true)) {
            DialogueRequestData requestData = new DialogueRequestData {
                type = DialogueAsset.DialogueType.Hacker,
                source = DialogueAsset.DialogueSource.EfficientTask
            };

            OnMessageTrigger?.Invoke(requestData);
        }
        GameSettings.CurrentRunData.tasksCompleted++;
        float previousAverage = GameSettings.CurrentRunData.averageTaskCompletionTime;
        GameSettings.CurrentRunData.averageTaskCompletionTime = previousAverage + (((Time.time - lastTaskStartTime) - previousAverage)/GameSettings.CurrentRunData.tasksCompleted);
        
        taskWindowObject.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => CheckContinueSequence());
        
    }


    private void CheckContinueSequence() {
        if(remainingTasksInSequence <= 0) {
            FinishTaskSequence();
        }
        else {
            StartCoroutine(routine: WaitForNewTask());
        }
    }
    private void FinishTaskSequence() {
        OnPlayerTasksCompleted?.Invoke();
    }

    private IEnumerator WaitForNewTask() {
        yield return new WaitForSeconds(newTaskInvervalDuration);

        taskWindowObject.transform.DOScale(Vector3.one, 0.25f).OnComplete(() => StartNewTask());

        yield break;
    }

    public void OnEndDispute() {
        availableTasks[lastPerformedTaskIndex].OnTaskCompleted -= TaskCompleted;
        taskWindowObject.transform.DOScale(Vector3.one, 0.15f);
        HideAllTasks();
        StopAllCoroutines();
    }

    public void HideAllTasks() {
        foreach(HackTask task in availableTasks) {
            task.HideTask();
        }
    }
}
