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
    private HackerMainDisplay hackerDisplay;

    [SerializeField]
    private PlayerBugManager playerBugManager;
    [SerializeField]
    private DialogueManager dialogueManager;
    [SerializeField]
    private SoundtrackManager soundtrackManager;

    private GameSettingsData gameSettings;

    private Coroutine invasionIntervalRoutine;

    void Start()
    {
        StartMenu();
        hackerDisplay = hackerManager.display;
    }

    void OnDisable()
    {
        hackerManager.OnHackerBugUploaded -= CommunicateBugStart;
    }

    private void CommunicateBugStart(HackerData hackerData) {
        playerBugManager.StartBugRequest(hackerData);
    }

    private void CommunicateMessageTrigger(DialogueRequestData dialogueRequest) {
        dialogueManager.OnRequestMessage(dialogueRequest);
    }

    private void BeginNewSystemDispute(GameSettingsData gameSettingsData) {
        taskManager.BeginTaskSequence(gameSettingsData);
        taskManager.OnPlayerTasksCompleted += CallWonGame;

        HackerData sequenceHacker = hackerManager.InitializeHackerData(gameSettingsData);

        dialogueManager.ResetDialogueData();
        dialogueManager.UpdateHackerDialogue(sequenceHacker);
        SetupMessageTriggers(init: true);

        hackerManager.BeginHackerSequence();

        
        playerBugManager.InitializeBugData(gameSettingsData);

        soundtrackManager.ResetVolume();
        soundtrackManager.UpdateIntensityByDifficulty(gameSettingsData.difficulty);
        soundtrackManager.InitializeTrack();

        hackerManager.OnHackerBugUploaded += CommunicateBugStart;
        hackerManager.OnHackerTasksCompleted += CallLostGame;

        Debug.Log("Called game");
    }

    private void SetupMessageTriggers(bool init) {
        if(init) {
            taskManager.OnMessageTrigger += CommunicateMessageTrigger;
            hackerManager.OnMessageTrigger += CommunicateMessageTrigger;
            hackerDisplay.OnMessageTrigger += CommunicateMessageTrigger;
        }
        else {
            taskManager.OnMessageTrigger -= CommunicateMessageTrigger;
            hackerManager.OnMessageTrigger -= CommunicateMessageTrigger;
            hackerDisplay.OnMessageTrigger -= CommunicateMessageTrigger;
        }
    }

    public void FinishSystemDispute(bool playerWon) {
        taskManager.OnPlayerTasksCompleted -= CallWonGame;
        taskManager.OnEndDispute();

        hackerManager.OnHackerBugUploaded -= CommunicateBugStart;
        hackerManager.OnHackerTasksCompleted -= CallLostGame;

        hackerManager.OnEndDispute();

        if(playerWon) {
            if(gameSettings.thisGameMode == GameSettingsData.GameMode.Endless) {
                soundtrackManager.ModifyVolume(0.5f);

                gameSettings.defeatedHackers++;
                gameSettings = GameSettings.SetGetGameData(gameSettings);

                invasionIntervalRoutine = StartCoroutine(NextInvasionTimer(gameSettings.newInvasionMaxIntervalTime));
            }
        }
        else {
            soundtrackManager.ResetVolume();
            soundtrackManager.SilenceAll();
        }

        SetupMessageTriggers(init: false);
        DialogueRequestData requestData = new DialogueRequestData {
            type = DialogueAsset.DialogueType.Hacker,
            source = playerWon ? DialogueAsset.DialogueSource.PlayerWon : DialogueAsset.DialogueSource.PlayerLost
        };
        CommunicateMessageTrigger(requestData);

        mainDisplay.OnEndDispute(gameSettings.thisGameMode, playerWon);
    }

    private void CallLostGame() {
        FinishSystemDispute(false);
    }

    private void CallWonGame() {
        FinishSystemDispute(true);
    }

    public void CallNewDispute() {
        TryEndInvasionInterval();

        BeginNewSystemDispute(gameSettings);
        mainDisplay.StartGameDisplay();
    }

    public void StartMenu() {
        TryEndInvasionInterval();
        soundtrackManager.PlayBackgroundNoise();

        taskManager.HideAllTasks();
        mainDisplay.StartMenu();
    }

    private bool TryEndInvasionInterval() {
        bool wasActive = false;
        if(invasionIntervalRoutine != null) {
            StopCoroutine(invasionIntervalRoutine);
            wasActive = true;
        }
        return wasActive;
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
