using UnityEngine;
using System;

public abstract class PlayerBug : MonoBehaviour
{
    public event Action OnBugCompleted;

    [SerializeField]
    protected GameObject prefabObject;

    protected abstract void ResetBug();

    public abstract void StartBug();

    public abstract bool CheckBugCompleted();

    protected virtual void CompleteBug() {
        OnBugCompleted?.Invoke();
    }
}