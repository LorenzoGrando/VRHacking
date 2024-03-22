using UnityEngine;
using System;

public abstract class PlayerBug : MonoBehaviour
{
    public event Action OnBugCompleted;
    [Range(0,2)]
    public float minDifficulty;

    [SerializeField]
    protected GameObject prefabObject;
    protected GameSettings.GameSettingsData gameSettingsData;

    protected abstract void ResetBug();

    public abstract void StartBug(GameSettings.GameSettingsData data);

    public abstract bool CheckBugCompleted();

    protected virtual void CompleteBug() {
        OnBugCompleted?.Invoke();
    }
}