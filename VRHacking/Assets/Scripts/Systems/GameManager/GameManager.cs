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

    private GameSettingsData gameSettings;

    void Start()
    {
        GameSettings.InitializeData();
        gameSettings = GameSettings.GetGameData();
        CallNewDispute();
    }

    void OnDisable()
    {
        hackerManager.OnHackerBugUploaded -= CommunicateBugStart;
    }

    private void CommunicateBugStart(HackerData hackerData) {
        playerBugManager.StartBugRequest(hackerData);
    }

    private void BeginNewSystemDispute(GameSettingsData gameSettingsData) {
        taskManager.BeginTaskSequence(gameSettingsData);

        hackerManager.InitializeHackerData(gameSettingsData);
        hackerManager.BeginHackerSequence();

        playerBugManager.InitializeBugData(gameSettingsData);

        hackerManager.OnHackerBugUploaded += CommunicateBugStart;

        Debug.Log("Called game");
    }

    public void FinishSystemDispute(bool playerWon) {
        if(!playerWon) {
            //trigger Game Over
        }

        if(gameSettings.thisGameMode == GameSettingsData.GameMode.Endless) {
            gameSettings.defeatedHackers++;
            gameSettings = GameSettings.SetGetGameData(gameSettings);
        }
        
        //hide hacker and active tasks, wait for player to call continue or begin timer for next invasion
        CallNewDispute();
    }

    public void CallNewDispute() {
        //TODO: Initialize and reset displays
        BeginNewSystemDispute(gameSettings);
    }
}
