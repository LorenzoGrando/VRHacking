using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class PlayerBugManager : MonoBehaviour
{
    [SerializeField]
    private List<PlayerBug> tricksterBugs;
    [SerializeField]
    private List<PlayerBug> daredevilBugs;

    private PlayerBug currentBug;
    private Queue<HackerData> bugRequestQueue;
    private GameSettingsData gameSettingsData;

    private Coroutine intervalRoutine;
    [SerializeField]
    private float bugDisplayIntervalDuration;

    public void InitializeBugData(GameSettingsData data) {
        gameSettingsData = data;
        bugRequestQueue = new Queue<HackerData>();
    }

    public void StartBugRequest(HackerData hackerData) {
        if(bugRequestQueue.Count == 0 && currentBug == null) {
            StartBug(hackerData);
        }

        else {
            bugRequestQueue.Enqueue(hackerData);
        }
    }
    
    private void StartBug(HackerData hackerData) {
        List<PlayerBug> targetList = new List<PlayerBug>();

        switch(hackerData.behaviour) {
            case HackerData.HackerBehaviour.Trickster:
                targetList = tricksterBugs;
            break;

            case HackerData.HackerBehaviour.Daredevil:
                targetList = daredevilBugs;
            break;

            default:
                targetList = tricksterBugs;
            break;
        }

        List<PlayerBug> availableBugs = targetList
            .Where(x => x.minDifficulty < gameSettingsData.difficulty)
            .ToList();

        Debug.Log("Available Bugs: " + availableBugs.Count);

        currentBug = SelectBugFromList(availableBugs);
        currentBug.OnBugCompleted += OnBugComplete;
        currentBug.StartBug(gameSettingsData);
    }

    private void OnBugComplete() {
        currentBug.OnBugCompleted -= OnBugComplete;
        currentBug = null;

        if(bugRequestQueue.Count > 0) {
            intervalRoutine = StartCoroutine(routine: ExecuteBugInterval(bugDisplayIntervalDuration));
        }
    }

    private PlayerBug SelectBugFromList(List<PlayerBug> availableBugs) {
        int index = UnityEngine.Random.Range(0, availableBugs.Count);
        return availableBugs[index];
    }


    private IEnumerator ExecuteBugInterval(float intervalDuration) {
        yield return new WaitForSeconds(intervalDuration);

        StartBug(bugRequestQueue.Dequeue());
    }
}