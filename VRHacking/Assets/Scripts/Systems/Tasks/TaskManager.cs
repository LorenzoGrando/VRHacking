using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public event Action OnPlayerTasksCompleted;
    [SerializeField]
    private HackTask[] availableTasks;
    private int lastPerformedTaskIndex;
    private int numberOfAvailableTasks;

    [SerializeField]
    private int baseSequenceLenght;
    private int remainingTasksInSequence;
    
    [SerializeField]
    private float newTaskInvervalDuration;

    private GameSettings.GameSettingsData currentData;


    void Start()
    {
        numberOfAvailableTasks = availableTasks.Length;
    }

    public void BeginTaskSequence(GameSettings.GameSettingsData gameData) {
        currentData = gameData;
        remainingTasksInSequence = CalculateLenght();

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
        availableTasks[index].OnTaskCompleted += TaskCompleted;

        lastPerformedTaskIndex = index;
    }

    private void TaskCompleted() {
        //Remove from event reaction
        availableTasks[lastPerformedTaskIndex].OnTaskCompleted -= TaskCompleted;
        remainingTasksInSequence--;

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
}
