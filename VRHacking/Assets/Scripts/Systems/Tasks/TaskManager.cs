using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    [Space(5)]
    [SerializeField]
    private MainScreenDisplay mainScreenDisplay;


    void Start()
    {
        numberOfAvailableTasks = availableTasks.Length;
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

        StartNewTask();
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
        availableTasks[index].StartTask(currentData);
        Debug.Log(availableTasks[index].gameObject.name);
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

        if(TestQuickness(isCompletion: true)) {
            DialogueRequestData requestData = new DialogueRequestData {
                type = DialogueAsset.DialogueType.Hacker,
                source = DialogueAsset.DialogueSource.EfficientTask
            };

            OnMessageTrigger?.Invoke(requestData);
        }

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

        StartNewTask();

        yield break;
    }

    public void OnEndDispute() {
        availableTasks[lastPerformedTaskIndex].OnTaskCompleted -= TaskCompleted;
        HideAllTasks();
        StopAllCoroutines();
    }

    public void HideAllTasks() {
        foreach(HackTask task in availableTasks) {
            task.HideTask();
        }
    }
}
