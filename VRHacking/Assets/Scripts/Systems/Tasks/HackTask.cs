using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HackTask : MonoBehaviour
{
    public event Action OnTaskCompleted;

    [SerializeField]
    protected GameObject prefabObject;

    //Reset task values and canvas to initial ones
    protected abstract void ResetTask();

    protected abstract void StartTask();

    //Implement specific method for checking if task is done
    protected abstract bool CheckTaskCompleted();
    
    //Finalizes the task. Should fire OnTaskCompleted
    protected virtual void CompleteTask() {
        OnTaskCompleted?.Invoke();
    }
}