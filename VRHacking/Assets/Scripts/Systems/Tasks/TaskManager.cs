using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public event Action OnPlayerTasksCompleted;
    public HackTask[] availableTasks;
    private int lastPerformedTaskIndex;
    private int numberOfAvailableTasks;

    private int remainingTasksInSequence;

    private GameSettings.GameSettingsData currentData;

    void Start()
    {
        numberOfAvailableTasks = availableTasks.Length;
    }

    public void BeginTaskSequence(int lenght, GameSettings.GameSettingsData gameData) {
        currentData = gameData;
        remainingTasksInSequence = lenght;

        Debug.Log("Called sequence");

        StartNewTask();
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

        Debug.Log("Called task");
    }

    private void TaskCompleted() {
        //Remove from event reaction
        availableTasks[lastPerformedTaskIndex].OnTaskCompleted -= TaskCompleted;
        remainingTasksInSequence--;

        if(remainingTasksInSequence == 0) {
            FinishTaskSequence();
        }
        else {
            StartNewTask();
        }
    }

    private void FinishTaskSequence() {
        OnPlayerTasksCompleted?.Invoke();
    }

    private void UpdateDisplay() {

    }
}
