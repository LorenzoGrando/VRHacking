using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private MainScreenDisplay mainDisplay;
    [SerializeField]
    private TaskManager taskManager;
    [SerializeField]
    private HackerManager hackerManager;

    [SerializeField]
    private PlayerBugManager playerBugManager;

    private GameSettingsData gameSettings;

    private Coroutine invasionIntervalRoutine;

    void Start()
    {
        StartMenu();
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
        taskManager.OnPlayerTasksCompleted += CallWonGame;

        hackerManager.InitializeHackerData(gameSettingsData);
        hackerManager.BeginHackerSequence();

        playerBugManager.InitializeBugData(gameSettingsData);

        hackerManager.OnHackerBugUploaded += CommunicateBugStart;
        hackerManager.OnHackerTasksCompleted += CallLostGame;

        Debug.Log("Called game");
    }

    public void FinishSystemDispute(bool playerWon) {
        taskManager.OnPlayerTasksCompleted -= CallWonGame;
        taskManager.OnEndDispute();

        hackerManager.OnHackerBugUploaded -= CommunicateBugStart;
        hackerManager.OnHackerTasksCompleted -= CallLostGame;

        hackerManager.OnEndDispute();

        if(playerWon) { 
            if(gameSettings.thisGameMode == GameSettingsData.GameMode.Endless) {
                gameSettings.defeatedHackers++;
                gameSettings = GameSettings.SetGetGameData(gameSettings);

                invasionIntervalRoutine = StartCoroutine(NextInvasionTimer(gameSettings.newInvasionMaxIntervalTime));
            }
        }

        mainDisplay.OnEndDispute(gameSettings.thisGameMode, playerWon);
    }

    private void CallLostGame() {
        FinishSystemDispute(false);
    }

    private void CallWonGame() {
        FinishSystemDispute(true);
    }

    public void CallNewDispute() {
        //TODO: Initialize and reset displays
        if(invasionIntervalRoutine != null) {
            StopCoroutine(invasionIntervalRoutine);
        }

        BeginNewSystemDispute(gameSettings);
        mainDisplay.StartGameDisplay();
    }

    public void StartMenu() {
        taskManager.HideAllTasks();
        mainDisplay.StartMenu();
    }

    public void StartMode(int enumValue) {
        GameSettingsData.GameMode mode = (GameSettingsData.GameMode)enumValue;
        GameSettings.InitializeData(mode);
        gameSettings = GameSettings.GetGameData();

        mainDisplay.ActivateMenuByMode(mode);
    }

    private IEnumerator NextInvasionTimer(float duration) {
        float elapsedTime = 0;
        while(elapsedTime < duration) {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Lerp(0, 1, elapsedTime/duration);
            mainDisplay.UpdateCooldownSlider(t);
            yield return null;
        }
        CallNewDispute();
        yield break;
    }
}
