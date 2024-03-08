using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TaskManager taskManager;
    [SerializeField]
    private HackerManager hackerManager;

    [SerializeField]
    private PlayerBugManager playerBugManager;

    void Start()
    {
        GameSettings.InitializeData();
        GameSettings.GameSettingsData gameSettingsData = GameSettings.GetGameData();


        taskManager.BeginTaskSequence(gameSettingsData);

        hackerManager.InitializeHackerData(gameSettingsData);
        hackerManager.BeginHackerSequence();

        playerBugManager.InitializeBugData(gameSettingsData);

        hackerManager.OnHackerBugUploaded += CommunicateBugStart;

        Debug.Log("Called game");
    }

    void OnDisable()
    {
        hackerManager.OnHackerBugUploaded -= CommunicateBugStart;
    }

    private void CommunicateBugStart(HackerData hackerData) {
        playerBugManager.StartBugRequest(hackerData);
    }
}
