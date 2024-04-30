using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HackTask : MonoBehaviour
{
    public event Action OnTaskCompleted;

    [SerializeField]
    protected GameObject prefabObject;
    protected GameSettingsData gameSettingsData;
    private GlitchManager glitchManager;

    [SerializeField]
    public float taskQuicknessTimeThreshold;
    [HideInInspector]
    public bool enableMines;

    //Reset task values and canvas to initial ones
    protected abstract void ResetTask();

    public abstract void HideTask();

    public abstract void StartTask(GameSettingsData settingsData);

    //Implement specific method for checking if task is done
    protected abstract bool CheckTaskCompleted();
    
    //Finalizes the task. Should fire OnTaskCompleted
    protected virtual void CompleteTask() {
        enableMines = false;
        OnTaskCompleted?.Invoke();
    }

    protected virtual void CallGlitch() {
        if(glitchManager == null)
            glitchManager = FindObjectOfType<GlitchManager>();

        glitchManager.CallGlitch();
    }
}
