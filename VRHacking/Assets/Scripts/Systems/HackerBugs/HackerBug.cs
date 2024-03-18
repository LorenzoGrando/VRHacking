using System.Collections;
using UnityEngine;
using System;

public abstract class HackerBug : MonoBehaviour
{
    public event Action OnCooldownStart;
    public event Action OnCooldownEndTimer;
    
    [SerializeField]
    protected HackerManager hackerManager;
    public float uploadTime;
    public float duration;
    public float modifier;
    public float cooldown;

    [HideInInspector]
    public float progress;
    [HideInInspector]
    public float cooldownTimer;
    private float InitialUploadTime;

    public void BeginBugUpload() {
        InitialUploadTime = Time.time;
        StartCoroutine(routine: ExecuteBugUpload());
    }

    public virtual void OnBugUpload() {
        StartCoroutine(routine: ExecuteBugTimer());
        cooldownTimer = cooldown;
        StartCoroutine(routine:ExecuteBugCooldown());
        OnCooldownStart?.Invoke();
    }

    public abstract void OnBugEnd();

    public virtual void OnCooldownEnd() {
        OnCooldownEndTimer?.Invoke();
    }

    protected IEnumerator ExecuteBugTimer() {
        yield return new WaitForSeconds(duration);

        OnBugEnd();

        yield break;
    }

    protected IEnumerator ExecuteBugUpload() {
        while(progress <= 0.98) {
            progress = ClampedTimerData(InitialUploadTime, Time.time, uploadTime);
            Debug.Log(progress);
            yield return null;
        }

        progress = 1;

        OnBugUpload();

        yield break;
    }
    protected IEnumerator ExecuteBugCooldown() {
        while (cooldownTimer > 0) {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }

        cooldownTimer = 0;

        OnCooldownEnd();

        yield break;
    }

    private float ClampedTimerData(float initial, float current, float interval) {
        float currentTime = current - initial;

        float remaningTime = (initial + interval) - current;

        if(remaningTime <= 0)
            return 0f;
        
        float clampedValue = currentTime / (currentTime + remaningTime);

        return clampedValue;
    }
}