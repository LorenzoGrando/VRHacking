using System.Collections;
using UnityEngine;

public abstract class HackerBug : MonoBehaviour
{
    [SerializeField]
    protected HackerManager hackerManager;
    public float uploadTime;
    public float duration;
    public float modifier;
    public float cooldown;

    [HideInInspector]
    public float progress;
    private float InitialUploadTime;

    public void BeginBugUpload() {
        InitialUploadTime = Time.time;
        StartCoroutine(routine: ExecuteBugUpload());
    }

    public abstract void OnBugUpload();

    public abstract void OnBugEnd();

    protected IEnumerator ExecuteBugTimer() {
        yield return new WaitForSeconds(duration);

        OnBugEnd();

        yield break;
    }

    protected IEnumerator ExecuteBugUpload() {
        while(progress <= 0.96) {
            progress = ClampedTimerData(InitialUploadTime, Time.time, uploadTime);
            Debug.Log(progress);
            yield return null;
        }

        progress = 1;

        OnBugUpload();

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